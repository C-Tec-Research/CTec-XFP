using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Xfp.DataTypes.PanelData
{
    public partial class SetConfigData
    {
        /// <summary>
        /// Subset of SiteConfigData, containing Night Mode properties plus Call Follow
        /// </summary>
        public class SetBundle
        {
            public SetBundle() { }


            public int Index { get; set; }
            public List<SetTriggerTypes> OutputSetTriggers { get; set; }
            public List<SetTriggerTypes> PanelRelayTriggers { get; set; }
            public TimeSpan DelayTimer { get; set; }
            public bool IsPanelData => Index >= ZoneConfigData.NumZones;
        

            public byte[] ToByteArray()
            {
                var result = new byte[21];

                int offset = 0;
                result[offset++] = (byte)(Index);

                result[offset++] = (byte)PanelRelayTriggers[0];
                result[offset++] = (byte)PanelRelayTriggers[1];

                for (int i = 0; i < NumOutputSetTriggers; i++)
                    result[offset++] = EnumConversions.SetTriggerTypeToByte(OutputSetTriggers[i]);

                var dt = ByteArrayProcessing.IntToByteArray((int)DelayTimer.TotalSeconds, 2);
                result[offset++] = dt[0];
                result[offset++] = dt[1];

                return result;
            }


            public static SetBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? index, int startOffset = 2)
            {
                if (!responseTypeCheck(data))
                    return null;

                if (data.Length > startOffset + 20)
                {
                    var result = new SetBundle();

                    //init the row data
                    result.OutputSetTriggers = new();
                    result.PanelRelayTriggers = new();
                    for (int i = 0; i < NumOutputSetTriggers; i++)
                        result.OutputSetTriggers.Add(SetTriggerTypes.NotTriggered);
                    for (int i = 0; i < NumPanelRelayTriggers; i++)
                        result.PanelRelayTriggers.Add(SetTriggerTypes.NotTriggered);

                    result.Index = index??0;
                    var offset = startOffset;

                    for (int i = 0; i < NumOutputSetTriggers; i++)
                        result.OutputSetTriggers[i] = EnumConversions.IntToSetTriggerType(data[offset++]);

                    result.PanelRelayTriggers[0] = EnumConversions.IntToSetTriggerType(data[offset++]);
                    result.PanelRelayTriggers[1] = EnumConversions.IntToSetTriggerType(data[offset++]);
                    
                    var delaySecs = Integer.Parse(data, null, 2, offset).Value;
                    result.DelayTimer = new TimeSpan(0, 0, delaySecs == 0xffff ? 0 : delaySecs);

                    return result;
                }

                return null;
            }

        }
    }
}
