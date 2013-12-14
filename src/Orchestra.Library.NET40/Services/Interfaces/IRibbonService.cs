﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRibbonService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2012 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchestra.Services
{
    using System;
    using Models;
    using Views;

    /// <summary>
    /// Interface definition of the ribbon service.
    /// </summary>
    public interface IRibbonService
    {
        /// <summary>
        /// Registers the specified ribbon item to the main ribbon.
        /// </summary>
        /// <param name="ribbonItem">The ribbon item.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="ribbonItem" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <c>Command</c> property of the <paramref name="ribbonItem" /> is <c>null</c>.</exception>
        void RegisterRibbonItem(IRibbonControl ribbonItem);

        /// <summary>
        /// Registers the ribbon item bound to a specific view type.
        /// </summary>
        /// <typeparam name="TView">The type of the T view.</typeparam>
        /// <param name="ribbonItem">The ribbon item.</param>
        /// <param name="contextualTabGroupName">The contextual tab group name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="ribbonItem"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="contextualTabGroupName"/> is <c>null</c> or whitespace.</exception>
        void RegisterContextualRibbonItem<TView>(IRibbonControl ribbonItem, string contextualTabGroupName)
            where TView : DocumentView;

        /// <summary>
        /// Registers the ribbon item bound to a specific view type.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="ribbonItem">The ribbon item.</param>
        /// <param name="contextualTabGroupName">The contextual tab group name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="ribbonItem"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="contextualTabGroupName"/> is <c>null</c> or whitespace.</exception>
        void RegisterContextualRibbonItem(Type viewType, IRibbonControl ribbonItem, string contextualTabGroupName);
    }
}