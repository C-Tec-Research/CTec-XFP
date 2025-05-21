using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData
    {
        /// <summary>
        /// Subset of SiteConfigData, containing Night Mode properties plus Call Follow
        /// </summary>
        public class DayNightBundle
        {
            public DayNightBundle() { }


            public TimeSpan DayModeStart { get; set; }
            public TimeSpan NightModeStart { get; set; }
            public TimeSpan RecalibrateTime { get; set; }
            public List<bool> DayFlags { get; set; }
            public List<bool> NightFlags { get; set; }
            public bool AutoAdjustDST { get; set; }
            public bool RealTimeEventOutput { get; set; }


            public byte[] ToByteArray()
            {
                var dayNightRecal = ByteArrayProcessing.CombineByteArrays([(byte)DayModeStart.Minutes,   (byte)DayModeStart.Hours],
                                                                          [(byte)NightModeStart.Minutes, (byte)NightModeStart.Hours],
                                                                          [(byte)RecalibrateTime.Hours,  0]);
                byte dayFlags   = 0;
                byte nightFlags = 0;
                byte dstEnabled = (byte)(AutoAdjustDST ? 0x01 : 0x00);
                byte rtEvents   = (byte)(RealTimeEventOutput ? 0x01 : 0x00);

                for (int i = 0; i < DayFlags.Count; i++)
                    dayFlags += (byte)(DayFlags[i] ? 0x01 << i : 0x00);
                for (int i = 0; i < NightFlags.Count; i++)
                    nightFlags += (byte)(NightFlags[i] ? 0x01 << i : 0x00);

                return ByteArrayProcessing.CombineByteArrays(dayNightRecal, [dayFlags, nightFlags, dstEnabled, rtEvents]);
            }


            public static DayNightBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int startOffset = 2)
            {
                if (!responseTypeCheck(data))
                    return null;

                if (data.Length > startOffset + 10)
                {
                    var result = new DayNightBundle();
                    result.DayModeStart        = TimeOfDay.Parse(data, null, startOffset).Value;
                    result.NightModeStart      = TimeOfDay.Parse(data, null, startOffset + 2).Value;
                    result.RecalibrateTime     = new TimeSpan(data[startOffset + 5], 0,0);
                    result.DayFlags            = new() { (data[startOffset + 6] & 0x01) > 0,
                                                         (data[startOffset + 6] & 0x02) > 0,
                                                         (data[startOffset + 6] & 0x04) > 0,
                                                         (data[startOffset + 6] & 0x08) > 0,
                                                         (data[startOffset + 6] & 0x10) > 0,
                                                         (data[startOffset + 6] & 0x20) > 0,
                                                         (data[startOffset + 6] & 0x40) > 0 };
                    result.NightFlags          = new() { (data[startOffset + 7] & 0x01) > 0,
                                                         (data[startOffset + 7] & 0x02) > 0,
                                                         (data[startOffset + 7] & 0x04) > 0,
                                                         (data[startOffset + 7] & 0x08) > 0,
                                                         (data[startOffset + 7] & 0x10) > 0,
                                                         (data[startOffset + 7] & 0x20) > 0,
                                                         (data[startOffset + 7] & 0x40) > 0 };
                    result.AutoAdjustDST       = data[startOffset + 8] > 0;
                    result.RealTimeEventOutput = data[startOffset + 9] > 0;

                    return result;
                }

                return null;
            }
        }
    }
}
