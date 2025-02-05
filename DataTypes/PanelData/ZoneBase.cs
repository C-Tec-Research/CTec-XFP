using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using Xfp.UI.Interfaces;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using CTecControls.UI.ViewHelpers;

namespace Xfp.DataTypes.PanelData
{
    public abstract class ZoneBase : ConfigData
    {
        public ZoneBase() : base() { }

        public ZoneBase(ZoneBase original)
        {
            Index         = original.Index;
            Name          = original.Name;
            SounderDelay  = original.SounderDelay;
            Relay1Delay   = original.Relay1Delay;
            Relay2Delay   = original.Relay2Delay;
            RemoteDelay   = original.RemoteDelay;
            SounderGroups = new();
            foreach (var g in original.SounderGroups)
                SounderGroups.Add(g);
        }
        

        private string _name = "";

        public abstract bool IsPanelData { get; }
        public int Index { get; set; }
        public virtual string Name { get => string.IsNullOrEmpty(_name) ? DefaultName : _name; set => _name = value; }
        [JsonIgnore] public int    Number => Index + 1;
        [JsonIgnore] public string DefaultName => string.Format(IsPanelData ? Cultures.Resources.Panel_x : Cultures.Resources.Zone_x, Index + 1);
        [JsonIgnore] public string DisplayName => string.IsNullOrEmpty(Name) ? DefaultName : Name == DefaultName ? Name : string.Format(IsPanelData ? Cultures.Resources.Panel_Name_x_Value_y : Cultures.Resources.Zone_x_Name_y, Number, Name);
        public TimeSpan InputDelay { get; set; }
        public TimeSpan SounderDelay { get; set; }
        public TimeSpan Relay1Delay { get; set; }
        public TimeSpan Relay2Delay { get; set; }
        public TimeSpan RemoteDelay { get; set; }
        public List<AlarmTypes> SounderGroups { get; set; }
        

        internal virtual void SetDesc(string name) => Name = name;


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not ZoneBase od)
                return false;
                
            if (Index != od.Index
             || Name != od.Name
             || SounderDelay != od.SounderDelay
             || Relay1Delay != od.Relay1Delay
             || Relay2Delay != od.Relay2Delay
             || RemoteDelay != od.RemoteDelay)
                return false;

            return true;
        }


        public override bool Validate()
        {
            _errorItems = new(Index, string.Format(IsPanelData ? Cultures.Resources.Panel_x : Cultures.Resources.Zone_x, Number));

            if (Index < 0 || Index >= ZoneConfigData.NumZones + ZoneConfigData.NumPanels)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataInvalidZoneNum);
            
            if (Name.Length > ZoneConfigData.MaxNameLength)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataZoneNameTooLong);
            
            //if (SounderDelay.CompareTo(ZoneConfigData.MaxTotalDelay) > 0)
            //    _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataSounderDelayTooLong);
            
            //if (Relay1Delay.CompareTo(ZoneConfigData.MaxTotalDelay) > 0)
            //    _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataRelay1DelayTooLong);
            
            //if (Relay2Delay.CompareTo(ZoneConfigData.MaxTotalDelay) > 0)
            //    _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataRelay2DelayTooLong);
            
            //if (RemoteDelay.CompareTo(ZoneConfigData.MaxTotalDelay) > 0)
            //    _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataOutputDelayTooLong);
            
            if (SounderDelay.Add(Relay1Delay).Add(Relay2Delay).Add(RemoteDelay).Add(InputDelay).CompareTo(ZoneConfigData.MaxTotalDelay) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataTotalDelayTooLong);

            return _errorItems.ValidationCodes.Count == 0;
        }


        public abstract byte[] ToByteArray();


        //public static new PanelData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        //{
        //    PanelData result = null;
        //    return result;
        //}
    }
}
