﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData : ConfigData, IConfigData
    {
        public ZoneConfigData()
        {
            Zones = new();
            OutputSetIsSilenceable = new();
            PanelRelayIsSilenceable = new();
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Zone_Configuration);
        }

        public ZoneConfigData(ZoneConfigData original) : this()
        {
            if (original is not null)
            {
                foreach (var z in original.Zones)
                    Zones.Add(new ZoneData(z));
                foreach (var o in original.OutputSetIsSilenceable)
                    OutputSetIsSilenceable.Add(o);
                foreach (var p in original.PanelRelayIsSilenceable)
                    PanelRelayIsSilenceable.Add(p);
                //DelayTimer = original.DelayTimer;
            }
        }


        /// <summary>
        /// The number of zones<br/>
        /// NB: a "16-zone" panel just means it only has 16 LEDs (it still works if 32 zones are sent to it)
        /// </summary>
        public const int NumZones = 32;     

        public const int NumPanels = 8;
        public const int NumDependencies = 6;
        public const int MaxNameLength = 14;
        public static readonly TimeSpan MaxOutputDelay   = new(0, 10, 0);
        public static readonly TimeSpan MaxTotalDelay    = new(0, 10, 0);
        public static readonly TimeSpan MaxDetectorReset = new(0,  1, 0);
        public static readonly TimeSpan MaxAlarmReset    = new(0, 30, 0);
        public static readonly TimeSpan MaxSetDelay      = new(0, 10, 0);

        public static bool IsValidZone(int? zone)               => zone.HasValue && zone >= 0 && zone <= ZoneConfigData.NumZones;
        public static bool IsValidZoneOrPanel(int? zonePanel)   => zonePanel.HasValue && zonePanel >= 0 && zonePanel < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels;
        public static bool IsValidOutputDelay(TimeSpan? delay)  => delay is null || delay?.CompareTo(ZoneConfigData.MaxOutputDelay) <= 0;
        public static bool IsValidDependencyOption(ZoneDependencyOptions option) => (int)option >= 0 && (int)option < Enum.GetNames(typeof(ZoneDependencyOptions)).Length;
        public static bool IsValidDetectorReset(TimeSpan? time) => time is null || time?.CompareTo(new(0, 0, 0)) >= 0 && time?.CompareTo(ZoneConfigData.MaxDetectorReset) <= 0;
        public static bool IsValidAlarmReset(TimeSpan? time)    => time is null || time?.CompareTo(new(0, 0, 0)) >= 0 && time?.CompareTo(ZoneConfigData.MaxAlarmReset) <= 0;
        public static bool IsValidSetDelay(TimeSpan? delay)     => delay.HasValue && delay?.CompareTo(new(0, 0, 0)) >= 0 && delay?.CompareTo(MaxSetDelay) <= 0;


        public ZonePanelConfigData ZonePanelData { get; set; }

        public List<ZoneData> Zones { get; set; }
        public TimeSpan InputDelay { get; set; }
        public TimeSpan InvestigationPeriod { get; set; }
        public List<bool> OutputSetIsSilenceable { get; set; }
        public List<bool> PanelRelayIsSilenceable { get; set; }


        public static bool HasDetectorReset(ZoneDependencyOptions option) => option == ZoneDependencyOptions.A;
        public static bool HasAlarmReset(ZoneDependencyOptions option)    => option switch { ZoneDependencyOptions.A or ZoneDependencyOptions.B => true, _ => false };


        #region elements in .XFP files that we don't use
        //[JsonIgnore]
        //public int DefaultAreaEquationValue { get; set; }

        //[JsonIgnore]
        //public int DefaultDeviceEquationValue { get; set; }

        //[JsonIgnore]
        //public string ZoneDefaultPrefix { get; set; }

        //[JsonIgnore]
        //public string ZoneZeroName = Cultures.Resources.Zone_Zero_Name;
        #endregion


        /// <summary>
        /// Returns an initialised ZoneConfigData object.
        /// </summary>
        public new static ZoneConfigData InitialisedNew()
        {
            var data = new ZoneConfigData();
            data.Zones = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                data.Zones.Add(ZoneData.InitialisedNew(i));
            for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                data.OutputSetIsSilenceable.Add(false);
            for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                data.PanelRelayIsSilenceable.Add(i == 0);
            //data.DelayTimer = new(0, DefaultDelayTimerSeconds / 60, DefaultDelayTimerSeconds % 60);
            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not ZoneConfigData od)
                return false;
            if (od.Zones.Count != Zones.Count
             || od.OutputSetIsSilenceable.Count != OutputSetIsSilenceable.Count
             || od.PanelRelayIsSilenceable.Count != PanelRelayIsSilenceable.Count
             /*|| od.DelayTimer != DelayTimer*/)
                return false;
            for (int i = 0; i < Zones.Count; i++)
                if (!od.Zones[i].Equals(Zones[i]))
                    return false;
            for (int i = 0; i < OutputSetIsSilenceable.Count; i++)
                if (!od.OutputSetIsSilenceable[i].Equals(OutputSetIsSilenceable[i]))
                    return false;
            for (int i = 0; i < PanelRelayIsSilenceable.Count; i++)
                if (!od.PanelRelayIsSilenceable[i].Equals(PanelRelayIsSilenceable[i]))
                    return false;
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            foreach (var z in Zones)
            {
                if (!z.Validate())
                {
                    var epi = z.GetErrorItems();
                    var p = new ConfigErrorPageItems(z.Number, epi.Name);
                    foreach (var e in epi.ValidationCodes)
                        p.ValidationCodes.Add(e);
                    _pageErrorOrWarningDetails.Items.Add(p);
                }
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}