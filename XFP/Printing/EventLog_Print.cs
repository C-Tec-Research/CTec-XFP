using CTecUtil.Printing;
using CTecUtil.Utils;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class EventLogData
    {
        public void GetReport(FlowDocument doc)
        {
            GridUtil.ResetDefaults();            
            PrintUtil.PageHeader(doc, Cultures.Resources.Print_Event_Log);

            var eventLogPage = new Section();
            var grid = new Grid();
            var row  = 0;

            if (!string.IsNullOrEmpty(FilePath))
            {
                GridUtil.AddRowToGrid(grid);
                GridUtil.AddCellToGrid(grid, "\n" + string.Format(CTecControls.Cultures.Resources.File_x, Path.GetFileName(FilePath)) + "\n", row++, 0, false, false);
            }

            GridUtil.SetNonProportionalFont();
            GridUtil.SetFontSize(8);
            GridUtil.AddRowToGrid(grid);

            if (!string.IsNullOrWhiteSpace(LogText))
            {
                GridUtil.AddCellToGrid(grid, LogText, row, 0, false, false);
            }
            else
            {
                GridUtil.SetFontStyle(FontStyles.Italic);
                GridUtil.AddCellToGrid(grid, Cultures.Resources.File_Is_Empty, row, 0, false, false);
            }

            eventLogPage.Blocks.Add(new BlockUIContainer(grid));
            doc.Blocks.Add(eventLogPage);
            GridUtil.ResetDefaults();
        }
    }
}
