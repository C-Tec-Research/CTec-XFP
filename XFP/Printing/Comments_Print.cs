using CTecUtil.Printing;
using System.Windows;
using System.Windows.Documents;

namespace Xfp.DataTypes.PanelData
{
    public class CommentsData
    {
        public static void GetReport(FlowDocument doc, string commentsText)
        {
            PrintUtil.PageHeader(doc, Cultures.Resources.Nav_Comments);

            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(new Thickness(0, 2, 0, 2));

            doc.Blocks.Add(printComments(commentsText));

            TableUtil.ResetDefaults();
        }


        /// <summary>
        /// Print in a single column, single row table
        /// </summary>
        private static Table printComments(string commentsText)
        {
            var table = new Table();
            table.Padding = new(0);
            table.Margin = new(0);
            table.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            var rowGroup = new TableRowGroup();

            var row = new TableRow();
            if (!string.IsNullOrWhiteSpace(commentsText))
            {
                row.Cells.Add(TableUtil.NewCell(commentsText));
            }
            else
            {
                TableUtil.SetFontStyle(FontStyles.Italic);
                row.Cells.Add(TableUtil.NewCell(Cultures.Resources.Comments_Is_Blank));
            }
            rowGroup.Rows.Add(row);
            
            table.RowGroups.Add(rowGroup);
            return table;
        }
    }
}
