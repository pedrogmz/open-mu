// <copyright file="JewelBankWithdrawRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the <see cref="JewelBankWithdrawRequest"/>, with which the player withdraws jewels from the jewel bank.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankWithdrawRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.JewelBankWithdrawRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A52291D6-EDAA-422F-8328-E02F149F8168")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class JewelBankWithdrawRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly JewelBankAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => JewelBankWithdrawRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < JewelBankWithdrawRequest.Length)
        {
            return;
        }

        JewelBankWithdrawRequest request = packet;
        await this._action.WithdrawAsync(player, request.ItemGroup, (short)request.ItemNumber, request.Amount).ConfigureAwait(false);
    }
}
