// <copyright file="LimitBaseStatsPlugIn075.cs" company="MUnique">
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
[Guid("4E1C7A93-2B6D-4F85-A0E3-9D51C8B27F14")]
public class LimitBaseStatsPlugIn075 : LimitBaseStatsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
