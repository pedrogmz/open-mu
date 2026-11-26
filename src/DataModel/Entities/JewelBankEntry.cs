// <copyright file="JewelBankEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The stored amount of one kind of jewel in the jewel bank of an account.
/// </summary>
/// <remarks>
/// The jewel bank stores jewels by their number instead of as items, so any amount of them
/// takes no space in the vault or the inventory. Bundles of jewels are stored as the
/// number of single jewels they hold.
/// </remarks>
public class JewelBankEntry
{
    /// <summary>
    /// Gets or sets the definition of the single jewel.
    /// </summary>
    public virtual ItemDefinition? Jewel { get; set; }

    /// <summary>
    /// Gets or sets the stored number of single jewels.
    /// </summary>
    public int Amount { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Amount} x {this.Jewel}";
    }
}
