using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Orchestra.Modules.TextEditor.Intellisense
{
    /// <summary>
    /// TextCompletionData
    /// </summary>
	public class TextCompletionData : ICompletionData
	{
		private readonly double _priority;
		private readonly string _header;
		private readonly string _text;
        
        /// <summary>
        /// TextCompletionData
        /// </summary>
        /// <param name="header"></param>
        /// <param name="text"></param>
        /// <param name="priority"></param>
		public TextCompletionData(string header, string text, double priority = 0)
		{
			_header = header;
			_text = text;
			_priority = priority;
		}

        /// <summary>
        /// Image
        /// </summary>
		public ImageSource Image
		{
			get { return null; }
		}

        /// <summary>
        /// Text
        /// </summary>
		public string Text
		{
			get { return _text; }
		}

        /// <summary>
        /// Content
        /// </summary>
		public object Content
		{
			get { return _header; }
		}

        /// <summary>
        /// Description
        /// </summary>
		public object Description
		{
			get { return Text; }
		}

        /// <summary>
        /// Priority
        /// </summary>
		public double Priority
		{
			get { return _priority; }
		}

        /// <summary>
        /// Complete
        /// </summary>
        /// <param name="textArea"></param>
        /// <param name="completionSegment"></param>
        /// <param name="insertionRequestEventArgs"></param>
		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, Text);
		}
	}
}