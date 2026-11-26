// <copyright file="LimitBaseStatsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update limits the base stats (strength, agility, vitality, energy and leadership)
/// to <see cref="MaximumBaseStatValue"/>.
/// </summary>
/// <remarks>
/// The client shows these stats as unsigned 16-bit integers, so higher values overflowed to zero.
/// A configured maximum which is already lower is kept. It runs on new databases as well, since the
/// initial data defines no maximum for these stats.
/// </remarks>
public abstract class LimitBaseStatsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The maximum value of the base stats.
    /// </summary>
    public const float MaximumBaseStatValue = 65000;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.LimitBaseStatsPlugInBase_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.LimitBaseStatsPlugInBase_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override bool RunsOnNewDatabase => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 26, 20, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the limited base stats.
    /// </summary>
    internal static IReadOnlyList<AttributeDefinition> BaseStats { get; } =
    [
        Stats.BaseStrength,
        Stats.BaseAgility,
        Stats.BaseVitality,
        Stats.BaseEnergy,
        Stats.BaseLeadership,
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var attribute in gameConfiguration.Attributes.Where(attribute => BaseStats.Contains(attribute)))
        {
            if (attribute.MaximumValue is not { } maximumValue || maximumValue > MaximumBaseStatValue)
            {
                attribute.MaximumValue = MaximumBaseStatValue;
            }
        }

        return ValueTask.CompletedTask;
    }
}
