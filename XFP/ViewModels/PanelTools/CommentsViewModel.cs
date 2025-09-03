using System.Globalization;
using System.Windows;
using CTecControls.UI;
using Xfp.DataTypes;
using Xfp.UI.Interfaces;


namespace Xfp.ViewModels.PanelTools
{
    public class CommentsViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel
    {
        public CommentsViewModel(FrameworkElement parent) : base(parent) { }


        public string Comments { get => _data.Comments; set { _data.Comments = value; OnPropertyChanged(); } }


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) => PageHeader = Cultures.Resources.Nav_Comments;
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) => Comments = (_data = data).Comments;
        
        public void RefreshView()
        {
            Validator.IsValid(Parent);
            OnPropertyChanged(nameof(IsReadOnly));
        }
        #endregion

    }
}
