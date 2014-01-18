
using Catel;
using Catel.Logging;
using Catel.Messaging;
using Catel.MVVM;
using Catel.MVVM.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Catel.IoC;
using System.Linq;
using System.Text;

namespace Orchestra.Modules.TextEditor.ViewModels
{
    /// <summary>
    /// DocMapSettingsViewModel
    /// </summary>
    public class DocumentMapViewModel : Orchestra.ViewModels.ViewModelBase
    {
        #region Variables
        //private readonly string _path;

        private readonly IMessageMediator _messageMediator;

        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Gets or sets the Current ViewModelUniqueIdentifier
        /// </summary>
        public int CurrentViewModelUniqueIdentifier { get; set; }

        /// <summary>
        /// Regex for Document Map
        /// </summary>
        public string RegexContent { get;  set; }

        #endregion

        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMapViewModel"/> class.
        /// </summary>
        public DocumentMapViewModel(string Regex)
        {
            RegexContent = Regex;

            SaveRegex = new Command(OnSaveRegexExecute);

            _messageMediator = Catel.IoC.ServiceLocator.Default.ResolveType<IMessageMediator>();

        }
        #endregion

        /// <summary>
        /// Gets the SaveRegex command.
        /// </summary>
        public Command SaveRegex { get; private set; }

        /// <summary>
        /// Method to invoke when the SaveRegex command is executed.
        /// </summary>
        private void OnSaveRegexExecute()
        {
            // Passing the currentFileName to differentiate the mediator message
            _messageMediator.SendMessage(RegexContent);

            // Close current window 
            CloseViewModel(true);
        }
    }
}
