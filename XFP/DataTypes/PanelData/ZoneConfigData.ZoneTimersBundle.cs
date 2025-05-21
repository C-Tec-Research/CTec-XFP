using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Intrinsics.Arm;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        /// <summary>
        /// Subset of ZoneConfigData
        /// </summary>
        public class ZoneTimersBundle
        {
            public ZoneTimersBundle() { }


            public int Index { get; set; }
            public TimeSpan SounderDelay { get; set; }
            public TimeSpan Relay1Delay { get; set; }
            public TimeSpan Relay2Delay { get; set; }
            public TimeSpan OutputDelay { get; set; }
            public ZoneDependencyOptions DayDependency { get; set; }
            public ZoneDependencyOptions NightDependency { get; set; }
            public TimeSpan DayDetectorResetTime { get; set; }
            public TimeSpan DayDetectorAlarmTime { get; set; }
            public TimeSpan NightDetectorResetTime { get; set; }
            public TimeSpan NightDetectorAlarmTime { get; set; }
            public int ZoneFunctions { get; set; }            
            public bool Detectors { get; set; }
            public bool MCPs { get; set; }
            public bool EndDelays { get; set; }


            public byte[] ToByteArray() => ByteArrayProcessing.CombineByteArrays([(byte)(Index + 1)],
                                                                                 ByteArrayProcessing.IntToByteArray((int)SounderDelay.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)Relay1Delay.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)Relay2Delay.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)OutputDelay.TotalSeconds, 2),
                                                                                 [EnumConversions.ZoneDependencyOptionToByte(DayDependency), EnumConversions.ZoneDependencyOptionToByte(NightDependency)],
                                                                                 ByteArrayProcessing.IntToByteArray((int)DayDetectorAlarmTime.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)DayDetectorResetTime.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)NightDetectorAlarmTime.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)NightDetectorResetTime.TotalSeconds, 2),
                                                                                 [(byte)((Detectors ? 0x01 : 0x00) | (MCPs ? 0x02 : 0x00) | (EndDelays ? 0x04 : 0x00))]);


            public static ZoneTimersBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? index, int startOffset = 2)
            {
                if (!responseTypeCheck(data))
                    return null;

                if (data.Length >= startOffset + 20)
                {
                    var result = new ZoneTimersBundle();
                    
                    result.Index = index??0;

                    var offset = startOffset;
                    result.SounderDelay            = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset).Value);
                    result.Relay1Delay             = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 2).Value);
                    result.Relay2Delay             = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 2).Value);
                    result.OutputDelay             = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 2).Value);
                    result.DayDependency           = EnumConversions.ByteToZoneDependencyOption(data[offset += 2]);
                    result.NightDependency         = EnumConversions.ByteToZoneDependencyOption(data[offset += 1]);
                    result.DayDetectorAlarmTime    = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 1).Value);
                    result.DayDetectorResetTime    = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 2).Value);
                    result.NightDetectorAlarmTime  = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 2).Value);
                    result.NightDetectorResetTime  = new TimeSpan(0, 0, Integer.Parse(data, null, 2, offset += 2).Value);
                    offset += 2;
                    result.Detectors               = (data[offset] & 0x01) == 0x01;
                    result.MCPs                    = (data[offset] & 0x02) == 0x02;
                    result.EndDelays               = (data[offset] & 0x04) == 0x04;
                    return result;
                }

                return null;
            }
        }
    }
}

