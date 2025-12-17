using CTecUtil.Printing;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Xfp.DataTypes.PanelData
{
    public partial class EventLogData
    {
        public static void GetReport(FlowDocument doc)
        {
            PrintUtil.PageHeader(doc, Cultures.Resources.Print_Event_Log);

            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(new Thickness(0, 2, 0, 2));

            doc.Blocks.Add(printEventLog());

            TableUtil.ResetDefaults();
        }


        /// <summary>
        /// Print in a single column table, with one row for 
        /// the file name if needed, and one for the log text.
        /// </summary>
        private static Table printEventLog()
        {
            var table = new Table();
            table.Padding = new(0);
            table.Margin = new(0);
            table.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            var rowGroup = new TableRowGroup();

            if (!string.IsNullOrEmpty(FilePath))
            {
                var fileNameRow = new TableRow();
                fileNameRow.Cells.Add(TableUtil.NewCell(string.Format(CTecControls.Cultures.Resources.File_x, Path.GetFileName(FilePath))));
                rowGroup.Rows.Add(fileNameRow);
            }

            TableUtil.SetNonProportionalFont();

            var logRow = new TableRow();
            if (!string.IsNullOrWhiteSpace(LogText))
            {
                TableUtil.SetFontSize(8);
                logRow.Cells.Add(TableUtil.NewCell(LogText));
            }
            else
            {
                TableUtil.SetFontStyle(FontStyles.Italic);
                logRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.File_Is_Empty));
            }
            rowGroup.Rows.Add(logRow);
            
            table.RowGroups.Add(rowGroup);
            return table;
        }
    }
}
