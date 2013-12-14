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

	/// <summary>
	/// Interaction logic for BrowserView.xaml.
	/// </summary>
	public partial class TextEditorView : DocumentView
	{
		#region Constants
		private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 7.1; Trident/5.0)";
		#endregion

		#region Fields
		int prevHighlightedLine = 0;

		List<LineColorizer> ColorizerCollection;
	
		CompletionWindow completionWindow;		
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

			ColorizerCollection = new List<LineColorizer>();

			#region Folding Init
            //HighlightingComboBox_SelectionChanged();
            //intialFolding();
            //foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy = new BraceFoldingStrategy();

            //FoldingManager foldingManager = FoldingManager.Install(textEditor.TextArea);
            //BraceFoldingStrategy foldingStrategy = new BraceFoldingStrategy();
            //foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);

            //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            //textEditor.SyntaxHighlighting = customHighlighting;
            // initial highlighting now set by XAML

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
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
				if (!string.IsNullOrWhiteSpace(vm.Url))
				{
					OnBrowse(vm.Url);
				}

				// Register to the message sent from Document Map
				var messageMediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
				messageMediator.Register<MatchItem>(this, OnParse, vm.FileName);
                
               

				//var messageMediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
				//messageMediator.Register<string>(this, OnBrowse, vm.UrlChangedMessageTag);


			}
		}

		private void OnBrowse(string url)
		{
			//webBrowser.Navigate(url, null, null, string.Format("User-Agent: {0}", UserAgent));
		}


 
		private void OnParse(MatchItem SelectedItem)
		{

			if (SelectedItem != null)
			{
				MatchItem m = SelectedItem;

				textEditor.ScrollTo(m.currentLine, 0);

				if (ColorizerCollection.Count > 0 && textEditor.Document.LineCount > 1)
				{
					textEditor.TextArea.TextView.LineTransformers.Remove(ColorizerCollection[0]);
					IHighlighter documentHighlighter = textEditor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
					HighlightedLine result = documentHighlighter.HighlightLine(textEditor.Document.GetLineByNumber(prevHighlightedLine).LineNumber);
					
					textEditor.TextArea.TextView.Redraw(result.DocumentLine); // invalidate specific Line
					ColorizerCollection.Clear();
				}


				// Add Colors
				LineColorizer currentHighligtedLine = new LineColorizer(m.currentLine);

				textEditor.TextArea.TextView.LineTransformers.Add(currentHighligtedLine);

				ColorizerCollection.Add(currentHighligtedLine);

				textEditor.TextArea.TextView.Redraw(); // invalidate specific Line

				//Keep track of previous line
				prevHighlightedLine = m.currentLine;
			}
		}
		#endregion


		void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
            ICompletionWindowResolver resolver = new CompletionWindowResolver(textEditor.Text, textEditor.CaretOffset, e.Text, textEditor);
            completionWindow = resolver.Resolve();

            //if (e.Text == ".")
            //{
            //    // Open code completion after the user has pressed dot:
            //    completionWindow = new CompletionWindow(textEditor.TextArea);
            //    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            //    data.Add(new MyCompletionData("Item1"));
            //    data.Add(new MyCompletionData("Item2"));
            //    data.Add(new MyCompletionData("Item3"));
            //    completionWindow.Show();
            //    completionWindow.Closed += delegate
            //    {
            //        completionWindow = null;
            //    };
            //}
		}

		void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Length > 0 && completionWindow != null)
			{
				if (!char.IsLetterOrDigit(e.Text[0]))
				{
					// Whenever a non-letter is typed while the completion window is open,
					// insert the currently selected element.
					completionWindow.CompletionList.RequestInsertion(e);
				}
			}
			// do not set e.Handled=true - we still want to insert the character that was typed
		}

		#region Folding
		FoldingManager foldingManager;
		AbstractFoldingStrategy foldingStrategy;

		//void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		void HighlightingComboBox_SelectionChanged()
		{
			if (textEditor.SyntaxHighlighting == null)
			{
				foldingStrategy = null;
			}
			else
			{
				switch (textEditor.SyntaxHighlighting.Name)
				{
					case "XML":
						foldingStrategy = new XmlFoldingStrategy();
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
						break;
					case "C#":
					case "C++":
					case "PHP":
					case "Java":
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
						foldingStrategy = new BraceFoldingStrategy();
						break;
					default:
					    textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
						foldingStrategy = new BraceFoldingStrategy();
						break;
				}
			}
			if (foldingStrategy != null)
			{
				if (foldingManager == null)
					foldingManager = FoldingManager.Install(textEditor.TextArea);
				foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
			}
			else
			{
				if (foldingManager != null)
				{
					FoldingManager.Uninstall(foldingManager);
					foldingManager = null;
				}
			}
		}

        void intialFolding()
        {
            //foldingStrategy = new XmlFoldingStrategy();

            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                {
                    foldingManager = new FoldingManager(textEditor.Document);

                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                }
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }
		void HighlightingComboBox()
		{
			if (textEditor.SyntaxHighlighting == null)
			{
				foldingStrategy = null;
			}
			else
			{
				switch (textEditor.SyntaxHighlighting.Name)
				{
					case "XML":
						foldingStrategy = new XmlFoldingStrategy();
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
						break;
					case "C#":
					case "C++":
					case "PHP":
					case "Java":
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
						foldingStrategy = new BraceFoldingStrategy();
						break;
					default:
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
						foldingStrategy = null;
						break;
				}
			}
			if (foldingStrategy != null)
			{
                if (foldingManager == null)
                {
                    foldingManager = new FoldingManager(textEditor.Document);

                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                }
				foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
			}
			else
			{
				if (foldingManager != null)
				{
					FoldingManager.Uninstall(foldingManager);
					foldingManager = null;
				}
			}
		}

		void foldingUpdateTimer_Tick(object sender, EventArgs e)
		{
			if (foldingStrategy != null && textEditor.Document != null)
			{
                intialFolding();

				foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
			}
		}
		#endregion
	
	}

 
 
}