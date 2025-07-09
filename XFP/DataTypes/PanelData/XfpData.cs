using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CTecDevices.Protocol;
using System.Windows;
using Newtonsoft.Json;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Views.PanelTools;
using System.Windows.Input;
using Windows.Devices.SmartCards;

namespace Xfp.DataTypes
{
    public class XfpData : ConfigData
    {
        internal XfpData()
        {
            ToolsVersion = BuildInfo.Details.Version;
            Comments = "";
        }

        internal XfpData(XfpData original) : this()
        {
            if (original is null)
                return;

            foreach (var p in original.Panels)
                Panels.Add(p.Key, new(p.Value));

            SiteConfig = new(original.SiteConfig);
            Comments = original.Comments;
            //FirmwareVersion = Panels[0].PanelConfig.FirmwareVersion;
            ToolsVersion = original.ToolsVersion;
        }

        /// <summary>
        /// Copy-constructor based on older format of Xfp2 file.
        /// </summary>
        /// <param name="original"></param>
        internal XfpData(XfpData_OldVersion_1 original) : this()
        {
            if (original is null)
                return;

            SiteConfig = new(original.SiteConfig);

            Panels.Add(original.PanelNumber, new XfpPanelData()
            {
                PanelNumber = original.PanelNumber,
                Protocol = original.Protocol,
                CEConfig = new(original.CEConfigData),
                LoopConfig = new(original.LoopConfig),
                DeviceNamesConfig = new(original.DeviceNamesConfig),
                ZoneConfig = new(original.ZoneConfig),
                ZonePanelConfig = new(original.ZonePanelConfig),
                SetConfig = new(original.SetConfig),
                GroupConfig = new(original.GroupConfig),
                NetworkConfig = new(original.NetworkConfig)
            });

            Comments = original.Comments;
            //FirmwareVersion = original.FirmwareVersion;
            ToolsVersion = original.ToolsVersion;
        }


        public Dictionary<int, XfpPanelData> Panels { get; set; } = new();

        [JsonIgnore]
        internal static int CurrentPanelNumber { get; set; } = MinPanelNumber;

        [JsonIgnore]
        public XfpPanelData CurrentPanel
        {
            get
            {
                XfpPanelData result;
                if (Panels is not null && Panels.TryGetValue(CurrentPanelNumber, out result))
                    return result;

                result = Panels.First().Value;
                CurrentPanelNumber = result.PanelNumber;
                return result;
            }
        }


        public SiteConfigData SiteConfig { get; set; }
        public string Comments { get; set; }

        public string ToolsVersion { get; set; }

        //legacy setting from file format with single panel only - retained for backward compatibility with old data files
        public string FirmwareVersion { get; set; }


        public const int MinPanelNumber = 1;
        public const int MaxPanelNumber = 8;
        public const int EventLogDataLength = 128;


        //        internal static new XfpData InitialisedNew() => InitialisedNew(CTecDevices.ObjectTypes.XfpCast, CurrentPanelNumber, true);

        internal static XfpData InitialisedNew(CTecDevices.ObjectTypes protocol, int panelNumber, bool setCurrentToThis)
        {
            var data = new XfpData();
            data.Panels.Add(panelNumber, XfpPanelData.InitialisedNew(protocol, panelNumber));
            data.SiteConfig = SiteConfigData.InitialisedNew();
            if (setCurrentToThis)
                CurrentPanelNumber = panelNumber;
            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not XfpData od)
                return false;

            if (Panels?.Count != od.Panels?.Count)
                return false;

            foreach (var pk in Panels.Keys)
                if (!od.Panels.Keys.Contains(pk))
                    return false;

            foreach (var odpk in od.Panels.Keys)
                if (!Panels.Keys.Contains(odpk))
                    return false;

            foreach (var p in Panels)
                if (!p.Value.Equals(od.Panels[p.Key]))
                    return false;

            return SiteConfig.Equals(od.SiteConfig)
                //&& FirmwareVersion == od.FirmwareVersion
                //&& ToolsVersion    == od.ToolsVersion
                && Comments == od.Comments;
        }


