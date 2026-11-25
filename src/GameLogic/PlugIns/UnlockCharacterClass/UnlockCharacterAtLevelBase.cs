// <copyright file="UnlockCharacterAtLevelBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Unlocks the a character class as soon as the first character of an account reaches a certain level.
/// </summary>
/// <remarks>
/// Besides a regular level up, the unlock is also checked when the account enters the character selection.
/// This catches characters which got their level without a level up, e.g. by a game master,
/// or which reached it before the plugin was activated.
/// </remarks>
public abstract class UnlockCharacterAtLevelBase : ICharacterLevelUpPlugIn, IPlayerStateChangedPlugIn
{
    private readonly byte _classNumber;
    private readonly int _minimumLevel;
    private readonly string _warningMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockCharacterAtLevelBase"/> class.
    /// </summary>
    /// <param name="classNumber">The class number.</param>
    /// <param name="minimumLevel">The minimum level.</param>
    /// <param name="className">Name of the class.</param>
    protected UnlockCharacterAtLevelBase(byte classNumber, int minimumLevel, string className)
    {
        this._classNumber = classNumber;
        this._minimumLevel = minimumLevel;
        this._warningMessage = $"Couldn't unlock {className}, because it couldn't be found in the configuration.";
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        this.TryUnlock(player, player.Level);
    }

    /// <inheritdoc />
    public ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (currentState == PlayerState.CharacterSelection && player.Account is { } account)
        {
            var highestLevel = account.Characters
                .Select(c => c.Attributes.FirstOrDefault(a => a.Definition == Stats.Level)?.Value ?? 0)
                .DefaultIfEmpty(0)
                .Max();
            this.TryUnlock(player, highestLevel);
        }

        return ValueTask.CompletedTask;
    }

    private void TryUnlock(Player player, float level)
    {
        // Server-side bots are excluded: they never create new characters, and animating several
        // characters of one account concurrently (each with its own persistence context and thus its
        // own stale copy of the account) lets two siblings pass the duplicate check below at the same
        // time - both insert the same unlock row and the account's saves keep failing with a duplicate
        // key of "PK_AccountCharacterClass" until the next restart.
        if (level >= this._minimumLevel
            && player.Account is { IsBot: false } account
            && account.UnlockedCharacterClasses.All(c => c.Number != this._classNumber))
        {
            var unlockedClass = player.GameContext.Configuration.CharacterClasses.FirstOrDefault(c => c.Number == this._classNumber);
            if (unlockedClass is null)
            {
                player.Logger.LogWarning(this._warningMessage);
                return;
            }

            account.UnlockedCharacterClasses.Add(unlockedClass);
        }
    }
}
