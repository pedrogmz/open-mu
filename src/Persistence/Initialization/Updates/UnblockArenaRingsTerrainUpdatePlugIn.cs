// <copyright file="UnblockArenaRingsTerrainUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update makes the rings of the Arena walkable, as in the map's terrain resource (Terrain7.att).
/// </summary>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.UnblockArenaRingsTerrainUpdatePlugIn_Name), Description = nameof(Properties.PlugInResources.UnblockArenaRingsTerrainUpdatePlugIn_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("3B6E2A41-7C58-4F0D-9E13-A2D47F6C0B95")]
public class UnblockArenaRingsTerrainUpdatePlugIn : UpdatePlugInBase
{
    private const int TerrainHeaderSize = 3;

    private const int TerrainSize = 256;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.UnblockArenaRingsTerrainUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.UnblockArenaRingsTerrainUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 26, 16, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the areas whose tiles lose the blocked flag, inclusive. Their other flags (safezone, no ground) stay.
    /// </summary>
    internal static IReadOnlyList<(byte X1, byte Y1, byte X2, byte Y2)> UnblockedAreas { get; } =
    [
        (16, 38, 16, 39), (35, 38, 35, 39), (53, 38, 53, 39),
        (16, 55, 16, 57), (35, 56, 35, 57), (53, 56, 53, 57),
        (101, 65, 102, 65), (103, 66, 104, 66), (105, 67, 105, 67),
        (16, 74, 16, 75), (35, 74, 35, 75), (122, 75, 122, 75),
        (53, 81, 53, 82), (16, 86, 17, 87), (29, 86, 30, 86),
        (103, 114, 112, 119), (65, 121, 65, 136), (61, 122, 64, 132),
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        const int minimumLength = TerrainHeaderSize + (TerrainSize * TerrainSize);
        const byte blocked = (byte)TerrainAttributeType.Blocked;
        foreach (var map in gameConfiguration.Maps.Where(map => map is { Number: Arena.Number, Discriminator: 0 }))
        {
            if (map.TerrainData is not { Length: >= minimumLength } terrainData
                || Tiles().All(tile => (terrainData[GetOffset(tile)] & blocked) == 0))
            {
                continue;
            }

            // A new array, so that the persistence layer recognizes the change.
            var fixedTerrainData = terrainData.ToArray();
            foreach (var tile in Tiles())
            {
                fixedTerrainData[GetOffset(tile)] &= unchecked((byte)~blocked);
            }

            map.TerrainData = fixedTerrainData;
        }

        return ValueTask.CompletedTask;
    }

    private static IEnumerable<(int X, int Y)> Tiles() =>
        UnblockedAreas.SelectMany(area =>
            Enumerable.Range(area.X1, area.X2 - area.X1 + 1)
                .SelectMany(x => Enumerable.Range(area.Y1, area.Y2 - area.Y1 + 1).Select(y => (x, y))));

    private static int GetOffset((int X, int Y) tile) => TerrainHeaderSize + (tile.Y * TerrainSize) + tile.X;
}
