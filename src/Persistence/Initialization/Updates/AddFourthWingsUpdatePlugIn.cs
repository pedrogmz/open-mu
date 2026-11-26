// <copyright file="AddFourthWingsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the fourth wings of season 13 (12,414-421): Wings of Elysium, Wings of Flame God, Wing of Celestial Body,
/// Wings of Annihilation, Cloak of Dominator, Wings of Other World, Cloak of Judgment and Cloak of Isolation.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFourthWingsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFourthWingsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("205CF022-6231-432D-B6A0-65A56846CF5E")]
public class AddFourthWingsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add fourth wings";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the fourth wings of season 13: Wings of Elysium, Wings of Flame God, Wing of Celestial Body, Wings of Annihilation, Cloak of Dominator, Wings of Other World, Cloak of Judgment and Cloak of Isolation. Existing items with the same numbers are left unchanged.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The wings get the movement speed attributes, the item rule flags and a stat profile, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn =>
    [
        typeof(AddMovementSpeedAttributesPlugInSeason6).GUID,
        typeof(AddItemRuleFlagsPlugIn).GUID,
        typeof(AddItemStatProfilesUpdatePlugInSeasonSix).GUID,
        typeof(AddMonsterWingsUpdatePlugIn).GUID,
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new Wings(context, gameConfiguration).AddFourthWings();
        ItemRules.Apply(addedItems);
        ItemStatProfiles.Apply(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
