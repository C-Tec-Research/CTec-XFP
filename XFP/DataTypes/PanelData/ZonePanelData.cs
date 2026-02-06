using System;
using Newtonsoft.Json;

namespace Xfp.DataTypes.PanelData
{
    public class ZonePanelData : ZoneBase
    {
        public ZonePanelData() : base() { }

        public ZonePanelData(ZonePanelData original) : base(original) { }


        [JsonIgnore] public XfpPanelData.PanelNameChangeHandler PanelNameChanged;


        public override bool IsPanelData => true;


        //private string _name;
        //public override string Name { get => _name; set { _name = value; } }
        //public override string Name { get; set; }

        internal override void SetDesc(string name) { Name = name; PanelNameChanged?.Invoke(Index, name); }


        /// <summary>
        /// Returns an initialised ZonePanelData object.
        /// </summary>
        public static ZonePanelData InitialisedNew(int index)
        {
            var data = new ZonePanelData()
            {
                Index = index,
                Name = string.Format(Cultures.Resources.Panel_x, index + 1),
            };
            
            data.Detectors     = true;
            data.Day           = new();
            data.Night         = new();
            data.SounderGroups = new();

            for (int i = 0; i < GroupConfigData.NumSounderGroups; i++)
                data.SounderGroups.Add(i < 5 ? AlarmTypes.Evacuate : AlarmTypes.Off);

            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not ZonePanelData od)
                return false;

            if (!base.Equals(otherData))
                return false;

            return true;
        }


        public override bool Validate()
        {
            base.Validate(/*type*/);
            return _errorItems.ValidationCodes.Count == 0;
        }


        public override byte[] ToByteArray()
        {
            var result = new byte[1];                
            return result;
        }


        public static ZoneBase Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        {
            ZoneBase result = null;
            return result;
        }
    }
}
