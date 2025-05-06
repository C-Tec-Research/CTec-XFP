//using CTecUtil.Printing;
//using System.Windows;
//using System.Windows.Documents;
//using System.Windows.Media;

//public class DocumentPaginatorWrapper : DocumentPaginator
//{
//    private Size              _pageSize;
//    private Size              _margin;
//    private DocumentPaginator _paginator;
//    private Typeface          _typeface;
//    private string            _header;   

//    public DocumentPaginatorWrapper(DocumentPaginator paginator, Size pageSize, Size margin, string header)
//    {
//        _pageSize  = pageSize;
//        _margin    = margin;   
//        _paginator = paginator;
//        _header    = header;
 
//        _paginator.PageSize = new Size(_pageSize.Width, _pageSize.Height - margin.Height * 2);
//    }
  

//    private Rect Move(Rect rect)
//    {
//        if (rect.IsEmpty)
//            return rect;
        
//        return new Rect(rect.Left + _margin.Width, rect.Top + _margin.Height, rect.Width, rect.Height);               
//    }
   

//    public override DocumentPage GetPage(int pageNumber)
//    {
//        DocumentPage page = _paginator.GetPage(pageNumber);
 
//        // Create a wrapper visual for transformation and add extras
//        ContainerVisual newpage = new ContainerVisual();
 
//        DrawingVisual title = new DrawingVisual();
 
//        SolidColorBrush TextForeground = Styles.Brush04;

//        using (DrawingContext ctx = title.RenderOpen())
//        {
//            if (_typeface == null)
//                _typeface = new Typeface(PrintUtil.PrintDefaultFont);
 
//            FormattedText headerText = new FormattedText(_header,
//                                                         System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
//                                                         _typeface, PrintUtil.PrintDefaultFontSize, TextForeground);

            
 
//            FormattedText footerText = new FormattedText("Page " + (pageNumber + 1),
//                                                         System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
//                                                         _typeface, PrintUtil.PrintDefaultFontSize, TextForeground);
 
//            ctx.DrawText(headerText, new Point(20, -96 / 4)); // 1/4 inch above page content
//            ctx.DrawText(footerText, new Point(20, 96 / 4)); // 1/4 inch below page content
//        }
 
//        DrawingVisual background = new DrawingVisual();
 
//        using (DrawingContext ctx = background.RenderOpen())
//        {
//            ctx.DrawRectangle(new SolidColorBrush(Colors.White), null, page.ContentBox);
//        }
 
//        newpage.Children.Add(background); // Scale down page and center
       
//        ContainerVisual smallerPage = new ContainerVisual();
//        smallerPage.Children.Add(page.Visual);
//        smallerPage.Transform = new MatrixTransform(0.95, 0, 0, 0.95, 0.025 * page.ContentBox.Width, 0.025 * page.ContentBox.Height);
 
//        newpage.Children.Add(smallerPage);
//        newpage.Children.Add(title);
 
//        newpage.Transform = new TranslateTransform(_margin.Width, _margin.Height);
 
//        return new DocumentPage(newpage, _pageSize, Move(page.BleedBox), Move(page.ContentBox));
//    }
 

//    public override IDocumentPaginatorSource Source => _paginator.Source;
//    public override bool IsPageCountValid           => _paginator.IsPageCountValid;
//    public override int PageCount                   => _paginator.PageCount;
 
//    public override Size PageSize
//    {
//        get => _paginator.PageSize;
//        set => _paginator.PageSize = value;
//    }
 
//}