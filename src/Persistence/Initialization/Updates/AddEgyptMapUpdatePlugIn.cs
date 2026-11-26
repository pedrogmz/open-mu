// <copyright file="AddEgyptMapUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the custom map Egypt (number 82) with its spawn gate and its warp list entry,
/// and lets every game server configuration host it.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddEgyptMapUpdatePlugIn_Name), Description = nameof(PlugInResources.AddEgyptMapUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8C0F4E3B-2D7A-4B61-9A55-3E1F0C6B7D82")]
public class AddEgyptMapUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Egypt map";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the custom map Egypt (number 82), its spawn gate and its warp list entry, and lets every game server host it.";

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
    /// <remarks>Version 2 adds the map to the game server configurations, without which no server hosts it.</remarks>
    public override int Version => 2;

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 11, 26, 14, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var map = gameConfiguration.Maps.FirstOrDefault(m => m.Number == Egypt.Number && m.Discriminator == 0);
        if (map is null)
        {
            var initializer = new Egypt(context, gameConfiguration);
            initializer.Initialize();
            initializer.SetSafezoneMap();
            map = gameConfiguration.Maps.First(m => m.Number == Egypt.Number && m.Discriminator == 0);
        }

        if (gameConfiguration.WarpList.All(warp => warp.Index != Gates.EgyptWarp.Index))
        {
            var warpInfo = context.CreateNew<WarpInfo>();
            warpInfo.Index = Gates.EgyptWarp.Index;
            warpInfo.Name = LocalizedString.FromResource(() => WarpNames.Egypt);
            warpInfo.Costs = Gates.EgyptWarp.Costs;
            warpInfo.LevelRequirement = Gates.EgyptWarp.LevelRequirement;
            warpInfo.Gate = map.ExitGates.First(gate => gate.IsSpawnGate);
            warpInfo.IsWarpable = true;
            gameConfiguration.WarpList.Add(warpInfo);
        }

        foreach (var serverConfiguration in await context.GetAsync<GameServerConfiguration>().ConfigureAwait(false))
        {
            if (!serverConfiguration.Maps.Contains(map))
            {
                serverConfiguration.Maps.Add(map);
            }
        }
    }
}