        ///// <summary>
        ///// Checks whether the data's FirmwareVersion is the same as the specified version.
        ///// </summary>
        ///// <returns>Null if FirmwareVersion is null</returns>
        //internal bool? FirmwareVersionEquals(string otherFirmwareVersion) => FirmwareVersion is not null ? FirmwareVersion == otherFirmwareVersion : null;

        /// <summary>
        /// Checks whether the data's ToolsVersion is the same as the specified version.
        /// </summary>
        /// <returns>Null if ToolsVersion is null</returns>
        internal bool? ToolsVersionEquals(string otherToolsVersion) => ToolsVersion is not null ? ToolsVersion == otherToolsVersion : null;


        ///// <summary>
        ///// Compares FirmwareVersion with the specified version number.
        ///// </summary>
        ///// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        //internal int? FirmwareVersionCompare(string otherFirmwareVersion) => CTecUtil.TextProcessing.CompareFirmwareVersion(otherFirmwareVersion, FirmwareVersion);


        /// <summary>
        /// Compares ToolsVersion with the specified version number.
        /// </summary>
        /// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        internal int? ToolsVersionCompare(string otherToolsVersion) => CTecUtil.TextProcessing.CompareSoftwareVersion(otherToolsVersion, ToolsVersion);


        /// <summary>Current errors or warnings</summary>
        private List<ConfigErrorPage> _errorsAndWarnings = new();


        public override bool Validate()
        {
            _errorsAndWarnings.Clear();

            foreach (var p in Panels)
                if (!p.Value.Validate())
                {
                    var e = p.Value.GetErrorItems();
                    //foreach (var e in p.Value.GetPageErrorDetails().Items)
                    //    _errorsAndWarnings.Add();
                }

            if (!SiteConfig.Validate())
                _errorsAndWarnings.Add(SiteConfig.GetPageErrorDetails());

            return !HasErrorsOrWarnings();
        }


        internal new bool HasErrorsOrWarnings()
        {
            foreach (var _ in from p in Panels where p.Value.HasErrorsOrWarnings() select new { })
                return true;
            return SiteConfig.HasErrorsOrWarnings();
        }

        internal new bool HasErrors()
        {
            foreach (var _ in from p in Panels where p.Value.HasErrors() select new { })
                return true;
            return SiteConfig.HasErrors();
        }

        internal new bool HasWarnings()
        {
            foreach (var _ in from p in Panels where p.Value.HasWarnings() select new { })
                return true;
            return SiteConfig.HasWarnings();
        }

        

        private List<string> _ceActions;
        private List<string> _ceTriggers;
        private List<string> _groups;
        private List<string> _inputs;
        private List<string> _loop1Devices;
        private List<string> _loop2Devices;
        private List<string> _zones;
        private List<string> _zoneNames;
        private List<string> _zonesPanels;
        private List<string> _zonePanelNames;
        private List<string> _sets;
        private List<string> _events;
        private List<string> _relays;
        private List<string> _setsRelays;
        private List<string> _ceTimerTs;

        internal List<string> GetCEActionsList()
        {
            _ceActions = new();
            foreach (var a in Enum.GetValues(typeof(CEActionTypes)))
                _ceActions.Add(Enums.CEActionTypesToString((CEActionTypes)a));
            return _ceActions;
        }

        internal List<string> GetCETriggersList()
        {
            _ceTriggers = new();
            foreach (var t in Enum.GetValues(typeof(CETriggerTypes)))
                if ((CETriggerTypes)t != CETriggerTypes.None && (CETriggerTypes)t != CETriggerTypes.NotSet)
                    _ceTriggers.Add(Enums.CETriggerTypesToString((CETriggerTypes)t));
            return _ceTriggers;
        }

        internal List<string> GetGroupsList()
        {
            _groups = new();
            for (int i = 0; i <= GroupConfigData.NumSounderGroups; i++)
                _groups.Add(i == 0 ? Cultures.Resources.Action_All_Groups : string.Format(Cultures.Resources.Group_x, i));
            return _groups;
        }

