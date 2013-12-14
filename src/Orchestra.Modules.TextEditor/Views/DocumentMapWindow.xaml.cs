namespace Orchestra.Modules.TextEditor.Views
{
    using Catel.Windows;

    using ViewModels;

    /// <summary>
    /// Interaction logic for PropertiesView.xaml.
    /// </summary>
    public partial class DocumentMapWindow : DataWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapWindow"/> class.
        /// </summary>
        public DocumentMapWindow()
            : base(DataWindowMode.OkCancel, null, DataWindowDefaultButton.OK, true, InfoBarMessageControlGenerationMode.Inline)
        {
            InitializeComponent();
        }
    }
}
