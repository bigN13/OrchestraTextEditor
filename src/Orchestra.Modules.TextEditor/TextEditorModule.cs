// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextEditorModule.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2012 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchestra.Modules.TextEditor
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Catel;
    using Catel.IoC;
    using Catel.Linq;
    using Catel.MVVM;
    using Models;
    using Services;
    using ViewModels;
    using Views;
    using System.Windows.Input;
    using Microsoft.Win32;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.IO;
    using Catel.MVVM.Services;
    using Orchestra.Properties;
    
    /// <summary>
    /// Browser module.
    /// </summary>
    public class TextEditorModule : ModuleBase
    {
        #region Variables - Properties - Commands
        /// <summary>
        /// The module name.
        /// </summary>
        public const string Name = "TextEditor";
        private IOrchestraService orchestraService;

        private readonly IUIVisualizerService _uiVisualizerService;


        ObservableCollection<TextEditorViewModel> _files = new ObservableCollection<TextEditorViewModel>();
        ReadOnlyObservableCollection<TextEditorViewModel> _readonyFiles = null;

        /// <summary>
        /// Collection of all files
        /// </summary>
        public ReadOnlyObservableCollection<TextEditorViewModel> Files
        {
            get
            {
                if (_readonyFiles == null)
                    _readonyFiles = new ReadOnlyObservableCollection<TextEditorViewModel>(_files);

                return _readonyFiles;
            }
        }

        /// <summary>
        /// Command - Create new Document
        /// </summary>
        public Command NewDocumentCommand { get; private set; }
        
        /// <summary>
        /// Command - Open Existent Document
        /// </summary>
        public Command OpenDocumentCommand { get; private set; }

        /// <summary>
        /// Show Document Map
        /// </summary>
        public Command ShowDocumentMapCommand { get; private set; }

        /// <summary>
        /// Show Document Map
        /// </summary>
        public Command CloseDocumentCommand { get; private set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditorModule"/> class. 
        /// </summary>
        public TextEditorModule(IUIVisualizerService uiVisualizerService)
            : base(Name)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;
        }

        /// <summary>
        /// Called when the module has been initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            NewDocumentCommand = new Command(NewDocumentCommandExecute, NewDocumentCommandCanExecute);
            OpenDocumentCommand = new Command(OpenDocumentCommandExecute, OpenDocumentCommandCanExecute);
            ShowDocumentMapCommand = new Command(ShowDocumentMapCommandExecute, ShowDocumentMapCommandCanExecute);
            CloseDocumentCommand = new Command(CloseDocumentCommandCanExecute);
        }

        /// <summary>
        /// Initializes the ribbon.
        /// <para />
        /// Use this method to hook up views to ribbon items.
        /// </summary>
        /// <param name="ribbonService">The ribbon service.</param>
        protected override void InitializeRibbon(IRibbonService ribbonService)
        {
            LoadResourceDictionary();

            orchestraService = GetService<IOrchestraService>();

            // Module specific
            var typeFactory = TypeFactory.Default;
            //ribbonService.RegisterRibbonItem(new RibbonButton(HomeRibbonTabName, ModuleName, "Open", new Command(() => NewDocumentCommand.Execute(null))) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_WordWrap32.png" });
            ribbonService.RegisterRibbonItem(new RibbonButton(OrchestraResources.HomeRibbonTabName, ModuleName, "Open", new Command(() =>
            {
                var textEditorViewModel = typeFactory.CreateInstance<TextEditorViewModel>();
                orchestraService.ShowDocument(textEditorViewModel);
            })) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_WordWrap32.png" });

            // View specific
            #region File Buttons

            // View specific
            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
               new RibbonButton(Name, "File", "New", new Command(() => NewDocumentCommand.Execute(null))) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/File_New32.png" },
               ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
               new RibbonButton(Name, "File", "Open", new Command(() => OpenDocumentCommand.Execute(null))) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/File_Open32.png" },
               ModuleName);

            //ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            //   new RibbonButton(Name, "File", "Open", "OpenDocumentCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/File_Open32.png" },
            //   ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
                 new RibbonButton(Name, "File", "Save", "SaveCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/File_Save32.png" },
                 ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
                new RibbonButton(Name, "File", "SaveAs", "SaveAsCommand") 
                { 
                    ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/File_SaveAs32.png" 
                },
                ModuleName);

            //ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            //  new RibbonButton(Name, "File", "CloseMe", new Command(() => CloseDocumentCommand.Execute(null)))
            //  {
            //      ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/action_close.png",
            //      ToolTip = new RibbonToolTip { Title = "Close (Ctrl+X)", Text = "Closes the file." }
            //  },
              //ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
                new RibbonButton(Name, "File", "CloseMe", "CloseDocument")
                {
                    ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/action_close.png",
                    ToolTip = new RibbonToolTip { Title = "Close (Ctrl+X)", Text = "Closes the file." }
                },
                ModuleName);
            #endregion

            #region Edit Buttons

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
             new RibbonButton(Name, "Edit", "Copy", ApplicationCommands.Copy) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Copy32.png" },
             ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
             new RibbonButton(Name, "Edit", "Cut", ApplicationCommands.Cut) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Cut32.png" },
             ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            new RibbonButton(Name, "Edit", "Paste", ApplicationCommands.Paste) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Paste32.png" },
            ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            new RibbonButton(Name, "Edit", "Delete", ApplicationCommands.Delete) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Delete32.png" },
            ModuleName);

            #endregion

            #region Undo / Redo Buttons

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
              new RibbonButton(Name, "Undo", "Undo", ApplicationCommands.Undo) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Undo32.png" },
              ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            new RibbonButton(Name, "Undo", "Redo", ApplicationCommands.Redo) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Redo32.png" },
            ModuleName);

            #endregion

            #region Text Editor Buttons
            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
               new RibbonButton(Name, "Text Editor", "WordWrap", "WordWrapCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_WordWrap32.png" },
               ModuleName);

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
             new RibbonButton(Name, "Text Editor", "LineNumbers", "ShowLineNumbersCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_Numbers32.png" },
             ModuleName);

            //ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            // new RibbonButton(Name, "Text Editor", "EndLine", "EndLineCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_EndLine32.png" },
            // ModuleName);

            //ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            //new RibbonButton(Name, "Text Editor", "ShowSpaces", "ShowSpacesCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/ShowSpaces32.png" },
            //ModuleName);

            //ribbonService.RegisterContextualRibbonItem<TextEditorView>(
            //new RibbonButton(Name, "Text Editor", "ShowTab", "ShowTabCommand") { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/ShowTab32.png" },
            //ModuleName);

            #endregion

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(
             new RibbonButton(Name, "Document", "Map", new Command(() => ShowDocumentMapCommand.Execute(null))) { ItemImage = "/Orchestra.Modules.TextEditor;component/Resources/Images/App/Edit_WordWrap32.png" },
             ModuleName);

            #region TextEditor Module
          
            ribbonService.RegisterContextualRibbonItem<TextEditorView>(new RibbonComboBox(Name, "Languages")
            {
                ItemsSource = "SyntaxHighlighting",
                SelectedItem = "SelectedLanguage",
                Layout = new RibbonItemLayout { Width = 100 },
                Style = Application.Current.Resources["SelectedSitesComboBoxStyle"] as Style
            }, ModuleName);

            // Find the template to show as dynamic content. TODO: Refactor, make more elegant.
            var template = Application.Current.Resources["TestTemplate"] as DataTemplate;

            ribbonService.RegisterContextualRibbonItem<TextEditorView>(new RibbonContentControl(Name, "Dynamic content") { ContentTemplate = template, Layout = new RibbonItemLayout { Width = 120 } }, ModuleName);

            ribbonService.RegisterRibbonItem(new RibbonButton(OrchestraResources.ViewRibbonTabName, ModuleName, "TextEditor properties", new Command(() =>
            {
                orchestraService.ShowDocumentIfHidden<PropertiesViewModel>();
            })) { ItemImage = "/rchestra.Modules.TextEditor;component/Resources/Images/App/Edit_WordWrap32.png" });
           
            #endregion

            var dockingSettings = new DockingSettings();
            dockingSettings.DockLocation = DockLocation.Right;
            dockingSettings.Width = 225;

            // Demo: register contextual view related to browserview
            var contextualViewModelManager = GetService<IContextualViewModelManager>();
            contextualViewModelManager.RegisterContextualView<TextEditorViewModel, PropertiesViewModel>("Document Map", DockLocation.Right);

            // Open blank document during application start
            var currenttextEditorViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<TextEditorViewModel>("Orchestra", this);
            //orchestraViewModel.Url = "http://www.github.com/Orcomp/Orchestra";
            orchestraService.ShowDocument(currenttextEditorViewModel, "New Document");
        }
        
        #endregion

        // Commmands
        #region New Document Command
        /// <summary>
        /// Method to check whether the New command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool NewDocumentCommandCanExecute()
        {
            // TODO: Handle command logic here
            return true;
        }

        /// <summary>
        /// Method to invoke when the New command is executed.
        /// </summary>
        private void NewDocumentCommandExecute()
        {
            var typeFactory = TypeFactory.Default;
            var orchestraViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<TextEditorViewModel>(this);
            _files.Add(orchestraViewModel);
            orchestraService.ShowDocument(orchestraViewModel);
        }
        #endregion

        #region Open Document Command
        /// <summary>
        /// Method to check whether the New command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OpenDocumentCommandCanExecute()
        {
            // TODO: Handle command logic here
            return true;
        }

        /// <summary>
        /// Method to invoke when the New command is executed.
        /// </summary>
        private void OpenDocumentCommandExecute()
        {
            // TODO: Handle command logic here
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                var fileViewModel = Open(dlg.FileName);
                _files.Add(fileViewModel);
                ActiveDocument = fileViewModel;
            }
        }

        /// <summary>
        /// Open File Window
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public TextEditorViewModel Open(string filepath)
        {
             //Verify whether file is already open in editor, and if so, show it
            var orcaViewModel = _files.FirstOrDefault(fm => fm.FilePath == filepath);
            if (orcaViewModel != null)
            {
                MessageBox.Show(string.Format("The file '{0}' is already Opened", orcaViewModel.FileName), "TextEditor App", MessageBoxButton.OK);
                return orcaViewModel;
            }

            var typeFactory = TypeFactory.Default;
            orcaViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<TextEditorViewModel>("Noname", this);
            orcaViewModel.FilePath = filepath;
            orchestraService.ShowDocument(orcaViewModel);

            return orcaViewModel;
        }
        #endregion

        #region Show Document Map Command
        /// <summary>
        /// Method to check whether the New command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool ShowDocumentMapCommandCanExecute()
        {
            // TODO: Handle command logic here
            return true;
        }

        /// <summary>
        /// Method to invoke when the New command is executed.
        /// </summary>
        private void ShowDocumentMapCommandExecute()
        {
            //var viewModel = new MapViewModel();
            var viewModel = new DocumentMapViewModel();
            _uiVisualizerService.ShowDialog(viewModel);
        }

       
        #endregion

        #region Close Document Command
        ///// <summary>
        ///// Method to check whether the New command can be executed.
        ///// </summary>
        ///// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>  
        //private bool CloseDocumentCommandExecute()
        //{
        //    // TODO: Handle command logic here
        //    return true;
        //}

        /// <summary>
        /// Method to invoke when the New command is executed.
        /// </summary>
        private void CloseDocumentCommandCanExecute()
        {
            this.Close(ActiveDocument);

        }
   
        #endregion


        #region Internal Close
        internal void Close(TextEditorViewModel fileToClose)
        {
            //if (fileToClose.IsDirty)
            //{
            //    var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "TextEditor App", MessageBoxButton.YesNoCancel);
            //    if (res == MessageBoxResult.Cancel)
            //        return;
            //    if (res == MessageBoxResult.Yes)
            //    {
            //        Save(fileToClose);
            //    }
            //}
            
            //Removes the file from the collection of Documents
            _files.Remove(fileToClose);
        } 
        #endregion

        #region Internal Save
        internal void Save(TextEditorViewModel fileToSave, bool saveAsFlag = false)
        {
            if (fileToSave.FilePath == null || saveAsFlag)
            {
                var dlg = new SaveFileDialog();
                if (dlg.ShowDialog().GetValueOrDefault())
                    fileToSave.FilePath = dlg.FileName;
                //fileToSave.FilePath = dlg.SafeFileName;
            }

            File.WriteAllText(fileToSave.FilePath, fileToSave.Document.Text);
            //ActiveDocument.IsDirty = false;
        } 
        #endregion


        #region ActiveDocument

        private TextEditorViewModel _activeDocument = null;
        /// <summary>
        /// Set Active Document
        /// </summary>
        public TextEditorViewModel ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if (_activeDocument != value)
                {
                    _activeDocument = value;
                    //RaisePropertyChanged("ActiveDocument");
                    //if (ActiveDocumentChanged != null)
                    //    ActiveDocumentChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Helpers
        private void LoadResourceDictionary()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Orchestra.Modules.TextEditor;component/ResourceDictionary.xaml", UriKind.RelativeOrAbsolute) });
        }  
        #endregion       
    }
}