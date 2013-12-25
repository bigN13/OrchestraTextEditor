using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Orchestra.Modules.TextEditor.Intellisense
{
    /// <summary>
    /// ICompletionWindowResolver
    /// </summary>
	public interface ICompletionWindowResolver
	{
        /// <summary>
        /// Resolve
        /// </summary>
        /// <returns></returns>
		CompletionWindow Resolve();
	}
}