        internal List<string> GetInputsList()
        {
            _inputs = new();
            for (int i = 0; i < 2; i++)
                _inputs.Add(string.Format(Cultures.Resources.Input_x, i + 1));
            return _inputs;
        }

        internal List<string> GetLoop1DeviceList(int panel)
        {
            _loop1Devices = new();
            for (int d = 0; d < DeviceConfigData.NumDevices && d < Panels[panel].Loop1Config.Devices.Count; d++)
                _loop1Devices.Add(string.Format(Cultures.Resources.Device_x_Type_y, d + 1, (DeviceTypes.DeviceTypeName(Panels[panel].Loop1Config.Devices[d].DeviceType, DeviceTypes.CurrentProtocolType) ?? Cultures.Resources.No_Device)));
            return _loop1Devices;
        }

        internal List<string> GetLoop2DeviceList(int panel)
        {
            _loop2Devices = new();
            for (int d = 0; d < DeviceConfigData.NumDevices && d < Panels[panel].Loop2Config.Devices.Count; d++)
                _loop2Devices.Add(string.Format(Cultures.Resources.Device_x_Type_y, d + 1, (DeviceTypes.DeviceTypeName(Panels[panel].Loop2Config.Devices[d].DeviceType, DeviceTypes.CurrentProtocolType) ?? Cultures.Resources.No_Device)));
            return _loop2Devices;
        }
  
        internal List<string> GetZonesList()
        {
            _zones = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                _zones.Add(string.Format(Cultures.Resources.Zone_x, i + 1));
            return _zones;
        }
  
        internal List<string> GetZoneNamesList(int panel)
        {
            _zoneNames = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                _zoneNames.Add(Panels[panel].ZoneConfig.Zones[i].Name);
            return _zoneNames;
        }
  
        internal List<string> GetZonePanelsList()
        {
            _zonesPanels = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                _zonesPanels.Add(string.Format(Cultures.Resources.Zone_x, i + 1));
            for (int i = 0; i < ZonePanelConfigData.NumZonePanels; i++)
                _zonesPanels.Add(string.Format(Cultures.Resources.Panel_x, i + 1));
            return _zonesPanels;
        }
  
        internal List<string> GetZonePanelNamesList(int panel)
        {
            _zonePanelNames = new();
            for (int i = 0; i < ZonePanelConfigData.NumZonePanels; i++)
                _zonePanelNames.Add(Panels[panel].ZonePanelConfig.Panels[i].Name);
            return _zonePanelNames;
        }

        internal List<string> GetSetsList()
        {
            _sets = new();
            for (int i = 0; i < XfpPanelData.NumSets; i++)
                _sets.Add(string.Format(Cultures.Resources.Set_x, i + 1));
            return _sets;
        }

        internal List<string> GetEventsList()
        {
            _events = new();
            for (int i = 0; i < CEConfigData.NumEvents; i++)
                _events.Add(string.Format(Cultures.Resources.Event_x, i + 1));
            return _events;
        }

        internal List<string> GetRelaysList()
        {
            _relays = new();
            for (int i = 0; i < XfpPanelData.NumRelays; i++)
                _relays.Add(string.Format(Cultures.Resources.Relay_x, i + 1));
            return _relays;
        }

        internal List<string> GetSetsRelaysList()
        {
            _setsRelays = new List<string>();
            for (int i = 0; i < XfpPanelData.NumSets; i++)
                _setsRelays.Add(string.Format(Cultures.Resources.Set_x, i + 1));
            for (int i = 0; i < XfpPanelData.NumRelays; i++)
                _setsRelays.Add(string.Format(Cultures.Resources.Relay_x, i + 1));
            return _setsRelays;
        }

        internal List<string> GetCETimerTList()
        {
            _ceTimerTs = new();
            for (int i = 0; i < CEConfigData.NumEvents; i++)
                _ceTimerTs.Add(string.Format(Cultures.Resources.Time_x, string.Format(Cultures.Resources.Time_T_x, i + 1)));
            return _ceTimerTs;
        }
    }
}
