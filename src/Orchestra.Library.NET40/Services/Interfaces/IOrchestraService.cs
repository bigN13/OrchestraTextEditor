﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrchestraService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2013 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchestra.Services
{
    using System;
    using Catel.MVVM;
    using Orchestra.Models;
    using Views;

    /// <summary>
    /// The orchestra service that allows communication with the shell.
    /// </summary>
    public interface IOrchestraService
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to show the debug window.
        /// </summary>
        /// <value><c>true</c> if the debug window should be shown; otherwise, <c>false</c>.</value>
        bool ShowDebuggingWindow { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Shows the document in the main shell.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="tag">The tag.</param>
        void ShowDocument<TViewModel>(object tag = null)
            where TViewModel : IViewModel;

        /// <summary>
        /// Opens a new document.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="dockingSettings">The docking settings.</param>
        void ShowDocument<TViewModel>(TViewModel viewModel, object tag = null, DockingSettings dockingSettings = null)
            where TViewModel : IViewModel;

        /// <summary>
        /// Shows the document in the main shell.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="tag">The tag.</param>
        void ShowContextSensitiveDocument<TViewModel>(object tag = null)
            where TViewModel : IViewModel;

        /// <summary>
        /// Shows the document in the main shell.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The view model to show which will automatically be resolved to a view.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="dockingSettings">The docking settings.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        void ShowContextSensitiveDocument<TViewModel>(TViewModel viewModel, object tag = null, DockingSettings dockingSettings = null)
            where TViewModel : IViewModel;

        /// <summary>
        /// Shows the document if it is hidden.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="tag">The tag.</param>
        void ShowDocumentIfHidden<TViewModel>(object tag = null)
            where TViewModel : IViewModel;

        /// <summary>
        /// Shows the document in nested dock view.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="dockingManager">The docking manager.</param>
        /// <param name="dockSettings">The dock settings.</param>
        /// <param name="tag">The tag.</param>
        void ShowDocumentInNestedDockView(IViewModel viewModel, NestedDockingManager dockingManager, DockingSettings dockSettings, object tag = null);

        /// <summary>
        /// Closes the document in the main shell with the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        void CloseDocument(IViewModel viewModel, object tag = null);
        #endregion
    }
}