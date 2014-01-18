using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Orchestra.Modules.TextEditor.Models
{
    /// <summary>
    /// Match Document Item, used for parsing documents
    /// </summary>
    public class MatchItem
    {
        /// <summary>
        /// The line of detected match
        /// </summary>
        public int CurrentLine { get; set; }

        /// <summary>
        /// The actual detected match
        /// </summary>
        public Match CurrentMatch { get; set; }
    }

}
