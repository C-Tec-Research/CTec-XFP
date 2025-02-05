using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData
    {
        /// <summary>
        /// Subset of SiteConfigData, containing the AutoResetTime
        /// </summary>
        public class AutoResetEscalatePeriod
        {
            public AutoResetEscalatePeriod(int period) => Period = period;


            public int Period { get; set; }


            public byte[] ToByteArray() => new byte[] { (byte)(Period / 10) };


            public static AutoResetEscalatePeriod Parse(byte[] data, Func<byte[], bool> responseTypeCheck)
            {
                if (!responseTypeCheck(data))
                    return null;
                return new(data.Length > 2 ? data[2] * 10 : 0);
            }
        }
    }
}
