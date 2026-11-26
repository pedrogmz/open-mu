// <copyright file="JewelBankBalance.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.JewelBank;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The stored amount of a jewel which the jewel bank accepts.
/// </summary>
/// <param name="Jewel">The definition of the single jewel.</param>
/// <param name="Amount">The stored number of single jewels.</param>
public readonly record struct JewelBankBalance(ItemDefinition Jewel, int Amount);
