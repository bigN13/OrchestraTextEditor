using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Orchestra.Modules.TextEditor.Intellisense;

namespace Orchestra.Modules.TextEditor.Intellisense
{
    /// <summary>
    /// CompletionWindowResolver
    /// </summary>
	public class CompletionWindowResolver : ICompletionWindowResolver
	{
		private readonly string _text;
		private readonly int _position;
		private readonly string _input;
        private readonly ICSharpCode.AvalonEdit.TextEditor _target;


		private readonly List<ICompletionDataProvider> _dataProviders = new List<ICompletionDataProvider>();

        /// <summary>
        /// CompletionWindowResolver
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="input"></param>+
        /// <param name="textEditor"></param>
        public CompletionWindowResolver(string text, int position, string input, ICSharpCode.AvalonEdit.TextEditor textEditor)
		{
			_text = text;
			_position = position;
			_input = input;
			_target = textEditor;

			_dataProviders.Add(new FileCompletionDataProvider());
		}

        /// <summary>
        /// Resolve
        /// </summary>
        /// <returns></returns>
		public CompletionWindow Resolve()
		{

			var hiName = string.Empty;
			if (_target.SyntaxHighlighting != null)
			{
				hiName = _target.SyntaxHighlighting.Name;
			}

			var cdata = _dataProviders.SelectMany(x => x.GetData(_text, _position, _input, hiName)).ToList();
			int count = cdata.Count;
			if (count > 0)
			{
				var completionWindow = new CompletionWindow(_target.TextArea);

				var data = completionWindow.CompletionList.CompletionData;

				foreach (var completionData in cdata)
				{
					data.Add(completionData);
				}

				completionWindow.Show();
				completionWindow.Closed += delegate
				                           	{
				                           		completionWindow = null;
				                           	};
				return completionWindow;

			}
			return null;
		}
	}
}