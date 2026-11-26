// <copyright file="IJewelBankViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.JewelBank;

/// <summary>
/// The view of the jewel bank.
/// </summary>
public interface IJewelBankViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the stored amount of every jewel which the jewel bank accepts.
    /// </summary>
    /// <param name="balances">The stored amounts, one per accepted jewel.</param>
    /// <param name="openWindow">If set to <c>true</c>, the client opens its jewel bank window.</param>
    ValueTask ShowJewelBankAsync(IReadOnlyList<JewelBankBalance> balances, bool openWindow);
}
