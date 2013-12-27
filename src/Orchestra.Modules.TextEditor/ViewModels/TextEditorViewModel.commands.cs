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
                
                if (_messageService.Show(string.Format("Save changes for file '{0}'?", this.FileName), "Are you sure?", MessageButton.YesNo) == MessageResult.Yes)
                {
                    Save(this);
                }
                else
                {
                    return;
                }     
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
    }
}
