﻿using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        /// <summary>
        /// Subset of ZoneeConfigData
        /// </summary>
        public class PhasedSettingsBundle
        {
            public PhasedSettingsBundle() { }


            public TimeSpan PhasedDelay { get; set; }
            public TimeSpan InputDelay { get; set; }
            public TimeSpan InvestigationPeriod { get; set; }


            public byte[] ToByteArray() => ByteArrayProcessing.CombineByteArrays(PhasedDelay == TimeOfDay.Midnight ? [0xff, 0xff] : ByteArrayProcessing.IntToByteArray((int)PhasedDelay.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)InputDelay.TotalSeconds, 2),
                                                                                 ByteArrayProcessing.IntToByteArray((int)InvestigationPeriod.TotalSeconds, 2),
                                                                                 [0x00, 0x00]);


            public static PhasedSettingsBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int startOffset = 2)
            {
                if (!responseTypeCheck(data))
                    return null;

                if (data.Length > startOffset + 8)
                {
                    var result = new PhasedSettingsBundle();
                    
                    if (data[2] == 0xff && data[3] == 0xff)
                        result.PhasedDelay     = new TimeSpan(0, 0, 0);
                    else
                        result.PhasedDelay     = new TimeSpan(0, 0, Integer.Parse(data, null, 2, startOffset).Value);
                    
                    result.InputDelay          = new TimeSpan(0, 0, Integer.Parse(data, null, 2, startOffset + 2).Value);
                    result.InvestigationPeriod = new TimeSpan(0, 0, Integer.Parse(data, null, 2, startOffset + 4).Value);
                    return result;
                }

                return null;
            }

        }
    }
}

