using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Data;
using System.ComponentModel;
using CTecUtil.ViewModels;
using CTecUtil.UI;

namespace CTecUtil.Printing
{
        public class CloneableObservableCollection<T> : ObservableCollection<T>, ICloneable
        {
            public object Clone() => MemberwiseClone();
        }


    public class CustomDataGridDocumentPaginator : DocumentPaginator
    {
        #region Constructor

        public CustomDataGridDocumentPaginator(DataGrid sourceDatGrid, string systemName, string reportTitle, Size pageSize, Thickness pageMargin, int startPage)
        {
            _dataGrid   = sourceDatGrid;

            foreach (var i in sourceDatGrid.ItemsSource)
            {
                if (i is CloneableObservableCollection<ViewModelBase> items)
                {
                    _dataGrid.ItemsSource = null;

                    //_dataSet    = new IEnumerable<object>();
                    //var i0 = sourceDatGrid.ItemsSource[0];

                    _dataGrid.ItemsSource = _dataGrid.ItemsSource;
                }
            }

            SystemName  = systemName;
            ReportTitle = reportTitle;
            PageSize    = pageSize;
            PageMargin  = pageMargin;
            PageNumber  = startPage;

            if (_dataGrid != null)
                measureElements();
        }

        #endregion
        
        #region Private Members

        private DataGrid _dataGrid;
        private IEnumerable _dataSet;

        private CloneableObservableCollection<ColumnDefinition> _tableColumnDefinitions;

        private double _avgRowHeight;

        private double _availableHeight;

        //private int _rowsPerPage;
        private int _currentRow = 0;

        private int _pageCount;
        
        private int _reportPage = 0;


        private static Style _reportTitleStyle          = Styles.ReportTitlePrintStyle;
        private static Style _pageHeaderStyle           = Styles.PageHeaderPrintStyle;
        private static Style _pageFooterStyle           = Styles.PageFooterPrintStyle;
        private static Style _printDefaultTextStyle     = Styles.DefaultTextPrintStyle;
        private static Style _printFixedFontTextStyle   = Styles.FixedFontTextPrintStyle;
        private static Style _alternatingRowBorderStyle = Styles.AlternatingRowBorderPrintStyle;
        private static Style _tableHeaderTextStyle      = Styles.TableHeaderTextPrintStyle;
        //private static Style _tableHeaderBorderStyle    = PrintStyles.DocumentHeaderBorder...;

        #endregion

        #region Public Properties

        #region Styling

        public Style GridContainerStyle { get; set; }

        #endregion

        public string SystemName { get; set; }
        public string ReportTitle { get; set; }

        public Thickness PageMargin { get; set; }

        public override Size PageSize { get; set; }

        public int PageNumber { get; set; }

        public override bool IsPageCountValid => true;
        public override int PageCount => _pageCount;
        public override IDocumentPaginatorSource Source => null;

        #endregion

        #region Public Methods

