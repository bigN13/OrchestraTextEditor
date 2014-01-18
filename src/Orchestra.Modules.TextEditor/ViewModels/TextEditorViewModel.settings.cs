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
        /// <summary>
        /// File Path of documnet
        /// </summary>
        public string FilePath { get; set; }

        private void OnFilePathChanged()
        {
            RaisePropertyChanged("FileName");
            RaisePropertyChanged("Title");

            if (File.Exists(FilePath))
            {
                //this._document = new TextDocument();
                Document = new TextDocument();
                HighlightDef = HighlightingManager.Instance.GetDefinition("C#");
                IsDirtyDoc = true;
                IsReadOnly = false;
                ShowLineNumbers = true;
                WordWrap = false;

                // Check file attributes and set to read-only if file attributes indicate that
                if ((System.IO.File.GetAttributes(FilePath) & FileAttributes.ReadOnly) != 0)
                {
                    this.IsReadOnly = true;
                    //this.IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                    //                        "Change the file access permissions or save the file in a different location if you want to edit it.";
                }

                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                    {
                        //this._document = new TextDocument(reader.ReadToEnd());
                        this.Document = new TextDocument(reader.ReadToEnd());
                    }
                }

                //ContentId = FilePath;
            }
        }

        #endregion

        #region IsDirty
        /// <summary>
        /// Check if document is dirty
        /// </summary>
        public bool IsDirtyDoc { get; set; }

        private void OnIsDirtyDocChanged()
        {
            //if(IsDirty != IsDirty)
            //{
            //RaisePropertyChanged("IsDirty");
                RaisePropertyChanged("Title");
                RaisePropertyChanged("FileName");
            //}

        }
        #endregion


        #region FileName
        /// <summary>
        ///  TextEditor - Specify the Name of Sheet
        /// </summary>
        public string FileName { get; set; }

        private void OnFileNameChanged()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                Title = "Noname" + (IsDirtyDoc ? "*" : string.Empty);
                return;
            }
            
            Title = FileName = System.IO.Path.GetFileName(FilePath) + (IsDirtyDoc ? "*" : string.Empty);
        }      
        #endregion FileName

        #region TextContent

        /// <summary>
        /// Current Document
        /// </summary>
        public TextDocument Document { get; set; }

        private void OnDocumentChanged()
        {
            IsDirtyDoc = true;
        }
        #endregion

        #region HighlightingDefinition
        /// <summary>
        /// Define the Highlighting of document
        /// </summary>
        public IHighlightingDefinition HighlightDef { get; set; }

        #endregion

        #region WordWrap
        /// <summary>
        /// Show Word Wrap of document
        /// </summary>
        public bool WordWrap { get; set; }

        #endregion WordWrap

        #region ShowLineNumbers
        /// <summary>
        /// Show Line Numbers
        /// </summary>
        public bool ShowLineNumbers { get; set; }

        #endregion ShowLineNumbers

        #region TextEditorOptions

        /// <summary>
        /// TextEditor TextOptions
        /// </summary>
        public TextEditorOptions TextOptions { get; set; }

        #endregion TextEditorOptions
    }
}
