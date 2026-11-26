// <copyright file="JewelBankAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.JewelBank;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.JewelBank;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.JewelBank;

/// <summary>
/// The actions of the jewel bank: show it, deposit jewels from the inventory and withdraw them again.
/// </summary>
/// <remarks>
/// <para>
/// Jewels are stored by their number. A bundle of the jewel mixes (the "stacked" jewels of the
/// jewel mix NPC Lahap) is stored as the number of single jewels it holds: 10 for level 0,
/// 20 for level 1 and 30 for level 2, like the jewel mix combines and dismantles them.
/// </para>
/// <para>
/// A withdrawal is put into the inventory as few items as possible: bundles of 30, 20 and 10 first,
/// then single jewels. Every item is taken from the bank only when it was put into the inventory,
/// so a full inventory stops the withdrawal without losing or creating jewels.
/// </para>
/// </remarks>
public class JewelBankAction
{
    /// <summary>
    /// The number of single jewels per level of a bundle: a bundle holds (level + 1) times this number.
    /// </summary>
    /// <remarks>
    /// It's the same rule as the jewel mix of <see cref="Items.ItemStackAction"/> and the item data of the client use.
    /// </remarks>
    internal const int JewelsPerBundleLevel = 10;

    /// <summary>
    /// The highest level of a bundle, which holds 30 jewels.
    /// </summary>
    internal const byte MaximumBundleLevel = 2;

    /// <summary>
    /// What a deposit did.
    /// </summary>
    [Flags]
    private enum DepositResult
    {
        /// <summary>
        /// Nothing was deposited.
        /// </summary>
        None = 0,

        /// <summary>
        /// At least one item was deposited.
        /// </summary>
        Deposited = 1,

        /// <summary>
        /// At least one item was left in the inventory, because the bank reached its maximum amount.
        /// </summary>
        LimitReached = 2,
    }

    /// <summary>
    /// Shows the jewel bank to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="openWindow">If set to <c>true</c>, the client opens its jewel bank window.</param>
    public async ValueTask ShowAsync(Player player, bool openWindow)
    {
        if (GetContext(player) is not { } context)
        {
            return;
        }

        await ShowBalancesAsync(player, context, openWindow).ConfigureAwait(false);
    }

    /// <summary>
    /// Deposits all jewels of one kind, single jewels and their bundles, from the inventory into the jewel bank.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="itemGroup">The group of the single jewel's item definition.</param>
    /// <param name="itemNumber">The number of the single jewel's item definition.</param>
    public async ValueTask DepositAsync(Player player, byte itemGroup, short itemNumber)
    {
        if (GetContext(player) is not { } context)
        {
            return;
        }

        if (context.FindAcceptedJewel(itemGroup, itemNumber) is not { } jewel)
        {
            player.Logger.LogWarning("Jewel bank deposit refused: {Player} sent jewel {Group}/{Number}, which the jewel bank doesn't accept.", player, itemGroup, itemNumber);
            return;
        }

        var result = await DepositJewelAsync(player, context, jewel).ConfigureAwait(false);
        await ShowResultMessageAsync(player, result).ConfigureAwait(false);
        await ShowBalancesAsync(player, context, false).ConfigureAwait(false);
    }

    /// <summary>
    /// Deposits every jewel which the jewel bank accepts from the inventory into the jewel bank.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask DepositAllAsync(Player player)
    {
        if (GetContext(player) is not { } context)
        {
            return;
        }

        var result = DepositResult.None;
        foreach (var jewel in context.AcceptedJewels)
        {
            result |= await DepositJewelAsync(player, context, jewel).ConfigureAwait(false);
        }

        await ShowResultMessageAsync(player, result).ConfigureAwait(false);
        await ShowBalancesAsync(player, context, false).ConfigureAwait(false);
    }

