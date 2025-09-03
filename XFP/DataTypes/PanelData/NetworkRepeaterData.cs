using System.Collections.Generic;
using System.Linq;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public class NetworkRepeaterData : ConfigData, IConfigData
    {
        public NetworkRepeaterData()
        {
            Repeaters = new();
            //for (int i = 0; i < NumRepeaters; i++)
            //    Repeaters.Add(new() { Number = i + 1, Name = String.Format(Cultures.Resources.Panel_x, i) });
        }

        public NetworkRepeaterData(NetworkRepeaterData original) : this()
        {
            Segment   = original.Segment;
            //PanelName = original.PanelName;
            Repeaters.AddRange(from r in original.Repeaters select r);
        }

        
        public const int NumRepeaters = 8;
        public const int NumPanelSettings = 8;


        public int    Segment { get; set; }
        //public string PanelName { get; set; }

        public List<NetworkRepeaterItemData> Repeaters { get; set; }


        /// <summary>
        /// Returns an initialised RepeaterConfigData object.
        /// </summary>
        public new static NetworkRepeaterData InitialisedNew()
        {
            //var data = new NetworkRepeaterData();
            //for (int i = 0; i < NumRepeaters; i++)
            //    data.Repeaters.Add(new() { Name = String.Format(Cultures.Resources.Panel_x, i) });
            //for (int i = 0; i < NumPanelSettings; i++)
            //    data.PanelSettings.Add(new());
            //return data;
            return new NetworkRepeaterData();
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not NetworkRepeaterData od)
                return false;

            if (Segment         != od.Segment
             //||PanelName        != od.PanelName
             || Repeaters.Count != od.Repeaters.Count)
                return false;

            for (int i = 0; i < Repeaters.Count; i++)
                if (Repeaters[i] != od.Repeaters[i])
                    return false;
            return true;
        }


        public override bool Validate()
        {
            return false;
        }
    }
}
