// <copyright file="AddMonsterWingsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the monster wings ("2.5 wings"): Cloak of Death, Wings of Chaos, Wings of Magic and Wings of Life.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddMonsterWingsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddMonsterWingsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("46065BA3-0BC2-4D0C-B6FC-2BE35F9941C9")]
public class AddMonsterWingsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add monster wings";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the monster wings: Cloak of Death, Wings of Chaos, Wings of Magic and Wings of Life. Existing items with the same numbers are left unchanged.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 26, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The wings get the movement speed attributes, the item rule flags and a stat profile, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn =>
    [
        typeof(AddMovementSpeedAttributesPlugInSeason6).GUID,
        typeof(AddItemRuleFlagsPlugIn).GUID,
        typeof(AddItemStatProfilesUpdatePlugInSeasonSix).GUID,
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new Wings(context, gameConfiguration).AddMonsterWings();
        ItemRules.Apply(addedItems);
        ItemStatProfiles.Apply(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