    /// <summary>
    /// Withdraws an amount of one kind of jewel from the jewel bank into the inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="itemGroup">The group of the single jewel's item definition.</param>
    /// <param name="itemNumber">The number of the single jewel's item definition.</param>
    /// <param name="amount">The number of single jewels to withdraw.</param>
    public async ValueTask WithdrawAsync(Player player, byte itemGroup, short itemNumber, uint amount)
    {
        if (GetContext(player) is not { } context)
        {
            return;
        }

        if (context.FindAcceptedJewel(itemGroup, itemNumber) is not { } jewel)
        {
            player.Logger.LogWarning("Jewel bank withdrawal refused: {Player} sent jewel {Group}/{Number}, which the jewel bank doesn't accept.", player, itemGroup, itemNumber);
            return;
        }

        var entry = context.FindEntry(jewel);
        if (amount == 0 || entry is null || entry.Amount < amount)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.JewelBankNotEnoughJewels)).ConfigureAwait(false);
            await ShowBalancesAsync(player, context, false).ConfigureAwait(false);
            return;
        }

        var remaining = (int)amount;
        foreach (var (definition, level, count) in GetWithdrawalItems(jewel, context.FindBundle(jewel), remaining))
        {
            if (!await TryPutIntoInventoryAsync(player, context.Inventory, definition, level).ConfigureAwait(false))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InventoryNotEnoughSpace)).ConfigureAwait(false);
                break;
            }

            entry.Amount -= count;
            remaining -= count;
        }

        player.Logger.LogInformation("Jewel bank: {Player} withdrew {Amount} of {Jewel}, {Stored} stored.", player, (int)amount - remaining, jewel, entry.Amount);
        await ShowBalancesAsync(player, context, false).ConfigureAwait(false);
    }

    private static JewelBankContext? GetContext(Player player)
    {
        if (JewelBankFeaturePlugIn.GetConfiguration(player.GameContext) is not { } configuration
            || player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || player.Account is not { } account
            || player.Inventory is not { } inventory)
        {
            return null;
        }

        return new JewelBankContext(configuration, player.GameContext.Configuration, account, inventory);
    }

    private static async ValueTask ShowBalancesAsync(Player player, JewelBankContext context, bool openWindow)
    {
        var balances = context.AcceptedJewels
            .Select(jewel => new JewelBankBalance(jewel, context.FindEntry(jewel)?.Amount ?? 0))
            .ToList();
        await player.InvokeViewPlugInAsync<IJewelBankViewPlugIn>(p => p.ShowJewelBankAsync(balances, openWindow)).ConfigureAwait(false);
    }

    private static async ValueTask ShowResultMessageAsync(Player player, DepositResult result)
    {
        if (result.HasFlag(DepositResult.LimitReached))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.JewelBankLimitReached)).ConfigureAwait(false);
        }
        else if (result == DepositResult.None)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.JewelBankNoJewelsToDeposit)).ConfigureAwait(false);
        }
    }

    private static async ValueTask<DepositResult> DepositJewelAsync(Player player, JewelBankContext context, ItemDefinition jewel)
    {
        var bundle = context.FindBundle(jewel);
        var items = context.Inventory.Items
            .Where(IsInMainInventory)
            .Select(item => (Item: item, Count: GetJewelCount(item, jewel, bundle)))
            .Where(tuple => tuple.Count > 0)
            .ToList();
        if (items.Count == 0)
        {
            return DepositResult.None;
        }

        var result = DepositResult.None;
        var entry = context.GetOrCreateEntry(jewel, player.PersistenceContext);
        var maximum = context.Configuration.EffectiveMaximumAmountPerJewel;
        foreach (var (item, count) in items)
        {
            if (entry.Amount > maximum - count)
            {
                result |= DepositResult.LimitReached;
                continue;
            }

            await RemoveFromInventoryAsync(player, context.Inventory, item).ConfigureAwait(false);
            entry.Amount += count;
            result |= DepositResult.Deposited;
        }

        player.Logger.LogInformation("Jewel bank: {Player} deposited {Jewel}, {Stored} stored.", player, jewel, entry.Amount);
        return result;
    }

    private static bool IsInMainInventory(Item item)
    {
        return item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex
               && item.ItemSlot < InventoryConstants.FirstStoreItemSlotIndex;
    }

    /// <summary>
    /// Gets the number of single jewels an item counts for, or 0 if it isn't the jewel or its bundle.
    /// </summary>
    /// <remarks>
    /// A single jewel with more than one piece of durability would be a stack of the item, which the
    /// jewels don't have; it's left in the inventory instead of being counted as one.
    /// </remarks>
    private static int GetJewelCount(Item item, ItemDefinition jewel, ItemDefinition? bundle)
    {
        if (item.Definition == jewel)
        {
            return item.Level == 0 && item.Durability <= 1 ? 1 : 0;
        }

        if (bundle is not null && item.Definition == bundle && item.Level <= MaximumBundleLevel)
        {
            return (item.Level + 1) * JewelsPerBundleLevel;
        }

        return 0;
    }

    private static IEnumerable<(ItemDefinition Definition, byte Level, int Count)> GetWithdrawalItems(ItemDefinition jewel, ItemDefinition? bundle, int amount)
    {
        if (bundle is not null)
        {
            for (int level = MaximumBundleLevel; level >= 0; level--)
            {
                var count = (level + 1) * JewelsPerBundleLevel;
                while (amount >= count)
                {
                    yield return (bundle, (byte)level, count);
                    amount -= count;
                }
            }
        }

        for (; amount > 0; amount--)
        {
            yield return (jewel, 0, 1);
        }
    }

    private static async ValueTask RemoveFromInventoryAsync(Player player, IInventoryStorage inventory, Item item)
    {
        var slot = item.ItemSlot;
        await inventory.RemoveItemAsync(item).ConfigureAwait(false);
        await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(slot)).ConfigureAwait(false);
    }

    private static async ValueTask<bool> TryPutIntoInventoryAsync(Player player, IInventoryStorage inventory, ItemDefinition definition, byte level)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Level = level;
        item.Durability = 1;
        if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
        {
            await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
            return false;
        }

        await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        return true;
    }
}
