using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Orchestra.Modules.TextEditor
{
    /// <summary>
    /// Custom LineColorizer
    /// </summary>
    class LineColorizer : DocumentColorizingTransformer
    {
        int lineNumber;


        public LineColorizer(int lineNumber)
        {
            if (lineNumber < 1)
                throw new ArgumentOutOfRangeException("lineNumber", lineNumber, "Line numbers are 1-based.");
            this.lineNumber = lineNumber;
        }

        public int LineNumber
        {
            get { return lineNumber; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", value, "Line numbers are 1-based.");
                lineNumber = value;
            }
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {


            if (!line.IsDeleted && line.LineNumber == lineNumber)
            {
                int start = line.Offset;
                int end = line.EndOffset;

                ChangeLinePart(start, end, ApplyChanges);
            }
        }

        void ApplyChanges(VisualLineElement element)
        {
            // apply changes here
            element.TextRunProperties.SetBackgroundBrush(Brushes.Silver);

            // This lambda gets called once for every VisualLineElement
            // between the specified offsets.
            Typeface tf = element.TextRunProperties.Typeface;
            // Replace the typeface with a modified version of
            // the same typeface
            element.TextRunProperties.SetTypeface(new Typeface(
                tf.FontFamily,
                FontStyles.Italic,
                FontWeights.Bold,
                tf.Stretch
            ));
        }
    }
}
