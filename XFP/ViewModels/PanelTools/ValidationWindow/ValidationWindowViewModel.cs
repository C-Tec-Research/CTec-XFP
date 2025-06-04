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
using CTecUtil.ViewModels;
using CTecControls.ViewModels;
using Xfp.Config;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels.PanelTools.ValidationWindow
{
    class ValidationWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// The errors/warnings display window viewmodel<br/>
        /// Note: if currentPanel, currentLoop & currentPage are set the error tree will expand the relevant branch if there are any errors or warnings within.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="currentPanel">The currently-open app page.</param> 
        /// <param name="currentLoop">The currently-open Loop (if on Device Details page).</param>
        /// <param name="currentPage">The currently-open app page.</param>
        public ValidationWindowViewModel(XfpData data, int currentPanel, int currentLoop, string currentPage)
        {
            ZoomLevel = XfpApplicationConfig.Settings.MainWindow.Scale;
            
            _data = data;

            _currentPanel = currentPanel;
            _currentPage = currentPage;
            _currentLoop = currentLoop;

            _panels = new();
            foreach (var p in _data.Panels)
                _panels.Add(new(string.Format(Cultures.Resources.Panel_x, p.Value.PanelNumber), p.Value));

            _siteConfig = new() { new(Cultures.Resources.Nav_Site_Configuration) };

            _refreshTimer = new() { Interval = 100, AutoReset = true };
            _refreshTimer.Elapsed += new ElapsedEventHandler(RefreshView);
            _refreshTimer.Start();
        }


        private XfpData _data;
        private int _currentPanel;
        private int _currentLoop;
        private string _currentPage;


        private Timer _refreshTimer;


        private ObservableCollection<ValidationPanelViewModel> _panels;
        public ObservableCollection<ValidationPanelViewModel> Panels => _panels;


        public ObservableCollection<ValidationPageViewModel> _siteConfig;
        public ObservableCollection<ValidationPageViewModel> SiteConfig => _siteConfig;


        private ScaleTransform _layoutTransform;
        public ScaleTransform LayoutTransform { get => _layoutTransform; set { _layoutTransform = value; OnPropertyChanged(); } }


        private ErrorLevels _errorLevel;
        public ErrorLevels ErrorLevel { get => _errorLevel; set { _errorLevel = value; OnPropertyChanged(); } }


        public void Close() => _refreshTimer.Stop();


        public string ValidationTypeHeader => Cultures.Resources.Validation_Header_Panel_Data;


        public void RefreshView(object source, ElapsedEventArgs eventArgs)
        {
            _refreshTimer.Stop();
            _refreshTimer.Interval = 30000;

            try
            {
                _data.Validate();

                foreach (var p in Panels)
                    p.RefreshView(source, eventArgs, _currentPanel, _currentLoop, _currentPage);


                List<ConfigErrorPageItems> pageErrors;
                List<ConfigErrorPageItems> allPageErrors = [.. pageErrors = _data.SiteConfig.GetPageErrorDetails().Items];
                _siteConfig[0]?.SetChildren(pageErrors, _siteConfig[0], _currentPage?.Equals(_siteConfig[0].Name) ?? false);


                ErrorLevels elv;
                ErrorLevel = ErrorLevels.OK;

                elv = _siteConfig[0].GetHighestErrorLevel();
                if (elv > ErrorLevel)
                    ErrorLevel = elv;

                foreach (var p in Panels)
                {
                    foreach (var c in p.Children)
                    {
                        if (c is null)
                            continue;

                        elv = c.GetHighestErrorLevel();
                        if (elv > ErrorLevel)
                            ErrorLevel = elv;
                    }
                    //var pan = ValidationTreeViewItemViewModel.FindInChildren(p, p.Name);
                    pageErrors = _data.Panels[p.PanelNumber].GetPageErrorDetails()?.Items;
                    if (pageErrors is not null)
                        allPageErrors.AddRange(pageErrors);
                }

                ErrorLevel = _data.HasErrors() ? ErrorLevels.Error : _data.HasWarnings() ? ErrorLevels.Warning : ErrorLevels.OK;
                OnPropertyChanged(nameof(Panels));
                OnPropertyChanged(nameof(SiteConfig));

                //expanding the current page only applies first time the window opens, so clear it now
                _currentPanel = 0;
                _currentLoop = 0;
                _currentPage = null;
            }
            finally
            {
                _refreshTimer.Interval = 3000;
                _refreshTimer.Start();
            }
        }


        #region Minimise, Maximise/Restore buttons
        private bool _windowIsMaximised;
        public bool WindowIsMaximised { get => _windowIsMaximised; set { _windowIsMaximised = value; OnPropertyChanged(); } }
        public void ChangeWindowState(WindowState windowState) => WindowIsMaximised = windowState == WindowState.Maximized;
        #endregion


        public double ZoomLevel
        {
            get => XfpApplicationConfig.Settings.ValidationWindowZoomLevel;
            set
            {
                XfpApplicationConfig.Settings.ValidationWindowZoomLevel = value;
                LayoutTransform = new ScaleTransform(value, value);
                OnPropertyChanged();
            }
        }        

        public void ZoomIn()  { ZoomLevel = (float)Math.Min(LayoutTransform.ScaleX + XfpApplicationConfig.ZoomStep, XfpApplicationConfig.MaxZoom); }
        public void ZoomOut() { ZoomLevel = (float)Math.Max(LayoutTransform.ScaleX - XfpApplicationConfig.ZoomStep, XfpApplicationConfig.MinZoom); }
    }
}
