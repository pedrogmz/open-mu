// <copyright file="LimitBaseStatsPlugIn095d.cs" company="MUnique">
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
[Guid("7A3F0C58-91E2-4B6A-8D47-E5B20F9C3D61")]
public class LimitBaseStatsPlugIn095d : LimitBaseStatsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
