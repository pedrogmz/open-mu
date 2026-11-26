// <copyright file="JewelBankJewel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.JewelBank;

/// <summary>
/// A jewel which the jewel bank accepts, identified by the group and number of its item definition.
/// </summary>
/// <remarks>
/// The bundles of the jewel are accepted, too: they are found through the jewel mixes
/// of the game configuration, which define the bundle of each single jewel.
/// </remarks>
public class JewelBankJewel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JewelBankJewel"/> class.
    /// </summary>
    public JewelBankJewel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JewelBankJewel"/> class.
    /// </summary>
    /// <param name="itemGroup">The group of the item definition.</param>
    /// <param name="itemNumber">The number of the item definition.</param>
    public JewelBankJewel(byte itemGroup, short itemNumber)
    {
        this.ItemGroup = itemGroup;
        this.ItemNumber = itemNumber;
    }

    /// <summary>
    /// Gets or sets the group of the single jewel's item definition.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.JewelBankJewel_ItemGroup_Name), Description = nameof(PlugInResources.JewelBankJewel_ItemGroup_Description))]
    public byte ItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the number of the single jewel's item definition.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.JewelBankJewel_ItemNumber_Name), Description = nameof(PlugInResources.JewelBankJewel_ItemNumber_Description))]
    public short ItemNumber { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{this.ItemGroup}/{this.ItemNumber}";
}
