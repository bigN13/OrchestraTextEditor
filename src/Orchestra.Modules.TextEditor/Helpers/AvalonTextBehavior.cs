using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace Orchestra.Modules.TextEditor
{
    /// <summary>
    /// AvalonEditBehaviour
    /// </summary>
    public sealed class AvalonEditBehaviour : Behavior<ICSharpCode.AvalonEdit.TextEditor>
    {
        ///// <summary>
        ///// GiveMeTheTextProperty
        ///// </summary>
        //public static readonly DependencyProperty GiveMeTheTextProperty =
        //    DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour),
        //    new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        ///// <summary>
        ///// GiveMeTheText
        ///// </summary>
        //public string GiveMeTheText
        //{
        //    get { return (string)GetValue(GiveMeTheTextProperty); }
        //    set { SetValue(GiveMeTheTextProperty, value); }
        //}

        ///// <summary>
        ///// OnAttached
        ///// </summary>
        //protected override void OnAttached()
        //{
        //    base.OnAttached();
        //    if (AssociatedObject != null)
        //        AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        //}

        ///// <summary>
        ///// OnDetaching
        ///// </summary>
        //protected override void OnDetaching()
        //{
        //    base.OnDetaching();
        //    if (AssociatedObject != null)
        //        AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        //}

        //private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        //{
        //    var textEditor = sender as ICSharpCode.AvalonEdit.TextEditor;
        //    if (textEditor != null)
        //    {
        //        if (textEditor.Document != null)
        //            GiveMeTheText = textEditor.Document.Text;
        //    }
        //}

        //private static void PropertyChangedCallback(
        //    DependencyObject dependencyObject,
        //    DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    if (AssociatedObject != null)
        //    {
        //        var editor = AssociatedObject as ICSharpCode.AvalonEdit.TextEditor;
        //        if (editor.Document != null)
        //        {
        //            editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
        //        }
        //    }
        //}
    }
}
