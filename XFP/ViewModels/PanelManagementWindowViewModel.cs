using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media;
using CTecUtil.UI;
using CTecUtil.ViewModels;
using CTecControls.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels
{
    class PanelManagementWindowViewModel : ViewModelBase
    {
        public PanelManagementWindowViewModel(XfpData data, CTecDevices.ObjectTypes protocol, List<int> panelNumberSet, Window panelManagementWindow)
        {
            _data = data;
            _protocol = protocol;
            _originalPanelNumberSet = [.. panelNumberSet];
            PanelNumberSet          = [.. panelNumberSet];
            _panelManagementWindow = panelManagementWindow;

            PanelList = new();
            for (int i = XfpData.MinPanelNumber; i <= XfpData.MaxPanelNumber; i++)
                PanelList.Add(new(i)
                {
                    StatusChanged = () => { foreach (var p in PanelList) p.RefreshView(); }, 
                    GetPanelNumberSet = () => PanelNumberSet,
                    GetTotalFitted = () => PanelNumberSet.Count
                });
        }


        private XfpData _data;
        private CTecDevices.ObjectTypes _protocol;
        private List<int> _panelNumberSet;
        private List<int> _originalPanelNumberSet;
        private Window _panelManagementWindow;
        private ObservableCollection<PanelManagementItemViewModel> _panelList = new();
        
        public ObservableCollection<PanelManagementItemViewModel> PanelList        { get => _panelList;      set { _panelList = value; OnPropertyChanged(); } }
        public List<int>                                          PanelNumberSet   { get => _panelNumberSet; set { _panelNumberSet = [.. value]; OnPropertyChanged(); } }
        public ScaleTransform                                     LayoutTransform => Config.UI.LayoutTransform;


        internal void AddPanel(PanelManagementItemViewModel panel)
        {
            if (panel is not null)
            {
                if (!PanelNumberSet.Contains(panel.Number))
                    PanelNumberSet.Add(panel.Number);
                RefreshView();
            }
        }

        internal void RemovePanel(PanelManagementItemViewModel panel)
        {
            if (panel is not null)
            {
                if (PanelNumberSet.Contains(panel.Number))
                    if (CTecMessageBox.ShowYesNoWarn(_panelManagementWindow, string.Format(Cultures.Resources.Remove_Panel_x, panel.Number), Cultures.Resources.Panel_Management) == MessageBoxResult.Yes)
                    {
                        PanelNumberSet.Remove(panel.Number);
                        RefreshView();
                    }
            }
        }

        internal void RefreshView()
        {
            foreach (var p in PanelList)
                p.RefreshView();
        }

        public bool ClosePanelManagementPopup(bool saveChanges)
        {
            if (saveChanges)
            {
                var msg = GetSaveMessage();
                var warn = msg != null;

                msg += Cultures.Resources.Do_You_Want_To_Save_These_Changes;

                var yesNo = warn ? CTecMessageBox.ShowYesNoWarn(_panelManagementWindow, msg, Cultures.Resources.Panel_Management)
                                 : CTecMessageBox.ShowYesNoQuery(_panelManagementWindow, msg, Cultures.Resources.Panel_Management);

                switch (yesNo)
                {
                    case MessageBoxResult.Yes:
                        UIState.SetBusyState();
                        foreach (var p in PanelList)
                        {
                            if (p.IsFitted)
                            {
                                if (!_originalPanelNumberSet.Contains(p.Number))
                                    _data.Panels.Add(p.Number, XfpPanelData.InitialisedNew(_protocol, p.Number, LoopConfigData.MaxLoops));
                            }
                            else
                            {
                                if (_originalPanelNumberSet.Contains(p.Number))
                                    _data.Panels.Remove(p.Number);
                            }
                        }
                        return true;

                    case MessageBoxResult.No:
                        return true;
                }
            }
            else
            {
                if (GetSaveMessage() == null)
                    return true;

                return CTecMessageBox.ShowYesNoQuery(_panelManagementWindow, Cultures.Resources.Cancel_Changes, Cultures.Resources.Panel_Management) == MessageBoxResult.Yes;
            }

            return false;
        }

        internal string GetSaveMessage()
        {
            int added = 0, removed = 0;
            foreach (var n in PanelNumberSet)
                if (!_originalPanelNumberSet.Contains(n))
                    added++;
            foreach (var n in _originalPanelNumberSet)
                if (!PanelNumberSet.Contains(n))
                    removed++;

            var msg = new StringBuilder();

            if (removed > 0)
                msg.Append(removed > 1 ? string.Format(Cultures.Resources.x_Panels_Removed, removed) : Cultures.Resources.One_Panel_Removed);

            if (added > 0)
                msg.Append(string.Format("{0}{1}", removed > 0 ? ", " : "", added > 1 ? string.Format(Cultures.Resources.x_Panels_Added, added) : Cultures.Resources.One_Panel_Added));

            if (msg.Length > 0)
            {
                msg.Append("." + Environment.NewLine + Environment.NewLine);
                return msg.ToString();
            }

            return null;
        }


        public void Close() { }


        #region Minimise, Maximise/Restore buttons
        private bool _windowIsMaximised;
        public bool WindowIsMaximised { get => _windowIsMaximised; set { _windowIsMaximised = value; OnPropertyChanged(); } }
        public void ChangeWindowState(WindowState windowState) => WindowIsMaximised = windowState == WindowState.Maximized;
        #endregion
    }
}
