using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CTecUtil.IO;
using System.Reflection;
using System.Linq;

namespace Xfp.UI.Views
{
    public partial class ViewRevisionHistory : Window
    {
        public ViewRevisionHistory()
        {
            InitializeComponent();
            
            var owner = Application.Current.MainWindow;
            this.Left = owner.Left + owner.ActualWidth  / 2 - 200;
            this.Top  = owner.Top  + 45;

            var name = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("XFP Revision History.docx"));
            fdr.Document = DocxToFlowDocumentConverter.ReadDocxResource(name);
        }

        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e) => DragMove();
        private void CloseRevisionHistory_Click(object sender, EventArgs e) => Close();

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
