using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Orchestra.Modules.TextEditor.Intellisense
{
	/// <summary>
	/// ICompletionDataProvider
	/// </summary>
	public interface ICompletionDataProvider
	{
		/// <summary>
		/// GetData
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		/// <param name="input"></param>
		/// <param name="highlightingName"></param>
		/// <returns></returns>
		IEnumerable<ICompletionData> GetData(string text, int position, string input, string highlightingName); 
	}
}