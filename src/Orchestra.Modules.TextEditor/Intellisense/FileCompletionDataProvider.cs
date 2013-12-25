using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Catel.Logging;

namespace Orchestra.Modules.TextEditor.Intellisense
{
    /// <summary>
    /// FileCompletionDataProvider
    /// </summary>
	public class FileCompletionDataProvider : ICompletionDataProvider
	{
		private static readonly Dictionary<string, IEnumerable<ICompletionData>> Data = new Dictionary<string, IEnumerable<ICompletionData>>();

        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// GetData
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="input"></param>
        /// <param name="highlightingName"></param>
        /// <returns></returns>
		public IEnumerable<ICompletionData> GetData(string text, int position, string input, string highlightingName)
		{

			if (!Data.Keys.Contains(highlightingName))
			{
				var result = GetData(highlightingName);
				Data.Add(highlightingName, result);
			}
			if (string.IsNullOrWhiteSpace(input))
				return Data[highlightingName];
			return new List<ICompletionData>();
		}


		private IEnumerable<ICompletionData> GetData(string highlightingName)
		{
			try
			{
			
			var result = new List<ICompletionData>();
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Intellisense", "Keywords", "options.xml");
			using (var sr = new StreamReader(path))
			{
				var ser = new XmlSerializer(typeof(List<KeywordsFileOption>));
				var ops = (List<KeywordsFileOption>)ser.Deserialize(sr);

                var filePath = Path.Combine(Application.StartupPath, "Modules", "Intellisense", "Keywords",
					ops
					.Where(x => string.Compare(x.HighlightingName, highlightingName) == 0)
					.Select(x => x.Filename)
					.FirstOrDefault() ?? string.Empty);
				if (File.Exists(filePath))
				{
					var w = GetWords(filePath);
					result.AddRange(w);
				}
			}
			return result;
			}
			catch (Exception ex)
			{
                Log.Error("GetData(string highlightingName) " + ex.Message);

				return new List<ICompletionData>();
			}
		}

		private IEnumerable<ICompletionData> GetWords(string filename)
		{
			return File.Exists(filename) ? 
				File.ReadAllLines(filename).Select(x => new TextCompletionData(x, x)).Cast<ICompletionData>().ToList() 
				: new List<ICompletionData>();
		}
	}

    /// <summary>
    /// KeywordsFileOption
    /// </summary>
	public class KeywordsFileOption
	{
        /// <summary>
        /// HighlightingName
        /// </summary>
		[XmlAttribute]
		public string HighlightingName { get; set; }

        /// <summary>
        /// Filename
        /// </summary>
		[XmlAttribute]
		public string Filename { get; set; }
	}
}