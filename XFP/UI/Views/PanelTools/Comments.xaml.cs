using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
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

        private void comments_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
    }
}
