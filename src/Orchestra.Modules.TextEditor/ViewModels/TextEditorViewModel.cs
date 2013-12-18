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
    public class TextEditorViewModel : Orchestra.ViewModels.ViewModelBase, IContextualViewModel
    {
        #region Variables - Properties
        private readonly List<string> _previousPages = new List<string>();
        private readonly List<string> _nextPages = new List<string>();

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

            ViewModelActivated();

            #region Mediators
            //var messageMediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
            //messageMediator.Register<string>(this, OnParse, "selectedItem");
            #endregion
        }

        //private void OnParse(string SelectedItem)
        //{
        //    //MessageBox.Show(SelectedItem);

        //    if (SelectedItem !=null)
        //    {
        //        //TextView textView = this._document.TextArea.TextView;

        //        this._document.GetLineByOffset(15);
        //    }

        //    //Match m = (Match)SelectedItem;
        //    //webBrowser.Navigate(url, null, null, string.Format("User-Agent: {0}", UserAgent));
        //}

        private void OnTestExecute()
        {
            _messageService.ShowInformation("This is a test, for loading dynamic content into the ribbon...");
        }
        #endregion

        #region TextEditor View SettinFgs

        #region FilePath
        private string _filePath = null;

        /// <summary>
        /// TextEditor Setup FilePath
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    RaisePropertyChanged("FilePath");
                    RaisePropertyChanged("FileName");
                    RaisePropertyChanged("Title");

                    if (File.Exists(this._filePath))
                    {
                        //this._document = new TextDocument();
                        this.Document = new TextDocument();
                        this.HighlightDef = HighlightingManager.Instance.GetDefinition("C#");
                        this.IsDirty = false;
                        this.IsReadOnly = false;
                        this.ShowLineNumbers = true;
                        this.WordWrap = false;

                        // Check file attributes and set to read-only if file attributes indicate that
                        if ((System.IO.File.GetAttributes(this._filePath) & FileAttributes.ReadOnly) != 0)
                        {
                            this.IsReadOnly = true;
                            //this.IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                            //                        "Change the file access permissions or save the file in a different location if you want to edit it.";
                        }

                        using (FileStream fs = new FileStream(this._filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                            {
                                //this._document = new TextDocument(reader.ReadToEnd());
                                this.Document = new TextDocument(reader.ReadToEnd());
                            }
                        }

                        ContentId = _filePath;
                    }
                }
            }
        }
        #endregion

        #region FileName
        /// <summary>
        /// TextEditor - Specify the Name of Sheet
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return "Noname" + (IsDirty ? "*" : "");

                this.Title = System.IO.Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
                return this.Title;
            }
        }
        #endregion FileName

        #region TextContent

        private TextDocument _document = new TextDocument();
        /// <summary>
        /// TextEditpr New Document creation
        /// </summary>
        public TextDocument Document
        {
            get { return this._document; }
            set
            {
                if (this._document != value)
                {
                    this._document = value;
                    RaisePropertyChanged("Document");

                    // Invalidate the ViewModel
                    //ViewModelActivated();

                    IsDirty = true;
                }
            }
        }

        #endregion

        #region HighlightingDefinition

        private IHighlightingDefinition _highlightdef = null;

        /// <summary>
        /// TextEditor Highligt Option
        /// </summary>
        public IHighlightingDefinition HighlightDef
        {
            get { return this._highlightdef; }
            set
            {
                if (this._highlightdef != value)
                {
                    this._highlightdef = value;
                    RaisePropertyChanged("HighlightDef");
                    IsDirty = true;
                }
            }
        }

        #endregion


        #region WordWrap
        // Toggle state WordWrap
        private bool mWordWrap = false;

        /// <summary>
        /// TextEditor Word Wrap Option
        /// </summary>
        public bool WordWrap
        {
            get
            {
                return this.mWordWrap;
            }

            set
            {
                if (this.mWordWrap != value)
                {
                    this.mWordWrap = value;
                    this.RaisePropertyChanged("WordWrap");
                }
            }
        }
        #endregion WordWrap

        #region ShowLineNumbers
        // Toggle state ShowLineNumbers
        private bool mShowLineNumbers = false;

        /// <summary>
        /// TextEditor Show Line Numbee Option
        /// </summary>
        public bool ShowLineNumbers
        {
            get
            {
                return this.mShowLineNumbers;
            }

            set
            {
                if (this.mShowLineNumbers != value)
                {
                    this.mShowLineNumbers = value;
                    this.RaisePropertyChanged("ShowLineNumbers");
                }
            }
        }
        #endregion ShowLineNumbers

        #region TextEditorOptions
        private TextEditorOptions mTextOptions = new TextEditorOptions()
        {
            ConvertTabsToSpaces = false,
            IndentationSize = 2
        };

        //private TextEditorOptions mTextOptions;
        /// <summary>
        /// TextEditor TextOptions
        /// </summary>
        public TextEditorOptions TextOptions
        {
            get
            {
                return this.mTextOptions;
            }
            set
            {
                if (this.mTextOptions != value)
                {
                    this.mTextOptions = value;
                    this.RaisePropertyChanged("TextOptions");
                }
            }
        }
        #endregion TextEditorOptions

        // Helpers
        #region ContentId

        private string _contentId = null;
        /// <summary>
        /// TextEditorContentId
        /// </summary>
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    RaisePropertyChanged("ContentId");
                }
            }
        }

        #endregion
        #endregion

        #region Text Editor Commands
        #region ShowLineNumbers Command
        /// <summary>
        /// Gets the ShowLineNumbers command.
        /// </summary>
        public Command ShowLineNumbersCommand { get; private set; }

        /// <summary>
        /// Method to check whether the ShowLineNumbers command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnShowLineNumbersCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the ShowLineNumbers command is executed.
        /// </summary>
        private void OnShowLineNumbersCommandExecute()
        {
            // If ShowLineNumbers == false then true 
            ShowLineNumbers = !ShowLineNumbers;
        }
        #endregion

        #region WordWrap Command
        /// <summary>
        /// Gets the WordWrap command.
        /// </summary>
        public Command WordWrapCommand { get; private set; }

        /// <summary>
        /// Method to check whether the WordWrap command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnWordWrapCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the WordWrap command is executed.
        /// </summary>
        private void OnWordWrapCommandExecute()
        {
            // Check if WordWrap is false then set to true
            WordWrap = !WordWrap;
        }
        #endregion

        #region EndLine Command
        /// <summary>
        /// Gets the EndLine command.
        /// </summary>
        public Command EndLineCommand { get; private set; }

        /// <summary>
        /// Method to check whether the EndLineCommand command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnEndLineCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the EndLineCommand command is executed.
        /// </summary>
        private void OnEndLineCommandExecute()
        {
            // TODO: Handle command logic here
            this.TextOptions.ShowEndOfLine = true;
        }
        #endregion

        #region ShowSpaces Command
        /// <summary>
        /// Gets the EndLine command.
        /// </summary>
        public Command ShowSpacesCommand { get; private set; }

        /// <summary>
        /// Method to check whether the EndLineCommand command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnShowSpacesCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the EndLineCommand command is executed.
        /// </summary>
        private void OnShowSpacesCommandExecute()
        {
            // Check if ShowSpaces is false else true
            this.TextOptions.ShowSpaces = !this.TextOptions.ShowSpaces;
        }
        #endregion

        #region ShowTab Command
        /// <summary>
        /// Gets the EndLine command.
        /// </summary>
        public Command ShowTabCommand { get; private set; }

        /// <summary>
        /// Method to check whether the EndLineCommand command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnShowTabCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the EndLineCommand command is executed.
        /// </summary>
        private void OnShowTabCommandExecute()
        {
            // Check if ShowSpaces is false else true
            TextOptions.ShowTabs = !TextOptions.ShowTabs;
        }
        #endregion

        #region Close Document Command
        /// <summary>
        /// Gets the Close command.
        /// </summary>
        public Command CloseDocument { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnCloseCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnCloseDocumentExecute()
        {
            if (this.IsDirty)
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", this.FileName), "TextEditor App", MessageBoxButton.YesNoCancel);

                if (res == MessageBoxResult.Cancel)
                    return;
                else if (res == MessageBoxResult.Yes)
                    Save(this);
            }
            _textEditorModule.Close(this);
            _orchestraService.CloseDocument(this);
        }

        #endregion

        #region Save Document Command
        /// <summary>
        /// Gets the Close command.
        /// </summary>
        public Command SaveCommand { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnSaveCommandCanExecute()
        {
            return this.IsDirty;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnSaveCommandExecute()
        {
            Save(this, false);
        }

        internal void Save(TextEditorViewModel fileToSave, bool saveAsFlag = false)
        {
            //_textEditorModule.Save(this);

            if (string.IsNullOrEmpty(fileToSave.FilePath) || saveAsFlag)
            {
                var dlg = new SaveFileDialog();
                if (dlg.ShowDialog().GetValueOrDefault())
                    fileToSave.FilePath = dlg.FileName;
                //fileToSave.FilePath = dlg.SafeFileName;
            }

            File.WriteAllText(fileToSave.FilePath, fileToSave.Document.Text);

            this.IsDirty = false;

            this.Title = FileName;
        }

        #endregion

        #region SaveAs Document Command
        /// <summary>
        /// Gets the Close command.
        /// </summary>
        public Command SaveAsCommand { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnSaveAsCommandCanExecute()
        {
            return this.IsDirty;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnSaveAsCommandExecute()
        {
            //bool saveAsFlag = true;
            _textEditorModule.Save(this, true);
        }

        #endregion

        #region Update Document Command
        /// <summary>
        /// Gets the Update command.
        /// </summary>
        public Command UpdateCommand { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnUpdateCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnUpdateCommandExecute()
        {
            //bool saveAsFlag = true;
            //_textEditorModule.Save(this, true);
            UpdateContextSensitiveData();
        }

        #endregion
        #endregion

        #region Properties

        #region Url property
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>        
        public string Url
        {
            get { return GetValue<string>(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        /// <summary>
        /// Url property data.
        /// </summary>
        public static readonly PropertyData UrlProperty = RegisterProperty("Url", typeof(string), null, (s, e) => ((TextEditorViewModel)(s)).OnUrlChanged(e));

        /// <summary>
        /// Called when the Url has changed.
        /// </summary>
        /// <param name="e">The <see cref="AdvancedPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnUrlChanged(AdvancedPropertyChangedEventArgs e)
        {
            UpdateContextSensitiveData();
        }
        #endregion

        /// <summary>
        /// Gets the name of the URL changed message.
        /// </summary>
        /// <value>The name of the URL changed message.</value>
        public string UrlChangedMessageTag { get { return string.Format("{0}_{1}", TextEditorModule.Name, UniqueIdentifier); } }

        /// <summary>
        /// Gets the recent sites.
        /// </summary>
        /// <value>
        /// The recent sites.
        /// </value>
        //public string[] SyntaxHighlighting { get { return new[] { "Orchestra", "Catel" }; } }
        public string[] SyntaxHighlighting { get { return new[] { "XML", "C#", "C++", "PHP", "Java" }; } }

        #region SelectedSite property

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
        public static readonly PropertyData SelectedLanugageProperty = RegisterProperty("SelectedSite", typeof(string), null,
            (sender, e) => ((TextEditorViewModel)sender).OnSelectedLanguageChanged());

        /// <summary>
        /// Called when the SelectedSite value changed.
        /// </summary>
        private void OnSelectedLanguageChanged()
        {
            switch (SelectedLanguage)
            {
                case "XML":
                    Url = "http://www.github.com/Orcomp/Orchestra";
                    break;

                case "C#":
                    Url = "http://www.catelproject.com";
                    break;

                default:
                    return;
            }

            //_this.OnBrowseExecute();
        }
        #endregion

        #endregion


        /// <summary>
        /// Method is called when the active view changes within the orchestra application
        /// </summary>
        public void ViewModelActivated()
        {
            //_textEditorModule.ActiveDocument = this;

            UpdateContextSensitiveData();
        }


        /// <summary>
        /// Saves the data.
        /// Called when the view model is Closing via x button
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        protected override bool Save()
        {
            if (_messageService.Show("Are you sure you want to close this window?", button: MessageButton.YesNo) == MessageResult.No)
            {
                return false;
            }

            // Remove this file from Collection
            _textEditorModule.Close(this);
            return true;
        }


        /// <summary>
        /// Update the context sensitive data, related to this view.
        /// </summary>
        private void UpdateContextSensitiveData()
        {
            if (_propertiesViewModel == null)
            {
                _propertiesViewModel = _contextualViewModelManager.GetViewModelForContextSensitiveView<PropertiesViewModel>();
            }
            else
            {
                _propertiesViewModel.Url = Url;

                MethodsCollection();

                if (_propertiesViewModel.MethodSignatureCollection != null)
                {
                    _propertiesViewModel.MethodSignatureCollection.Clear();
                }

                _propertiesViewModel.MethodSignatureCollection = MethodsCollection();
                _propertiesViewModel.currentFileName = FileName;
            }
        }

        #region Document map
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

            string regextPattern2 = @"^.*\b(namespace|private|public|sealed|protected|virtual|internal)\b.*$";

            try
            {
                r = new Regex(regextPattern2, TheOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error in the regular expression!\n\n"
                    + ex.Message + "\n", "orchestra TextEditor Error");
            }

            for (int i = 1; i < this._document.LineCount; i++)
            {
                DocumentLine line = this._document.GetLineByNumber(i);

                m = r.Match(this._document.GetText(line));
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