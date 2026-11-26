// <copyright file="JewelBankContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.JewelBank;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.JewelBank;
using MUnique.OpenMU.Persistence;

/// <summary>
/// What a jewel bank action of one player works with: the configuration, the account and the inventory.
/// </summary>
internal sealed class JewelBankContext
{
    private readonly GameConfiguration _gameConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="JewelBankContext"/> class.
    /// </summary>
    /// <param name="configuration">The configuration of the jewel bank.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="account">The account of the player.</param>
    /// <param name="inventory">The inventory of the player.</param>
    public JewelBankContext(JewelBankConfiguration configuration, GameConfiguration gameConfiguration, Account account, IInventoryStorage inventory)
    {
        this.Configuration = configuration;
        this._gameConfiguration = gameConfiguration;
        this.Account = account;
        this.Inventory = inventory;
        this.AcceptedJewels = configuration.Jewels
            .Select(jewel => gameConfiguration.Items.FirstOrDefault(definition => definition.Group == jewel.ItemGroup && definition.Number == jewel.ItemNumber))
            .OfType<ItemDefinition>()
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Gets the configuration of the jewel bank.
    /// </summary>
    public JewelBankConfiguration Configuration { get; }

    /// <summary>
    /// Gets the account of the player.
    /// </summary>
    public Account Account { get; }

    /// <summary>
    /// Gets the inventory of the player.
    /// </summary>
    public IInventoryStorage Inventory { get; }

    /// <summary>
    /// Gets the definitions of the single jewels which the jewel bank accepts, in the order of the configuration.
    /// Configured jewels which the game configuration doesn't have are left out.
    /// </summary>
    public IReadOnlyList<ItemDefinition> AcceptedJewels { get; }

    /// <summary>
    /// Finds the accepted jewel of the given item group and number.
    /// </summary>
    /// <param name="itemGroup">The item group.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <returns>The definition of the jewel, or <c>null</c> if the jewel bank doesn't accept it.</returns>
    public ItemDefinition? FindAcceptedJewel(byte itemGroup, short itemNumber)
    {
        return this.AcceptedJewels.FirstOrDefault(jewel => jewel.Group == itemGroup && jewel.Number == itemNumber);
    }

    /// <summary>
    /// Finds the bundle of a single jewel, which the jewel mixes of the game configuration define.
    /// </summary>
    /// <param name="jewel">The definition of the single jewel.</param>
    /// <returns>The definition of the bundle, or <c>null</c> if the jewel has none.</returns>
    public ItemDefinition? FindBundle(ItemDefinition jewel)
    {
        return this._gameConfiguration.JewelMixes.FirstOrDefault(mix => mix.SingleJewel == jewel)?.MixedJewel;
    }

    /// <summary>
    /// Finds the jewel bank entry of a jewel.
    /// </summary>
    /// <param name="jewel">The definition of the single jewel.</param>
    /// <returns>The entry, or <c>null</c> if the account never stored this jewel.</returns>
    public JewelBankEntry? FindEntry(ItemDefinition jewel)
    {
        return this.Account.JewelBank.FirstOrDefault(entry => entry.Jewel == jewel);
    }

    /// <summary>
    /// Gets the jewel bank entry of a jewel, and creates it when the account never stored this jewel.
    /// </summary>
    /// <param name="jewel">The definition of the single jewel.</param>
    /// <param name="persistenceContext">The persistence context of the player.</param>
    /// <returns>The entry.</returns>
    public JewelBankEntry GetOrCreateEntry(ItemDefinition jewel, IContext persistenceContext)
    {
        if (this.FindEntry(jewel) is { } entry)
        {
            return entry;
        }

        entry = persistenceContext.CreateNew<JewelBankEntry>();
        entry.Jewel = jewel;
        this.Account.JewelBank.Add(entry);
        return entry;
    }
}
