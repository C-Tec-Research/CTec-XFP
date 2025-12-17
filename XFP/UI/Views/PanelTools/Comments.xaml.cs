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
        }


        private CommentsViewModel _context;


        private void comments_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TextUtil.KeyIsSafeEditKey(e.Key))
                return;

            if (!_context.CheckChangesAreAllowed?.Invoke() ?? true)
                e.Handled = true;
        }
    }
}
