using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Xfp.DataTypes.PanelData
{
    public class SetData : ConfigData
    {
        public SetData()
        {
        }

        public SetData(SetData original)
        {
            Index              = original.Index;
            OutputSetTriggers  = new();
            PanelRelayTriggers = new();

            foreach (var s in original.OutputSetTriggers)
                OutputSetTriggers.Add(s);
            foreach (var s in original.PanelRelayTriggers)
                PanelRelayTriggers.Add(s);
        }
        

        public int Index { get; set; }
        public int Number => Index + 1;
        public ObservableCollection<SetTriggerTypes> OutputSetTriggers { get; set; }
        public ObservableCollection<SetTriggerTypes> PanelRelayTriggers { get; set; }
        

        /// <summary>
        /// Returns an initialised SetData object.
        /// </summary>
        public static SetData InitialisedNew(int index, bool isPanelData = false)
        {
            var data = new SetData()
            {
                Index = index,
            };

            data.OutputSetTriggers  = new();
            data.PanelRelayTriggers = new();

            for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                data.OutputSetTriggers.Add(SetTriggerTypes.NotTriggered);
            for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                data.PanelRelayTriggers.Add(SetTriggerTypes.Continuous);

            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not SetData od)
                return false;

            if (Index != od.Index)
                return false;

            if (OutputSetTriggers.Count != od.OutputSetTriggers.Count
             || PanelRelayTriggers.Count != od.PanelRelayTriggers.Count)
                return false;

            for (int i = 0; i < OutputSetTriggers.Count; i++)
                if (OutputSetTriggers[i] != od.OutputSetTriggers[i])
                    return false;
            for (int i = 0; i < PanelRelayTriggers.Count; i++)
                if (PanelRelayTriggers[i] != od.PanelRelayTriggers[i])
                    return false;
            
            return true;
        }


        public override bool Validate()
        {
            _errorItems = new(Index, string.Format(Cultures.Resources.Set_x, Number));
            return true;
        }


        public byte[] ToByteArray()
        {
            var result = new byte[1];                
            return result;
        }


        public static ZoneData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        {
            ZoneData result = null;
            return result;
        }
    }
}
