// <copyright file="JewelBankViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.JewelBank;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.JewelBank;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The view of the jewel bank for the open source client, which sends the <see cref="JewelBankStateExtended"/>.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankViewPlugIn_Name), Description = nameof(PlugInResources.JewelBankViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("F1F12523-309D-40F5-8FA1-BD04720DF52D")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class JewelBankViewPlugIn : IJewelBankViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="JewelBankViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public JewelBankViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowJewelBankAsync(IReadOnlyList<JewelBankBalance> balances, bool openWindow)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // The count field is one byte. Configured jewel lists are far below that.
        var entries = balances.Take(byte.MaxValue).ToList();

        int Write()
        {
            var size = JewelBankStateExtendedRef.GetRequiredSize(entries.Count);
            var packet = new JewelBankStateExtendedRef(connection.Output.GetSpan(size)[..size])
            {
                OpenWindow = openWindow,
                Count = (byte)entries.Count,
            };

            for (var i = 0; i < entries.Count; i++)
            {
                var balance = entries[i];
                var entry = packet[i];
                entry.ItemGroup = balance.Jewel.Group;
                entry.ItemNumber = (ushort)balance.Jewel.Number;
                entry.Amount = (uint)Math.Max(balance.Amount, 0);
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