        public override DocumentPage GetPage(int pageNumber)
        {
            _reportPage++;
            PageNumber++;

            DocumentPage page = null;
            
            List<object> itemsSource = new List<object>();
            itemsSource.AddRange(from object item in CollectionViewSource.GetDefaultView(_dataGrid.ItemsSource) select item);

            if (itemsSource != null)
            {
                int rowIndex = 1;
                int startPos = _currentRow;//pageNumber * _rowsPerPage;
                //int endPos = startPos + _rowsPerPage;

                //Create a new grid
                //Grid tableGrid = createGrid(true) as Grid;

                //for (int index = startPos; index < endPos && index < itemsSource.Count; index++)
                //{
                //    if (rowIndex > 0)
                //    {
                //        object item = itemsSource[index];
                //        int columnIndex = 0;

                //        if (_sourceDataGrid.Columns != null)
                //        {
                //            foreach (var column in _sourceDataGrid.Columns)
                //            {
                //                addTableCell(tableGrid, column, item, rowIndex, columnIndex);
                //                columnIndex++;
                //            }
                //        }

                //        if (_alternatingRowBorderStyle != null && rowIndex % 2 == 0)
                //        {
                //            Border alernatingRowBorder = new Border();

                //            alernatingRowBorder.Style = _alternatingRowBorderStyle;
                //            alernatingRowBorder.SetValue(Grid.RowProperty, rowIndex);
                //            alernatingRowBorder.SetValue(Grid.ColumnSpanProperty, columnIndex);
                //            alernatingRowBorder.SetValue(Grid.ZIndexProperty, -1);
                //            tableGrid.Children.Add(alernatingRowBorder);
                //        }
                //    }

                //    rowIndex++;
                //}

                Grid tableGrid = new(); 

                for ( ; _currentRow < itemsSource.Count; _currentRow++)
                {
                    if (rowIndex > 0)
                    {
                        object item = itemsSource[_currentRow];
                        int columnIndex = 0;

                        if (_dataGrid.Columns != null)
                        {

                            foreach (var column in _dataGrid.Columns)
                            {
                                addTableCell(tableGrid, column, item, rowIndex, columnIndex);
                                columnIndex++;
                            }
                        }

                        if (_alternatingRowBorderStyle != null && rowIndex % 2 == 0)
                        {
                            Border alernatingRowBorder = new Border();

                            alernatingRowBorder.Style = _alternatingRowBorderStyle;
                            alernatingRowBorder.SetValue(Grid.RowProperty, rowIndex);
                            alernatingRowBorder.SetValue(Grid.ColumnSpanProperty, columnIndex);
                            alernatingRowBorder.SetValue(Grid.ZIndexProperty, -1);
                            tableGrid.Children.Add(alernatingRowBorder);
                        }
                    }

                    rowIndex++;
                }

                page = constructPage(tableGrid, pageNumber);
            }

            return page;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This function measures the heights of the page header, page footer and grid header and the first row in the grid
        /// in order to work out how manage pages might be required.
        /// </summary>
        private void measureElements()
        {
            double allocatedSpace = 0;

            //Measure the page header
            ContentControl pageHeader = new ContentControl();
            pageHeader.Content = createDocumentHeader();
            allocatedSpace += measureHeight(pageHeader);
            
            //Measure the page footer
            ContentControl pageFooter = new ContentControl();
            pageFooter.Content = createDocumentFooter(0);
            allocatedSpace += measureHeight(pageFooter);
            
            //Measure the table header
            ContentControl tableHeader = new ContentControl();
            tableHeader.Content = createGrid(false);
            allocatedSpace += measureHeight(tableHeader);

            //Include any margins
            allocatedSpace += PageMargin.Bottom + PageMargin.Top;

            //Work out how much space we need to display the grid
            _availableHeight = PageSize.Height - allocatedSpace;







            //Calculate the height of the first row
            _avgRowHeight = measureHeight(createTempRow());


            //Calculate how many rows we can fit on each page
            double rowsPerPage = Math.Floor(_availableHeight / _avgRowHeight);

            //if (!double.IsInfinity(rowsPerPage))
            //    _rowsPerPage = Convert.ToInt32(rowsPerPage);

            //Count the rows in the document source
            double rowCount = countRows(_dataGrid.ItemsSource);

            //Calculate the nuber of pages that we will need
            if (rowCount > 0)
                _pageCount = Convert.ToInt32(Math.Ceiling(rowCount / rowsPerPage));
        }


        private List<DataGrid> _reportPages = new();


        private DocumentPage constructPage(Grid content, int pageNumber)
        {
            if (content == null)
                return null;

            //Build the page inc header and footer
            Grid pageGrid = new Grid();

            //Header row
            addGridRow(pageGrid, GridLength.Auto);
            ContentControl pageHeader = new ContentControl();
            pageHeader.Content = createDocumentHeader();
            pageGrid.Children.Add(pageHeader);

            //Title row
            if (_reportPage == 1)
            {
                addGridRow(pageGrid, GridLength.Auto);
                ContentControl reportTitle = new ContentControl();
                reportTitle.Content = createReportTitle();
                reportTitle.SetValue(Grid.RowProperty, 1);
                pageGrid.Children.Add(reportTitle);
            }

            //Content row
            addGridRow(pageGrid, new GridLength(1.0d, GridUnitType.Star));

            if (content != null)
            {
                content.SetValue(Grid.RowProperty, _reportPage == 1 ? 2 : 1);
                pageGrid.Children.Add(content);
            }

            //Footer row
            //addGridRow(pageGrid, GridLength.Auto);
            //ContentControl pageFooter = new ContentControl();
            //pageFooter.Content = createDocumentFooter(pageNumber + 1);
            //pageFooter.SetValue(Grid.RowProperty, _reportPage == 1 ? 3 : 2);
            //pageGrid.Children.Add(pageFooter);


            double width  = PageSize.Width - (PageMargin.Left + PageMargin.Right);
            double height = PageSize.Height - (PageMargin.Top + PageMargin.Bottom);

            pageGrid.Measure(new Size(width, height));
            pageGrid.Arrange(new Rect(PageMargin.Left, PageMargin.Top, width, height));

            return new DocumentPage(pageGrid);
        }


        private object createReportTitle()
        {
            TextBlock titleText = new TextBlock();
            titleText.Style = _reportTitleStyle;
            titleText.TextTrimming = TextTrimming.CharacterEllipsis;
            titleText.Text = ReportTitle;
            titleText.HorizontalAlignment = HorizontalAlignment.Left;

            return titleText;
        }


        private object createDocumentHeader()
        {
            Grid headerGrid = new Grid();

            TextBlock systemNameText = new TextBlock();
            systemNameText.Style = _pageHeaderStyle;
            systemNameText.TextTrimming = TextTrimming.CharacterEllipsis;
            systemNameText.Text = SystemName;
            systemNameText.HorizontalAlignment = HorizontalAlignment.Left;
            headerGrid.Children.Add(systemNameText);

            ColumnDefinition colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(0.5d, GridUnitType.Star);

            TextBlock reportDateText = new TextBlock();
            reportDateText.Style = _pageHeaderStyle;
            reportDateText.Text  = DateTime.Now.ToString("dd-MMM-yyy HH:mm");
            reportDateText.SetValue(Grid.ColumnProperty, 1);
            reportDateText.HorizontalAlignment = HorizontalAlignment.Right;
            headerGrid.Children.Add(reportDateText);

            return headerGrid;
        }


        private object createDocumentFooter(int pageNumber)
        {
            Grid footerGrid = new Grid();

            ColumnDefinition colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(0.5d, GridUnitType.Star);

            TextBlock dateTimeText = new TextBlock();
            dateTimeText.Style = _pageFooterStyle;
            dateTimeText.Text = DateTime.Now.ToString("dd-MMM-yyy HH:mm");

            footerGrid.Children.Add(dateTimeText);

            TextBlock pageNumberText = new TextBlock();
            pageNumberText.Style = _pageFooterStyle;
            pageNumberText.Text = string.Format(Cultures.Resources.Print_Page_x_Of_y, pageNumber, PageCount);
            pageNumberText.SetValue(Grid.ColumnProperty, 1);
            pageNumberText.HorizontalAlignment = HorizontalAlignment.Right;

            footerGrid.Children.Add(pageNumberText);

            return footerGrid;
        }


        /// <summary>
        /// Counts the number of rows in the document source
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <returns></returns>
        private double countRows(IEnumerable itemsSource)
        {
            int count = 0;

            if (itemsSource != null)
            {
                foreach (object item in itemsSource)
                    count++;
            }

            return count;
        }


        /// <summary>
        /// The following function creates a temp table with a single row so that it can be measured and used to 
        /// calculate the total number of req'd pages
        /// </summary>
        /// <returns></returns>
        private Grid createTempRow()
        {
            Grid tableRow = new Grid();

            if (_dataGrid != null)
            {
                foreach (ColumnDefinition colDefinition in _tableColumnDefinitions)
                {
                    ColumnDefinition copy = XamlReader.Parse(XamlWriter.Save(colDefinition)) as ColumnDefinition;
                    tableRow.ColumnDefinitions.Add(copy);
                }

                foreach (object item in _dataGrid.ItemsSource)
                {
                    int columnIndex = 0;
                    if (_dataGrid.Columns != null)
                    {
                        foreach (DataGridColumn column in _dataGrid.Columns)
                        {
                            if (column.Visibility == Visibility.Visible)
                            {
                                addTableCell(tableRow, column, item, columnIndex, 0);
                                columnIndex++;
                            }
                        }
                    }

                    //We only want to measure the first row
                    break;
                }
            }

            return tableRow;
        }


        /// <summary>
        /// 
        /// </summary>
        private object createGrid(bool createRowDefinitions)
        {
            if (_dataGrid == null)
                return null;

            //Grid grid = new Grid();
            //grid.Style = GridContainerStyle;

            //int columnIndex = 0;

            //if (_sourceDataGrid.Columns != null)
            //{
            //    double  totalColumnWidth = _sourceDataGrid.Columns.Sum(column => column.Visibility == Visibility.Visible ? column.Width.Value : 0);

            //    foreach (DataGridColumn column in _sourceDataGrid.Columns)
            //    {
            //        if (column.Visibility == Visibility.Visible)
            //        {
            //            addTableColumn(grid, totalColumnWidth, columnIndex, column);
            //            columnIndex++;
            //        }
            //    }
            //}

            //if (_tableHeaderBorderStyle != null)
            //{
            //    Border headerBackground = new Border();
            //    headerBackground.Style = _tableHeaderBorderStyle;
            //    headerBackground.SetValue(Grid.ColumnSpanProperty, columnIndex);
            //    headerBackground.SetValue(Grid.ZIndexProperty, -1);

            //    grid.Children.Add(headerBackground);
            //}

            //if (createRowDefinitions)
            //{
            //    //for (int i = 0; i <= _rowsPerPage; i++)
            //        grid.RowDefinitions.Add(new RowDefinition());
            //}


            DataGrid grid = new();
            grid.Style = _dataGrid.Style;

            int columnIndex = 0;


            if (_dataGrid.Columns != null)
            {
                double  totalColumnWidth = _dataGrid.Columns.Sum(column => column.Visibility == Visibility.Visible ? column.Width.Value : 0);

                foreach (DataGridColumn column in _dataGrid.Columns)
                {
                    if (column.Visibility == Visibility.Visible)
                    {
                        //addTableColumn(grid, totalColumnWidth, columnIndex, column);
                        grid.Columns.Add(column);
                        columnIndex++;
                    }
                }
            }

            //if (_tableHeaderBorderStyle != null)
            //{
            //    Border headerBackground = new Border();
            //    headerBackground.Style = _tableHeaderBorderStyle;
            //    headerBackground.SetValue(Grid.ColumnSpanProperty, columnIndex);
            //    headerBackground.SetValue(Grid.ZIndexProperty, -1);

            //    grid.Children.Add(headerBackground);
            //}

            //if (createRowDefinitions)
            //{
            //    //for (int i = 0; i <= _rowsPerPage; i++)
            //    grid.RowDefinitions.Add(new RowDefinition());
            //}

            return grid;
        }


        /// <summary>
        /// Measures the height of an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private double measureHeight(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.Measure(PageSize);
            return element.DesiredSize.Height;
        }


