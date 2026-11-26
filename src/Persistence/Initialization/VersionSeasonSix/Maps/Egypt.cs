// <copyright file="Egypt.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// The initialization for the Egypt map, a custom map of this server.
/// It has no safezone in its terrain, so it gets a spawn gate of its own where players enter and respawn.
/// </summary>
internal class Egypt : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 82;

    /// <summary>
    /// The area where players arrive on the map and respawn after death: walkable tiles of its main area.
    /// </summary>
    internal static readonly (byte X1, byte Y1, byte X2, byte Y2) SpawnGate = (120, 137, 124, 141);

    /// <summary>
    /// Initializes a new instance of the <see cref="Egypt"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Egypt(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.Egypt);

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override LocalizedString MapName => Name;

    /// <inheritdoc/>
    protected override void AdditionalInitialization(GameMapDefinition mapDefinition)
    {
        var gate = this.Context.CreateNew<ExitGate>();
        gate.Map = mapDefinition;
        gate.X1 = SpawnGate.X1;
        gate.Y1 = SpawnGate.Y1;
        gate.X2 = SpawnGate.X2;
        gate.Y2 = SpawnGate.Y2;
        gate.IsSpawnGate = true;
        mapDefinition.ExitGates.Add(gate);
    }
}
