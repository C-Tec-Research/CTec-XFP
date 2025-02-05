using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CTecControls.ViewModels;
using CTecUtil.IO;
using CTecUtil.Config;
using CTecControls.UI;
using CTecUtil;

namespace Xfp.ViewModels
{
    public class PrintDialogWindowViewModel : ViewModelBase
    {
        public PrintDialogWindowViewModel(ApplicationConfig applicationConfig)
        {
            ApplicationConfig = applicationConfig;
            ZoomLevel = ApplicationConfig.LogWindow.Scale;
        }


        public ApplicationConfig ApplicationConfig { get; }


        //public delegate void AppendTextHandler(string text, Color color);
        //public AppendTextHandler AppendText;


        public int    ErrorCount   => CTecUtil.CommsLog.ExceptionCount;
        public string HeaderEmpty  => null;
        public string HeaderErrors => "";


        private ScaleTransform _layoutTransform;
        public ScaleTransform LayoutTransform { get => _layoutTransform; set { _layoutTransform = value; OnPropertyChanged(); } }
        public void UpdateWindowParams(Window window, bool save = false) => ApplicationConfig.UpdateLogWindowParams(window, LayoutTransform.ScaleX, save);

        public void Close(Window window) => UpdateWindowParams(window, true);


        public string  CommsLogTitle => string.Format(Cultures.Resources.Select_Print_Options, CTecUtil.CommsLog.CurrentFileDate?.ToString("d"), CTecUtil.CommsLog.CurrentFileDate?.ToString("T"));

        //note: for some reason these next three won't bind to the source file in the xaml (whereas the Save tooltip does)
        public string  RetentionPeriodNote => "";

        private string CurrentFolder { get; set; }


        public bool HasPreviousFile => CTecUtil.CommsLog.HasPreviousFile;
        public bool HasNextFile     => CTecUtil.CommsLog.HasNextFile;




        public void RefreshView()
        {
            OnPropertyChanged(nameof(CommsLogTitle));
            OnPropertyChanged(nameof(CurrentFolder));
            OnPropertyChanged(nameof(CurrentFileName));
            OnPropertyChanged(nameof(HasPreviousFile));
            OnPropertyChanged(nameof(HasNextFile));
            OnPropertyChanged(nameof(ErrorCount));
            OnPropertyChanged(nameof(HeaderEmpty));
            OnPropertyChanged(nameof(HeaderErrors));
        }


        #region Minimise, Maximise/Restore buttons
        private bool _windowIsMaximised;
        public bool WindowIsMaximised { get => _windowIsMaximised; set { _windowIsMaximised = value; OnPropertyChanged(); } }
        public void ChangeWindowState(WindowState windowState) => WindowIsMaximised = windowState == WindowState.Maximized;
        #endregion


        public double ZoomLevel
        {
            get => ApplicationConfig.LogWindow.Scale;
            set
            {
                ApplicationConfig.LogWindow.Scale = value;
                LayoutTransform = new ScaleTransform(value, value);
                OnPropertyChanged();
            }
        }

        public void ZoomIn() { ZoomLevel = (float)Math.Min(LayoutTransform.ScaleX + ApplicationConfig.ZoomStep, CTecUtil.Config.ApplicationConfig.MaxZoom); }
        public void ZoomOut() { ZoomLevel = (float)Math.Max(LayoutTransform.ScaleX - ApplicationConfig.ZoomStep, CTecUtil.Config.ApplicationConfig.MinZoom); }
    }
}
