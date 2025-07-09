using Xfp.DataTypes.PanelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Timers;
using Xfp.DataTypes;

namespace Xfp.ViewModels.PanelTools.ValidationWindow
{
    public class ValidationPanelViewModel : ValidationTreeViewItemViewModel
    {
        public ValidationPanelViewModel(string name, XfpPanelData panelData) : base(name)
        {
            _panelData = panelData;

            _pages = new()
            {
                new(Cultures.Resources.Panel_Site_Configuration),
                new(string.Format(Cultures.Resources.Nav_Loop_x, 1)),
                new(string.Format(Cultures.Resources.Nav_Loop_x, 2)),
                new(Cultures.Resources.Comms_Device_Names),
                new(Cultures.Resources.Nav_Zone_Configuration),
                new(Cultures.Resources.Nav_Group_Configuration),
                new(Cultures.Resources.Nav_Set_Configuration),
                new(Cultures.Resources.Nav_C_And_E_Configuration),
                new(Cultures.Resources.Nav_Network_Configuration),
            };

            foreach (var p in _pages)
                Add(p);
        }


        private XfpPanelData _panelData;
        private ObservableCollection<ValidationPageViewModel> _pages;
        public ObservableCollection<ValidationPageViewModel> Pages => _pages;


        public int PanelNumber => _panelData.PanelNumber;


        public void RefreshView(object source, ElapsedEventArgs eventArgs, int currentPanel, int currentLoop, string currentPage)
        {
            ValidationTreeViewItemViewModel page;
            //ValidationPageViewModel page;

            if (currentPanel == PanelNumber)
                IsExpanded = true;

            List<ConfigErrorPageItems> pageErrors;
            List<ConfigErrorPageItems> allPageErrors = new();
            bool expand;

            if ((page = FindInChildren(Pages, Cultures.Resources.Panel_Site_Configuration)) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.PanelConfig.GetPageErrorDetails().Items);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, string.Format(Cultures.Resources.Nav_Loop_x, 1))) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.Loop1Config.GetPageErrorDetails().Items);
                expand = currentPage == Cultures.Resources.Nav_Device_Details ? currentPanel == PanelNumber && currentLoop == 1 && (currentPage?.Equals(page.Name) ?? false) : false;
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, string.Format(Cultures.Resources.Nav_Loop_x, 2))) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.Loop2Config.GetPageErrorDetails().Items);
                expand = currentPage == Cultures.Resources.Nav_Device_Details ? currentPanel == PanelNumber && currentLoop == 2 && (currentPage?.Equals(page.Name) ?? false) : false;
                page.SetChildren(pageErrors, page, currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false));
            }

            if ((page = FindInChildren(Pages, Cultures.Resources.Comms_Device_Names)) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.DeviceNamesConfig.GetPageErrorDetails().Items);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, Cultures.Resources.Nav_Zone_Configuration)) is not null)
            {
                //combine Zone and ZonePanel errors `
                pageErrors = _panelData.ZoneConfig.GetPageErrorDetails().Items;
                pageErrors.AddRange(_panelData.ZonePanelConfig.GetPageErrorDetails().Items);

                allPageErrors.AddRange(pageErrors);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, Cultures.Resources.Nav_Group_Configuration)) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.GroupConfig.GetPageErrorDetails().Items);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, Cultures.Resources.Nav_Set_Configuration)) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.SetConfig.GetPageErrorDetails().Items);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, Cultures.Resources.Nav_C_And_E_Configuration)) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.CEConfig.GetPageErrorDetails().Items);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            if ((page = FindInChildren(Pages, Cultures.Resources.Nav_Network_Configuration)) is not null)
            {
                allPageErrors.AddRange(pageErrors = _panelData.NetworkConfig.GetPageErrorDetails().Items);
                expand = currentPanel == PanelNumber && (currentPage?.Equals(page.Name) ?? false);
                page.SetChildren(pageErrors, page, expand);
            }

            ErrorLevel = GetHighestErrorLevel(allPageErrors);
        }
    }
}
