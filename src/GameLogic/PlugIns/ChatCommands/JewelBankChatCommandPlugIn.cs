// <copyright file="JewelBankChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.JewelBank;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command which opens the jewel bank window of the open source client.
/// </summary>
[Guid("4F53BDB3-07A5-4515-A6C3-DBB2291693DC")]
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankChatCommandPlugIn_Name), Description = nameof(PlugInResources.JewelBankChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(Arguments), CharacterStatus.Normal)]
public class JewelBankChatCommandPlugIn : ChatCommandPlugInBase<JewelBankChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    /// <summary>
    /// The command.
    /// </summary>
    internal const string Command = "/banco";

    private readonly JewelBankAction _action = new();

    /// <inheritdoc />
    public override string Key => Command;

    /// <summary>
    /// Gets the minimum status: every player has a jewel bank.
    /// </summary>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        await this._action.ShowAsync(player, true).ConfigureAwait(false);
    }

    /// <summary>
    /// The arguments of the command; it has none.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
    }
}
