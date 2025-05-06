using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using CTecUtil.Config;
using CTecUtil.UI;

namespace CTecUtil.Printing
{
    public class PrintUtil
    {
        private const string _tempPrintFileSuffix = ".xps";
        private static bool _housekept = false;


        public static string GetTempPrintFilePath(string prefix)
        {
            _temporaryPrintFolder = Path.Combine(ApplicationConfig.AppDataFolder, Path.Combine("Temp", "Print"));
            housekeeping();
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "print";                
            var tempFile   = prefix + "_" + DateTime.Now.ToFileTimeUtc() + _tempPrintFileSuffix;
            Directory.CreateDirectory(_temporaryPrintFolder);
            return Path.Combine(_temporaryPrintFolder, tempFile);
        }


        private static void housekeeping()
        {
            if (_housekept)
                return;

            try
            {
                Directory.CreateDirectory(_temporaryPrintFolder);

                //delete any files older that 1 day
                foreach (var file in new DirectoryInfo(_temporaryPrintFolder).GetFiles())
                    if (file.CreationTime < DateTime.Now.AddDays(-1))
                        file.Delete();
                _housekept = true;
            }
            catch { }
        }


        private static string _temporaryPrintFolder;


        public const string PrintHeaderFont  = "Times New Roman";
        public const string PrintDefaultFont = "Arial";
        public const string PrintFixedFont   = "Lucida Console";

        public const int PrintDocumentHeaderFontSize = 18;
        public const int PrintPageHeaderFontSize     = 14;
        public const int PrintDefaultFontSize        = 9;
        public static readonly Thickness DefaultGridMargin = new Thickness(3, 2, 3, 2);
        
        public static SolidColorBrush GridHeaderBackground         = Styles.Brush08;
        public static SolidColorBrush GridAlternatingRowBackground = Styles.Brush095;
        public static SolidColorBrush NoBackground                 = Styles.Brush10;
        public static SolidColorBrush TextForeground               = Styles.Brush00;
        public static SolidColorBrush ErrorBrush                   = Styles.ErrorBrush;


        public static BlockUIContainer DocumentHeader(string header, string systemName)
        {
            var grid = new Grid();
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid, 5);
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.Children.Add(GridUtil.GridCell(header,     0, 0, true, HorizontalAlignment.Left, PrintDocumentHeaderFontSize, PrintHeaderFont));
            grid.Children.Add(GridUtil.GridCell(systemName, 1, 0, true, HorizontalAlignment.Left, PrintPageHeaderFontSize, PrintHeaderFont));
            return new(grid);
        }

        public static BlockUIContainer PageHeader(string header)
        {
            var grid = new Grid();
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid, 5);
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(header, 0, 0, true, HorizontalAlignment.Left, PrintPageHeaderFontSize, PrintHeaderFont), new(3,2,3,10)));
            grid.Children.Add(GridUtil.GridCell("", 1, 0));
            return new(grid);
        }
        public static Grid PageHeaderGrid(string header)
        {
            var grid = new Grid();
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid, 5);
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(header, 0, 0, true, HorizontalAlignment.Left, PrintPageHeaderFontSize, PrintHeaderFont), new(3,2,3,10)));
            grid.Children.Add(GridUtil.GridCell("", 1, 0));
            return grid;
        }

        
        public static Run Text(string text, int fontSize = PrintDefaultFontSize, string fontFamily = PrintDefaultFont) => new Run(text) { FontFamily = new(fontFamily), FontSize = fontSize };

        public static Bold BoldText(string text, int fontSize = PrintDefaultFontSize, string fontFamily = PrintDefaultFont)
        {
            var bold = new Bold() { FontFamily = new(fontFamily), FontSize = fontSize };
            bold.Inlines.Add(Text(text));
            return bold;
        }


        //public static Paragraph Paragraph(string text, int fontSize, bool bold = false)
        //{
        //    var para = new Paragraph();
        //    if (bold)
        //        para.Inlines.Add(BoldText(text, fontSize, PrintHeaderFont));
        //    else
        //        para.Inlines.Add(Text(text, fontSize, PrintHeaderFont));
        //    return para;
        //}
    }
}
