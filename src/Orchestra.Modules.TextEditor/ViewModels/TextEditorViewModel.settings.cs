using System;
using System.Collections.Generic;
using Catel;
using Catel.Data;
using Catel.MVVM;
using Catel.MVVM.Services;
using Catel.Messaging;
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


namespace Orchestra.Modules.TextEditor.ViewModels
{
    public partial class TextEditorViewModel : Orchestra.ViewModels.ViewModelBase, IContextualViewModel
    {
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
                _filePath = OnFilePathChanged(value); 
            }
        }

        private string OnFilePathChanged(string value)
        {
            if (_filePath != value)
            {
                _filePath = value;
                RaisePropertyChanged("FilePath");
                RaisePropertyChanged("FileName");
                RaisePropertyChanged("Title");

                if (File.Exists(_filePath))
                {
                    //this._document = new TextDocument();
                    this.Document = new TextDocument();
                    this.HighlightDef = HighlightingManager.Instance.GetDefinition("C#");
                    this.IsDirty = false;
                    this.IsReadOnly = false;
                    this.ShowLineNumbers = true;
                    this.WordWrap = false;

                    // Check file attributes and set to read-only if file attributes indicate that
                    if ((System.IO.File.GetAttributes(_filePath) & FileAttributes.ReadOnly) != 0)
                    {
                        this.IsReadOnly = true;
                        //this.IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                        //                        "Change the file access permissions or save the file in a different location if you want to edit it.";
                    }

                    using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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

            return _filePath;
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
                    return "Noname" + (IsDirty ? "*" : string.Empty);

                Title = System.IO.Path.GetFileName(FilePath) + (IsDirty ? "*" : string.Empty);
                return Title;
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
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;
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
        private bool wordWrap = false;

        /// <summary>
        /// TextEditor Word Wrap Option
        /// </summary>
        public bool WordWrap
        {
            get
            {
                return wordWrap;
            }

            set
            {
                if (wordWrap != value)
                {
                    wordWrap = value;
                    RaisePropertyChanged("WordWrap");
                }
            }
        }
        #endregion WordWrap

        #region ShowLineNumbers
        // Toggle state ShowLineNumbers
        private bool showLineNumbers = false;

        /// <summary>
        /// TextEditor Show Line Numbee Option
        /// </summary>
        public bool ShowLineNumbers
        {
            get
            {
                return showLineNumbers;
            }

            set
            {
                if (showLineNumbers != value)
                {
                    showLineNumbers = value;
                    RaisePropertyChanged("ShowLineNumbers");
                }
            }
        }
        #endregion ShowLineNumbers

        #region TextEditorOptions
        private TextEditorOptions textOptions = new TextEditorOptions()
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
                return textOptions;
            }
            set
            {
                if (textOptions != value)
                {
                    textOptions = value;
                    RaisePropertyChanged("TextOptions");
                }
            }
        }
        #endregion TextEditorOptions

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
    }
}
