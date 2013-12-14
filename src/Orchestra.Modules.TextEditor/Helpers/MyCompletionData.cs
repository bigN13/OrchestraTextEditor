// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Orchestra.Modules.TextEditor
{
	/// <summary>
	/// Implements AvalonEdit ICompletionData interface to provide the entries in the completion drop down.
	/// </summary>
	public class MyCompletionData : ICompletionData
	{
        /// <summary>
        /// Complete the Text
        /// </summary>
        /// <param name="text"></param>
		public MyCompletionData(string text)
		{
			this.Text = text;
		}
		
        /// <summary>
        /// Image
        /// </summary>
		public System.Windows.Media.ImageSource Image {
			get { return null; }
		}
		
        /// <summary>
        /// Text Property
        /// </summary>
		public string Text { get; private set; }
		
		/// <summary>
        ///  Use this property if you want to show a fancy UIElement in the drop down list.
		/// </summary>
		public object Content {
			get { return this.Text; }
		}
		
        /// <summary>
        /// Description for this test
        /// </summary>
		public object Description {
			get { return "Description for " + this.Text; }
		}
		
        /// <summary>
        /// Priority 
        /// </summary>
		public double Priority { get { return 0; } }
		
        /// <summary>
        /// Complete the textarea
        /// </summary>
        /// <param name="textArea"></param>
        /// <param name="completionSegment"></param>
        /// <param name="insertionRequestEventArgs"></param>
		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, this.Text);
		}
	}
}
