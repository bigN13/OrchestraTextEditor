// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2013 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Catel;
using Catel.Messaging;
using Catel.MVVM;
using Catel.MVVM.Services;
using Orchestra.Modules.TextEditor.Models;
using Orchestra.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
namespace Orchestra.Modules.TextEditor.ViewModels
{
    /// <summary>
    /// Backing ViewModel for the PropertiesView
    /// </summary>
    public class PropertiesViewModel : Orchestra.ViewModels.ViewModelBase
    {
        #region Fields
        private readonly IMessageService _messageService;
        private readonly IOrchestraService _orchestraService;
        private readonly IMessageMediator _messageMediator;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesViewModel"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="messageService">The message service.</param>
        /// <param name="orchestraService">The orchestra service.</param>
        /// <param name="messageMediator">The message mediator.</param>
        /// <param name="contextualViewModelManager">The contextual view model manager.</param>
        public PropertiesViewModel(string title, IMessageService messageService, IOrchestraService orchestraService, IMessageMediator messageMediator, IContextualViewModelManager contextualViewModelManager)
            : this(messageService, orchestraService, messageMediator, contextualViewModelManager)
        {
            Title = title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesViewModel"/> class.
        /// </summary>
        public PropertiesViewModel(IMessageService messageService, IOrchestraService orchestraService, IMessageMediator messageMediator, IContextualViewModelManager contextualViewModelManager)
        {
            Argument.IsNotNull(() => orchestraService);
            Argument.IsNotNull(() => orchestraService);
            Argument.IsNotNull(() => messageMediator);

            _messageService = messageService;
            _orchestraService = orchestraService;
            _messageMediator = messageMediator;

            Title = "Properties";

            // Comands
            DocMapSelectedCommand = new Command(OnDocMapSelectedCommandExecute, OnDocMapSelectedCommandCanExecute);
           
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the MethodSignatureCollection
        /// </summary>
        public List<MatchItem> MethodSignatureCollection { get; set; }

        /// <summary>
        /// Gets or sets the Current FileName
        /// </summary>
        public string currentFileName { get; set; }

        /// <summary>
        /// Gets or sets the MethodSignatureCollection
        /// </summary>
        public MatchItem SelectectedDocumentItem { get; set; }

     
        #endregion

        #region DocumentMap Command
        /// <summary>
        /// Gets the ShowLineNumbers command.
        /// </summary>
        public Command DocMapSelectedCommand { get; private set; }

        /// <summary>
        /// Method to check whether the ShowLineNumbers command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnDocMapSelectedCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to send the Selected Item  to Text Editor View
        /// </summary>
        private void OnDocMapSelectedCommandExecute()
        {
            if (SelectectedDocumentItem != null)
            {
                // Passing the currentFileName to differentiate the mediator message
                _messageMediator.SendMessage(SelectectedDocumentItem, currentFileName);
            }           
        }
        #endregion

    }
}