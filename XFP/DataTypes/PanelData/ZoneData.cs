using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;
using WinRT;
using Xfp.UI.Interfaces;
using static Xfp.DataTypes.PanelData.GroupConfigData;

namespace Xfp.DataTypes.PanelData
{
    public class ZoneData : ZoneBase
    {
        public ZoneData()
        {
            Day   = new();
            Night = new();
        }

        public override bool IsPanelData => false;
        public virtual bool MCPs { get; set; }
        public virtual bool Detectors { get; set; }
        public virtual int DetectorReset { get; set; }
        public virtual int AlarmReset { get; set; }
        public virtual bool EndDelays { get; set; }
        public virtual ZoneDependency Day { get; set; }
        public virtual ZoneDependency Night { get; set; }
        public ObservableCollection<SetTriggerTypes> OutputSetTriggers { get; set; }
        public ObservableCollection<SetTriggerTypes> PanelRelayTriggers { get; set; }

        
        public ZoneData(ZoneData original)
            : base(original)
        {
            MCPs          = original.MCPs;
            Detectors     = original.Detectors;
            DetectorReset = original.DetectorReset;
            AlarmReset    = original.AlarmReset;
            EndDelays     = original.EndDelays;
            Day           = new(original.Day);
            Night         = new(original.Night);
        }


        /// <summary>
        /// Returns an initialised ZoneData object.
        /// </summary>
        public static ZoneData InitialisedNew(int index)
        {
            var data = new ZoneData()
            {
                Index = index,
                //Name = string.Format(Cultures.Resources.Zone_x, index + 1),
            };

            data.Name = data.DefaultName;

            data.Detectors          = true;
            data.SounderGroups      = new();
            data.Day                = new();
            data.Night              = new();

            for (int i = 0; i < GroupConfigData.NumSounderGroups; i++)
                data.SounderGroups.Add(i < 5 ? AlarmTypes.Evacuate : AlarmTypes.Off);

            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not ZoneData od)
                return false;

            if (!base.Equals(otherData))
                return false;
                
            if (MCPs != od.MCPs
             || Detectors != od.Detectors
             || DetectorReset != od.DetectorReset
             || AlarmReset != od.AlarmReset
             || EndDelays != od.EndDelays)
                return false;

            return true;
        }


        public override bool Validate()
        {
            base.Validate(/*type*/);

            //if (SounderDelay.Add(Relay1Delay).Add(Relay2Delay).Add(RemoteDelay).Add(InputDelay).CompareTo(ZoneConfigData.MaxTotalDelay) > 0)
            //    _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataTotalDelayTooLong);

            if ((int)Day.DependencyOption < 0 || (int)Day.DependencyOption >= Enum.GetNames(typeof(ZoneDependencyOptions)).Length)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataDayOptionInvalid);

            if (Day.ShowDetectorReset && Day.DetectorReset.CompareTo(ZoneConfigData.MaxDetectorReset) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataDayDetectorResetTooLong);

            if (Day.ShowAlarmReset && Day.AlarmReset.CompareTo(ZoneConfigData.MaxAlarmReset) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataDayAlarmResetTooLong);

            if ((int)Night.DependencyOption < 0 || (int)Night.DependencyOption >= Enum.GetNames(typeof(ZoneDependencyOptions)).Length)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataNightOptionInvalid);

            if (Night.ShowDetectorReset && Night.DetectorReset.CompareTo(ZoneConfigData.MaxDetectorReset) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataNightDetectorResetTooLong);

            if (Night.ShowAlarmReset && Night.AlarmReset.CompareTo(ZoneConfigData.MaxAlarmReset) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataNightAlarmResetTooLong);

            return _errorItems.ValidationCodes.Count == 0;
        }


        public override byte[] ToByteArray()
        {
            var result = new byte[1];                
            return result;
        }


        public static ZonePanelData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        {
            ZonePanelData result = null;
            return result;
        }
    }
}
