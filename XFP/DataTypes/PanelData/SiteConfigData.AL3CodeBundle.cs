using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecUtil;

namespace Xfp.DataTypes.PanelData
{
    public partial class PanelConfigData
    {
        /// <summary>
        /// Subset of SiteConfigData containing the AL2 and AL3 access codes
        /// </summary>
        public class AL3CodeBundle
        {
            public AL3CodeBundle() { }


            public string AL3Code { get; set; }
            public bool   BlinkPollingLED { get; set; }
            public int    MCPDebounce { get; set; }
            public int    DetectorDebounce { get; set; }
            public int    IODebounce { get; set; }


            public byte[] ToByteArray() => ByteArrayProcessing.CombineByteArrays(ByteArrayProcessing.IntStrToByteArray(AL3Code, 2),
                                                                                 [ (byte)(BlinkPollingLED ? 0x01 : 0x00),
                                                                                   (byte)(MCPDebounce - 1),
                                                                                   (byte)(DetectorDebounce - 1),
                                                                                   (byte)(IODebounce - 1) ]);


            public static AL3CodeBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck)
            {
                if (!responseTypeCheck(data))
                    return null;

                var result = new AL3CodeBundle();

                result.AL3Code = TextProcessing.IntToZeroPaddedString(data[2] + (data[3] << 8), AccessCodeLength);
                result.BlinkPollingLED  = data[4] > 0;
                result.MCPDebounce      = data[5] + 1;
                result.DetectorDebounce = data[6] + 1;
                result.IODebounce       = data[7] + 1;
                return result;
            }
        }
    }
}
