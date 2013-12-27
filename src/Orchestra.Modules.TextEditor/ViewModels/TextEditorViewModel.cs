// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2012 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchestra.Modules.TextEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Data;
    using Catel.MVVM;
    using Catel.MVVM.Services;
    using Catel.Messaging;
    using Models;
    using Orchestra.Services;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Document;
    using System.IO;
    using ICSharpCode.AvalonEdit.Utils;
    using System.Text;
    using System.Windows;
    using Microsoft.Win32;
    using System.Text.RegularExpressions;
    using ICSharpCode.AvalonEdit.Rendering;
    using Catel.Logging;
    using Orchestra.Models;

    /// <summary>
    /// UserControl view model.
    /// </summary>
    public partial class TextEditorViewModel : Orchestra.ViewModels.ViewModelBase, IContextualViewModel
    {
        #region Variables - Properties
        private readonly IMessageService _messageService;
        private readonly IOrchestraService _orchestraService;
        private readonly IMessageMediator _messageMediator;
        private readonly IContextualViewModelManager _contextualViewModelManager;

        private PropertiesViewModel _propertiesViewModel;

        private TextEditorModule _textEditorModule;

        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditorViewModel" /> class.
        /// Main TextEdtior Class is passed by reference to this Class
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="messageService">The message service.</param>
        /// <param name="orchestraService">The orchestra service.</param>
        /// <param name="messageMediator">The message mediator.</param>
        /// <param name="contextualViewModelManager">The contextual view model manager.</param>
        /// <param name="textEditorModule">The Main Module Class.</param>
        public TextEditorViewModel(string title, TextEditorModule textEditorModule, IMessageService messageService, IOrchestraService orchestraService, IMessageMediator messageMediator, IContextualViewModelManager contextualViewModelManager)
            : this(textEditorModule, messageService, orchestraService, messageMediator, contextualViewModelManager)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
            _textEditorModule = textEditorModule;
            // Set Highlightning to C#
            this.HighlightDef = HighlightingManager.Instance.GetDefinition("C#");
            //this._isDirty = false;
            this.IsReadOnly = false;
            this.ShowLineNumbers = true;
            this.WordWrap = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditorViewModel" /> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="orchestraService">The orchestra service.</param>
        /// <param name="messageMediator">The message mediator.</param>
        /// <param name="contextualViewModelManager">The contextual view model manager.</param>
        /// <param name="textEditorModule">The Main Module Class.</param>
        public TextEditorViewModel(TextEditorModule textEditorModule, IMessageService messageService, IOrchestraService orchestraService, IMessageMediator messageMediator, IContextualViewModelManager contextualViewModelManager)
        {
            Argument.IsNotNull(() => orchestraService);
            Argument.IsNotNull(() => orchestraService);
            Argument.IsNotNull(() => messageMediator);

            _messageService = messageService;
            _orchestraService = orchestraService;
            _messageMediator = messageMediator;
            _contextualViewModelManager = contextualViewModelManager;
            _textEditorModule = textEditorModule;

            #region TextEditor related
            TextOptions = new TextEditorOptions() { ShowSpaces = true };


            // Set Highlightning to C#
            this.HighlightDef = HighlightingManager.Instance.GetDefinition("C#");
            //this._isDirty = false;
            this.IsReadOnly = false;
            this.ShowLineNumbers = true;
            this.WordWrap = false;

            // Comands
            ShowLineNumbersCommand = new Command(OnShowLineNumbersCommandExecute, OnShowLineNumbersCommandCanExecute);
            WordWrapCommand = new Command(OnWordWrapCommandExecute, OnWordWrapCommandCanExecute);
            EndLineCommand = new Command(OnEndLineCommandExecute, OnEndLineCommandCanExecute);
            ShowSpacesCommand = new Command(OnShowSpacesCommandExecute, OnShowSpacesCommandCanExecute);
            ShowTabCommand = new Command(OnShowTabCommandExecute, OnShowTabCommandCanExecute);

            SaveAsCommand = new Command(OnSaveAsCommandExecute, OnSaveAsCommandCanExecute);
            SaveCommand = new Command(OnSaveCommandExecute, OnSaveCommandCanExecute);
            CloseDocument = new Command(OnCloseDocumentExecute, OnCloseCommandCanExecute);
            UpdateCommand = new Command(OnUpdateCommandExecute, OnUpdateCommandCanExecute);
            #endregion

            #region Document related

            //CloseDocument = new Command(OnCloseDocumentExecute);
            this.Title = FileName;
            #endregion

            // Invalidate the current viewmodel
            ViewModelActivated();
        }

        private void OnTestExecute()
        {
            _messageService.ShowInformation("This is a test, for loading dynamic content into the ribbon...");
        }
        #endregion

        #region Changing language

        /// <summary>
        /// Gets the recent sites.
        /// </summary>
        /// <value>
        /// The recent sites.
        /// </value>
        public string[] SyntaxHighlighting { get { return new[] { "XML", "C#", "C++", "PHP", "Java" }; } }

        /// <summary>
        /// Gets or sets the SelectedSite value.
        /// </summary>
        public string SelectedLanguage
        {
            get { return GetValue<string>(SelectedLanugageProperty); }
            set { SetValue(SelectedLanugageProperty, value); }
        }

        /// <summary>
        /// SelectedSite property data.
        /// </summary>
        public static readonly PropertyData SelectedLanugageProperty = RegisterProperty("SelectedLanguage", typeof(string), null,
            (sender, e) => ((TextEditorViewModel)sender).OnSelectedLanguageChanged());

        /// <summary>
        /// Called when the SelectedSite value changed.
        /// </summary>
        private void OnSelectedLanguageChanged()
        {
            switch (SelectedLanguage)
            {
                // TODO: Implement logic for changing Programming language

                //case "XML":
                //    Url = "http://www.github.com/Orcomp/Orchestra";
                //    break;

                //case "C#":
                //    Url = "http://www.catelproject.com";
                //    break;

                default:
                    return;
            }

            //_this.OnBrowseExecute();
        }

        #endregion

        #region ViewModel Related

        /// <summary>
        /// Method is called when the active view changes within the orchestra application
        /// </summary>
        public void ViewModelActivated()
        {
            UpdateContextSensitiveData();
        }

        /// <summary>
        /// Saves the data.
        /// Called when the view model is Closing via x button
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        protected override bool Save()
        {
            if (_messageService.Show("Are you sure you want to close this window?", "Are you sure?", MessageButton.YesNo) == MessageResult.Yes)
            {
                // Remove this file from Collection
                _textEditorModule.Close(this);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the context sensitive data, related to current view.
        /// Everytime the View is changed 
        /// the activeview source is parsed in MethodsCollection and sent to the _propertiesViewModel
        /// to update the document map
        /// </summary>
        private void UpdateContextSensitiveData()
        {
            if (_propertiesViewModel == null)
            {
                _propertiesViewModel = _contextualViewModelManager.GetViewModelForContextSensitiveView<PropertiesViewModel>();
            }
            else
            {
                if (_propertiesViewModel.MethodSignatureCollection != null)
                {
                    _propertiesViewModel.MethodSignatureCollection.Clear();
                }

                _propertiesViewModel.MethodSignatureCollection = MethodsCollection();
                _propertiesViewModel.CurrentViewModelUniqueIdentifier = this.UniqueIdentifier;
            }
        }
        
        #endregion

        #region Document map
        /// <summary>
        /// Responsible for parsing the Text and return the collection 
        /// for Document Map
        /// </summary>
        /// <returns></returns>
        private List<MatchItem> MethodsCollection()
        {
            List<MatchItem> methodsCollection = new List<MatchItem>();

            Regex r = null;
            Match m;
            RegexOptions TheOptions = RegexOptions.None;
            TheOptions |= RegexOptions.IgnoreCase;
            TheOptions |= RegexOptions.Multiline;
            TheOptions |= RegexOptions.IgnorePatternWhitespace;

            #region Testing regex
            //string regextPattern = @"@Q(?:[^Q]+|QQ)*Q|Q(?:[^Q\\]+|\\.)*Q".Replace('Q', '\"');

            //Finds all strings
            //string regextPattern = @"((private)|(public)|(sealed)|(protected)|(virtual)|(internal))+([a-z]|[A-Z]|[0-9]|[\s])*([\()([a-z]|[A-Z]|[0-9]|[\s])*([\)|\{]+)";

            //Finds All Methods
            //string regextPattern = @"((private)|(public)|(sealed)|(protected)|(virtual)|(internal))+([a-z]|[A-Z]|[0-9]|[\s])*([\()([a-z]|[A-Z]|[0-9]|[\s])*([\)|\(]+)";
            //string regextPattern2 = @"\s+|(<<|>>|\+\+|--|==|\!=|>=|<=|\{|\}|\[|\]|\(|\)|\.|,|:|;|\+|-|\*|/|%|&|\||\^|!|~|=|\<|\>|\?)";
            //string regextPattern2 = @"^(?=.*?\b(private|public|sealed)\b)(?=.*?\b(\b)(?=.*?\b)\b).*$";
            //string regextPattern = @"((private)|(public)|(sealed)|(protected)|(virtual)|(internal))+([a-z]|[A-Z]|[0-9]|[\s])*([\()([a-z]|[A-Z]|[0-9]|[\s])*([\)|\{]+)";
            //m = Regex.Match(this._document.Text, regextPattern2);
            //MatchCollection MethodSignatureCollection = Regex.Match(this._document.Text, regextPattern);

            // Matches a complete line of text that contains any of the words "private", "public" etc.
            // The first backreference will contain the word the line actually contains. If it contains more than one of the words, 
            // then the last (rightmost) word will be captured into the first backreference. This is because the star is greedy.
            // Finally, .*$ causes the regex to actually match the line, after the lookaheads have determined it meets the requirements.
            #endregion

            // Specify the correct match pattern
            // need to be altered for different language
            string regextPattern2 = @"^.*\b(namespace|private|public|sealed|protected|virtual|internal)\b.*$";

            try
            {
                r = new Regex(regextPattern2, TheOptions);
            }
            catch (Exception ex)
            {
                _messageService.ShowError("There was an error in the regular expression!\n\n"
                    + ex.Message + "\n", "Error");
            }

            for (int i = 1; i < _document.LineCount; i++)
            {
                // Get the current line number
                DocumentLine line = _document.GetLineByNumber(i);

                // Try to Match the contect fo line with specified pattern
                m = r.Match(_document.GetText(line));

                // If line is has match create new MatchItem and add it 
                // to collection
                if (m.Success)
                {
                    MatchItem mi = new MatchItem();
                    mi.currentLine = i;
                    mi.currentMatch = m;
                    methodsCollection.Add(mi);
                }
            }
            return methodsCollection;
        }
        #endregion
    }
}