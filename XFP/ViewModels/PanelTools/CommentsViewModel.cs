using System;
using System.Collections;
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
using CTecControls.UI;
using CTecControls.ViewModels;
using Xfp.DataTypes;
using Xfp.IO;
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
