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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Catel.IoC;

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
            Log.Info("Show Hide Line Numbers");
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
            Log.Info("Show Hide Word Wrap");
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
            Log.Info("Show End Of Line");
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
            Log.Info("Show Spaces");
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
            Log.Info("Show Tabs");

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
            if (IsDirty)
            {
                if (_messageService.Show(string.Format("Save changes for file '{0}'?", this.FileName), "Are you sure?", MessageButton.YesNo) == MessageResult.Yes)
                {
                    _textEditorModule.Save(this);
                    Log.Info(string.Format("Current file {0} is saved!", FileName));

                }
                else
                {
                    return;
                }     
            }
            _textEditorModule.Close(this);
            _orchestraService.CloseDocument(this);
            Log.Info(string.Format("Current file {0} is closed!", FileName));

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
            return IsDirty;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnSaveCommandExecute()
        {
            _textEditorModule.Save(this, false);

            IsDirty = false;
            //Title = FileName;
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
            return true;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnSaveAsCommandExecute()
        {
            //bool saveAsFlag = true;
            _textEditorModule.Save(this, true);

            IsDirty = false;
            //Title = FileName;
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

        #region Run ScriptCS Command
        /// <summary>
        /// Gets the Update command.
        /// </summary>
        public Command ScriptCSCommand { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnScriptCSCommandCanExecute()
        {
            bool result = (Path.GetExtension(_filePath) == ".csx") ? true : false;

            return result;
        }

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnScriptCSCommandExecute()
        {
            //  /C      Carries out the command specified by string and then terminates
            //  /K      Carries out the command specified by string but remains

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = Path.GetDirectoryName(_filePath);
            //startInfo.WorkingDirectory = @"K:\SCRIPTCS\webapi";
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            startInfo.Arguments = "/K scriptcs " + Title.TrimEnd('*');

            process.StartInfo = startInfo;

            process.Start();

            process.WaitForExit();

            //var startInfo = new ProcessStartInfo
            //{
            //    FileName = "server.csx",
            //    WorkingDirectory = @"K:\SCRIPTCS\webapi",
            //    //Arguments = "scriptcs"
            //    //Arguments = "/c scriptcs server.csx"
            //    //RedirectStandardInput = true,
            //    //RedirectStandardOutput = true,
            //    //UseShellExecute = false,
            //    //CreateNoWindow = true
            //};

            //var process = new Process { StartInfo = startInfo };

            ////process.Start();
            ////process.StandardInput.WriteLine("scriptcs server.csx");
            ////process.StandardInput.WriteLine(@"dir>c:\results2.txt");
            ////process.StandardInput.WriteLine("exit");
        }

        #endregion

        #region Open Document Map Dialog Command
        /// <summary>
        /// Gets the Update command.
        /// </summary>
        public Command DocumentMapOpenCommand { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnDocumentMapOpenCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnDocumentMapOpenExecute()
        {
            var viewModel = new DocumentMapViewModel(regextPattern);

            var uiVisualizerService = Catel.IoC.ServiceLocator.Default.ResolveType<IUIVisualizerService>();
            uiVisualizerService.ShowDialog(viewModel);

            Log.Info("Show Map Window");

            //bool saveAsFlag = true;
            //_textEditorModule.Save(this, true);
            UpdateContextSensitiveData();
        }

        #endregion

    }
}
