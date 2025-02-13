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

            //ReadDocx(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("XFP Revision History.docx")));
            var name = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("XFP Revision History.docx"));
            fdr.Document = DocxToFlowDocumentConverter.ReadDocxResource(name);
        }

        private void ReadDocx(string resourceName)
        {
            try
            {
                //var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName));
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    var flowDocumentConverter = new DocxToFlowDocumentConverter(stream);
                    flowDocumentConverter.Read();
                    fdr.Document = flowDocumentConverter.Document;
                    this.Title = Path.GetFileName(resourceName);
                }
            }
            catch (Exception ex) { }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseRevisionHistory_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
