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
        /// Subset of SiteConfigData, containing Call Accept and Divert timeouts
        /// </summary>
        public class AutoResetEscalateMode
        {
            public AutoResetEscalateMode(bool autoResetEnabled, bool callEscalateEnabled)
            {
                AutoResetEnabled = autoResetEnabled;
                CallEscalateEnabled = callEscalateEnabled;
            }


            public bool CallEscalateEnabled { get; set; }
            public bool AutoResetEnabled { get; set; }


            public byte[] ToByteArray() => [(byte)(CallEscalateEnabled ? 2 : AutoResetEnabled ? 1 : 0)];


            public static AutoResetEscalateMode Parse(byte[] data, Func<byte[], bool> responseTypeCheck)
            {
                if (!responseTypeCheck(data))
                    return null;

                int mode = 0;
                if (data.Length > 2) mode = data[2];
                return new(mode == 1, mode == 2);
            }
        }
    }
}
