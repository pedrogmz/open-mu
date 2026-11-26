// <copyright file="JewelBankConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.JewelBank;

using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// The configuration of the <see cref="JewelBankFeaturePlugIn"/>.
/// </summary>
public class JewelBankConfiguration
{
    /// <summary>
    /// Gets or sets the jewels which the jewel bank accepts. Their bundles are accepted, too.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.JewelBankConfiguration_Jewels_Name), Description = nameof(PlugInResources.JewelBankConfiguration_Jewels_Description))]
    public ICollection<JewelBankJewel> Jewels { get; set; } = new List<JewelBankJewel>();

    /// <summary>
    /// Gets or sets the most single jewels of one kind which the jewel bank of an account stores.
    /// </summary>
    /// <remarks>
    /// The amount is sent to the client as a 32-bit value; the stored value is a signed 32-bit value.
    /// An existing configuration without this value reads 0, which counts as <see cref="int.MaxValue"/>.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.JewelBankConfiguration_MaximumAmountPerJewel_Name), Description = nameof(PlugInResources.JewelBankConfiguration_MaximumAmountPerJewel_Description))]
    public int MaximumAmountPerJewel { get; set; } = int.MaxValue;

    /// <summary>
    /// Gets the maximum stored amount per jewel, with 0 or less read as no limit.
    /// </summary>
    internal int EffectiveMaximumAmountPerJewel => this.MaximumAmountPerJewel > 0 ? this.MaximumAmountPerJewel : int.MaxValue;

    /// <summary>
    /// Creates the default configuration, with the jewels of the season 6 item data: Bless, Soul, Chaos,
    /// Life, Creation, Guardian, Harmony, the Gemstone and the lower and higher refining stones.
    /// </summary>
    /// <returns>The default configuration.</returns>
    public static JewelBankConfiguration CreateDefault()
    {
        var configuration = new JewelBankConfiguration();
        configuration.Jewels.Add(new JewelBankJewel(14, 13)); // Jewel of Bless
        configuration.Jewels.Add(new JewelBankJewel(14, 14)); // Jewel of Soul
        configuration.Jewels.Add(new JewelBankJewel(12, 15)); // Jewel of Chaos
        configuration.Jewels.Add(new JewelBankJewel(14, 16)); // Jewel of Life
        configuration.Jewels.Add(new JewelBankJewel(14, 22)); // Jewel of Creation
        configuration.Jewels.Add(new JewelBankJewel(14, 31)); // Jewel of Guardian
        configuration.Jewels.Add(new JewelBankJewel(14, 42)); // Jewel of Harmony
        configuration.Jewels.Add(new JewelBankJewel(14, 41)); // Gemstone
        configuration.Jewels.Add(new JewelBankJewel(14, 43)); // Lower refining stone
        configuration.Jewels.Add(new JewelBankJewel(14, 44)); // Higher refining stone
        return configuration;
    }
}
