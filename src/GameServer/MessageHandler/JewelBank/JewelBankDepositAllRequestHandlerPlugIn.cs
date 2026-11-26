// <copyright file="JewelBankDepositAllRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.JewelBank;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.JewelBank;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the <see cref="JewelBankDepositAllRequest"/>, with which the player deposits all accepted jewels in the jewel bank.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankDepositAllRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.JewelBankDepositAllRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("DB3C3FCE-E6CE-4E7C-A80E-655026064A75")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class JewelBankDepositAllRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly JewelBankAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => JewelBankDepositAllRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < JewelBankDepositAllRequest.Length)
        {
            return;
        }

        await this._action.DepositAllAsync(player).ConfigureAwait(false);
    }
}
