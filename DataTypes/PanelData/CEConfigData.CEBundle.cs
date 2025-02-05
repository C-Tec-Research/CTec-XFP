using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public partial class CEConfigData
    {
        public class CEBundle
        {
            public CEBundle() { }


            public int            Index { get; set; }
            public CEActionTypes  ActionType { get; set; }
            public int            ActionParam { get; set; }
            public CETriggerTypes TriggerType { get; set; }
            public int            TriggerParam { get; set; }
            public int            TriggerParam2 { get; set; }
            public bool           TriggerCondition { get; set; }
            public CETriggerTypes ResetType { get; set; }
            public int            ResetParam { get; set; }
            public int            ResetParam2 { get; set; }
            public bool           ResetCondition { get; set; }
            public TimeSpan       TimerEventTime { get; set; }
            

            public byte[] ToByteArray()
            {
                var result = new byte[12];

                result[0] = (byte)(Index + 1);
                result[1] = (byte)ActionType;
                var ap = ByteArrayProcessing.IntToByteArray(ActionParam, 2);
                result[2] = ap[0];
                result[3] = ap[1];
                result[4] = (byte)TriggerType;
                if (!TriggerCondition)  result[4] |= 0x80;
                if (TriggerParam >= 0)  result[5] = (byte)TriggerParam;
                if (TriggerParam2 >= 0) result[6] = (byte)TriggerParam2;
                result[7] = (byte)ResetType;
                if (ResetCondition)   result[7] |= 0x80;
                if (ResetParam >= 0)  result[8] = (byte)ResetParam;
                if (ResetParam2 >= 0) result[9] = (byte)ResetParam2;
                result[10] = (byte)TimerEventTime.Minutes;
                result[11] = (byte)TimerEventTime.Hours;

                return result;
            }


            public static CEBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
            {
                if (!responseTypeCheck(data))
                    return null;

                CEBundle result = new();
                result.Index = requestedIndex??0;
                if (data.Length > 12)
                {
                    result.ActionType       = (CEActionTypes)data[2];
                    result.ActionParam      = Integer.Parse(data, null, 2, 3).Value;
                    result.TriggerType      = EnumConversions.ByteToCETriggerType((byte)(data[5] & 0x7f));
                    result.TriggerCondition = (data[5] & 0x80) == 0;
                    result.TriggerParam     = data[6];
                    result.TriggerParam2    = data[7];
                    result.ResetType        = EnumConversions.ByteToCETriggerType((byte)(data[8] & 0x7f));
                    result.ResetCondition   = (data[8] & 0x80) > 0;
                    result.ResetParam       = data[9];
                    result.ResetParam2      = data[10];
                    result.TimerEventTime   = TimeOfDay.Parse(data, null, 11).Value;
                }
                return result;
            }
        }
    }
}
