// <copyright file="JewelBankDepositRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the <see cref="JewelBankDepositRequest"/>, with which the player deposits one kind of jewel in the jewel bank.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankDepositRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.JewelBankDepositRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D892E23C-8D59-4F22-8A8B-E07B7925DD66")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class JewelBankDepositRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly JewelBankAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => JewelBankDepositRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < JewelBankDepositRequest.Length)
        {
            return;
        }

        JewelBankDepositRequest request = packet;
        await this._action.DepositAsync(player, request.ItemGroup, (short)request.ItemNumber).ConfigureAwait(false);
    }
}
