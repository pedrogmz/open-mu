// <copyright file="JewelBankStateRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the <see cref="JewelBankStateRequest"/>, with which the open source client gets the stored jewels for its jewel bank window.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankStateRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.JewelBankStateRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("BB81CEE8-C310-4B1F-9C5A-A80D472258FE")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class JewelBankStateRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly JewelBankAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => JewelBankStateRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < JewelBankStateRequest.Length)
        {
            return;
        }

        await this._action.ShowAsync(player, false).ConfigureAwait(false);
    }
}
