using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CTecUtil.Utils;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class Comments : Page
    {
        public Comments()
        {
            InitializeComponent();
            DataContext = _context = new CommentsViewModel(this);

            // Attach the DataObject.Pasting event so we can react to clipboard paste operations
            DataObject.AddPastingHandler(comments, new DataObjectPastingEventHandler(onPaste));
        }


        private CommentsViewModel _context;

        private void onPaste(object sender, DataObjectPastingEventArgs e)
        {
            // If the paste data is text, refresh the view model after paste
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (isText)
                _context.RefreshView();
        }


        private void comments_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_context.CheckChangesAreAllowed?.Invoke() ?? true)
            {
                e.Handled = true;
                return;
            }

            // Handle paste explicitly so we can normalize text and update
            // bindings, while still enforcing CheckChangesAreAllowed
            if (e.Command == ApplicationCommands.Paste)
            {
                if (!(sender is TextBox tb) || !Clipboard.ContainsText())
                {
                    e.Handled = true;
                    return;
                }

                string text = Clipboard.GetText(TextDataFormat.UnicodeText) ?? string.Empty;

                // Normalize line endings to the environment default.
                text = text.Replace("\r\n", "\n").Replace("\n", System.Environment.NewLine);

                // Insert text at the current selection (preserves undo stack)
                int selStart = tb.SelectionStart;
                tb.SelectedText = text;
                tb.SelectionStart = selStart + text.Length;
                tb.SelectionLength = 0;

                // Push change back to view model
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                _context.RefreshView();

                e.Handled = true;
            }
            else
            {
                _context.RefreshView();
            }
        }

        private void comments_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!_context.CheckChangesAreAllowed?.Invoke() ?? true)
                e.Handled = true;

            if (TextUtil.KeyIsSafeEditKey(e.Key))
                e.Handled = true;
        }

        private void comments_PreviewKeyUp(object sender, KeyEventArgs e) => _context.Comments = (sender as TextBox).Text;
    }
}
