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
        public class Timeouts
        {
            public Timeouts(int callAcceptTimeout, int divertTimeout) { CallAcceptTimeout = callAcceptTimeout; DivertTimeout = divertTimeout; }


            public int CallAcceptTimeout { get; set; }
            public int DivertTimeout { get; set; }


            public byte[] ToByteArray() => new byte[] { (byte)CallAcceptTimeout, (byte)DivertTimeout };


            public static Timeouts Parse(byte[] data, Func<byte[], bool> responseTypeCheck)
            {
                if (!responseTypeCheck(data))
                    return null;

                int t1 = 0, t2 = 0;
                if (data.Length > 3)
                {
                    t2 = data[2];
                    t1 = data[3];
                }
                return new(t1, t2);
            }
        }
    }
}
