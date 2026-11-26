// <copyright file="LimitBaseStatsPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update limits the base stats to 65000.
/// </summary>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.LimitBaseStatsPlugInBase_Name), Description = nameof(Properties.PlugInResources.LimitBaseStatsPlugInBase_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("C2958E4B-6F17-4A3D-B8C0-15E7A4D96B28")]
public class LimitBaseStatsPlugInSeason6 : LimitBaseStatsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