        /// <summary>
        /// Adds a column to a grid
        /// </summary>
        /// <param name="grid">Grid to add the column to</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="column">Source column defintition which will be used to calculate the width of the column</param>
        private void addTableColumn(Grid grid, double totalColumnWidth, int columnIndex, DataGridColumn column)
        {
            double proportion = column.Width.Value / (PageSize.Width - (PageMargin.Left + PageMargin.Right));

            ColumnDefinition colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(proportion, GridUnitType.Star);

            grid.ColumnDefinitions.Add(colDefinition);

            TextBlock text = new TextBlock();
            text.Style = _tableHeaderTextStyle;
            text.TextTrimming = TextTrimming.CharacterEllipsis;
            text.Text = column.Header.ToString(); ;
            text.SetValue(Grid.ColumnProperty, columnIndex);

            grid.Children.Add(text);
            _tableColumnDefinitions.Add(colDefinition);
        }


        /// <summary>
        /// Adds a cell to a grid
        /// </summary>
        /// <param name="grid">Grid to add the cell to</param>
        /// <param name="column">Source column definition which contains binding info</param>
        /// <param name="item">The binding source</param>
        /// <param name="columnIndex">Column index</param>
        /// <param name="rowIndex">Row index</param>
        private void addTableCell(Grid grid, DataGridColumn column, object item, int columnIndex, int rowIndex)
        {
            if (column is DataGridTemplateColumn templateColumn)
            {
                ContentControl contentControl = new ContentControl();

                contentControl.Focusable = true;
                contentControl.ContentTemplate = templateColumn.CellTemplate;
                contentControl.Content = item;

                contentControl.SetValue(Grid.ColumnProperty, columnIndex);
                contentControl.SetValue(Grid.RowProperty, rowIndex);

                grid.Children.Add(contentControl);
            }
            else if (column is DataGridTextColumn textColumn)
            {
                TextBlock text = new TextBlock { Text = "Text" };

                Style textStyle = new();
//                textStyle.Setters .Add(new Setter(Grid.))
//                text.Style = textStyle;

                text.TextTrimming = TextTrimming.CharacterEllipsis;
                text.DataContext = item;

                Binding binding = textColumn.Binding as Binding;

                //if (!string.IsNullOrEmpty(column.DisplayFormat))
                    //binding.StringFormat = column.DisplayFormat;

                text.SetBinding(TextBlock.TextProperty, binding);

                text.SetValue(Grid.ColumnProperty, columnIndex);
                text.SetValue(Grid.RowProperty, rowIndex);
                //text.SetValue(Grid.RowSpan, rowIndex);

                grid.Children.Add(text);
            }
        }


        /// <summary>
        /// Adds a row to a grid
        /// </summary>
        private void addGridRow(Grid grid, GridLength rowHeight)
        {
            if (grid == null)
                return;

            RowDefinition rowDef = new RowDefinition();

            if (rowHeight != null)
                rowDef.Height = rowHeight;

            grid.RowDefinitions.Add(rowDef);
        }

        #endregion

    }
}
