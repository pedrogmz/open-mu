// <copyright file="JewelBankFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.JewelBank;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The jewel bank: every account can store jewels by their number, so that they take no space
/// in the vault or the inventory. The open source client shows it in its jewel bank window.
/// </summary>
/// <remarks>
/// The original game doesn't have a jewel bank, so it's disabled by default.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.JewelBankFeaturePlugIn_Name), Description = nameof(PlugInResources.JewelBankFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("86D28853-AB58-41F0-ACB9-F380ABBCE333")]
public class JewelBankFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<JewelBankConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <inheritdoc />
    public JewelBankConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the configuration of the jewel bank, if it's active.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration, or <c>null</c> when the jewel bank isn't active.</returns>
    public static JewelBankConfiguration? GetConfiguration(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<JewelBankFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? JewelBankConfiguration.CreateDefault()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return JewelBankConfiguration.CreateDefault();
    }
}
