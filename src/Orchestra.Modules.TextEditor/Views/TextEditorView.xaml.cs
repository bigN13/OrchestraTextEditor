// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserView.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2013 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orchestra.Modules.TextEditor.Views
{
    using System.Windows.Navigation;
    using Catel.IoC;
    using Catel.Messaging;
    using Orchestra.Modules.TextEditor.ViewModels;
    using Orchestra.Views;
    using Catel.MVVM;
    using ICSharpCode.AvalonEdit.Document;
    using System.Text.RegularExpressions;
    using System.Windows.Threading;
    using System;
    using ICSharpCode.AvalonEdit.Folding;
    using System.Windows.Controls;
    using ICSharpCode.AvalonEdit.Rendering;
    using System.Windows;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Windows.Input;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Orchestra.Modules.TextEditor.Intellisense;
    using Orchestra.Modules.TextEditor.Models;
    using Catel.Logging;

    /// <summary>
    /// Interaction logic for BrowserView.xaml.
    /// </summary>
    public partial class TextEditorView : DocumentView
    {
       
        #region Fields
        int _prevHighlightedLine = 0;

        List<LineColorizer> _colorizerCollection;
    
        CompletionWindow _completionWindow;

        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditorView"/> class.
        /// </summary>
        public TextEditorView()
        {
            #region Highlighting definition
            // Load our custom highlighting definition
            var type = typeof(TextEditorView);
            var fullName = "Orchestra.Modules.TextEditor.Helpers.CustomHighlighting.xshd";

            // Get Names of All Embeded Resources
            //var something = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            IHighlightingDefinition customHighlighting;
            using (Stream s = type.Assembly.GetManifestResourceStream(fullName))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);
            
            #endregion

            InitializeComponent();

            _colorizerCollection = new List<LineColorizer>();

            #region Folding Init

            _foldingStrategy = new BraceFoldingStrategy();

            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += FoldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
            #endregion

        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the view model has changed.
        /// </summary>
        protected override void OnViewModelChanged()
        {
            var vm = ViewModel as TextEditorViewModel;
            if (vm != null)
            {
               
                // Register to the message sent from Document Map
                var messageMediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
                messageMediator.Register<MatchItem>(this, OnParse, vm.UniqueIdentifier);                
            }
        }
 
        private void OnParse(MatchItem SelectedItem)
        {

            if (SelectedItem != null)
            {
                MatchItem m = SelectedItem;

                textEditor.ScrollTo(m.CurrentLine, 0);

                if (_colorizerCollection.Count > 0 && textEditor.Document.LineCount > 1)
                {
                    textEditor.TextArea.TextView.LineTransformers.Remove(_colorizerCollection[0]);
                    IHighlighter documentHighlighter = textEditor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
                    HighlightedLine result = documentHighlighter.HighlightLine(textEditor.Document.GetLineByNumber(_prevHighlightedLine).LineNumber);
                    
                    // invalidate specific Line
                    textEditor.TextArea.TextView.Redraw(result.DocumentLine);
                    _colorizerCollection.Clear();
                }


                // Add Colors
                LineColorizer currentHighligtedLine = new LineColorizer(m.CurrentLine);

                textEditor.TextArea.TextView.LineTransformers.Add(currentHighligtedLine);

                _colorizerCollection.Add(currentHighligtedLine);

                // invalidate specific Line
                textEditor.TextArea.TextView.Redraw(); 

                //Keep track of previous line
                _prevHighlightedLine = m.CurrentLine;
            }
        }
        #endregion


        void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            ICompletionWindowResolver resolver = new CompletionWindowResolver(textEditor.Text, textEditor.CaretOffset, e.Text, textEditor);
            _completionWindow = resolver.Resolve();
        }

        void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding
        FoldingManager _foldingManager;
        AbstractFoldingStrategy _foldingStrategy;

        //void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        void HighlightingComboBox_SelectionChanged()
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                _foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        _foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        _foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        _foldingStrategy = new BraceFoldingStrategy();
                        break;
                }
            }
            if (_foldingStrategy != null)
            {
                if (_foldingManager == null)
                    _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }
        }

        void IntialFolding()
        {
            //foldingStrategy = new XmlFoldingStrategy();

            if (_foldingStrategy != null)
            {
                if (_foldingManager == null)
                {
                    _foldingManager = new FoldingManager(textEditor.Document);

                    _foldingManager = FoldingManager.Install(textEditor.TextArea);
                }
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }
        }
        void HighlightingComboBox()
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                _foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        _foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        _foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        _foldingStrategy = null;
                        break;
                }
            }
            if (_foldingStrategy != null)
            {
                if (_foldingManager == null)
                {
                    _foldingManager = new FoldingManager(textEditor.Document);

                    _foldingManager = FoldingManager.Install(textEditor.TextArea);
                }
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }
        }

        void FoldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_foldingStrategy != null && textEditor.Document != null)
            {
                IntialFolding();

                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
        }
        #endregion
    }
}