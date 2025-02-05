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
        /// Subset of SiteConfigData containing the emergency and attack global/local settings
        /// </summary>
        public class EmergencySettings
        {
            public EmergencySettings(bool emergenciesSentGlobally, bool attacksSentGlobally) { EmergenciesSentGlobally = emergenciesSentGlobally; AttacksSentGlobally = attacksSentGlobally; }


            public bool EmergenciesSentGlobally { get; set; }
            public bool AttacksSentGlobally { get; set; }


            public byte[] ToByteArray() => new byte[] { (byte)(EmergenciesSentGlobally ? 1 : 0), (byte)(AttacksSentGlobally ? 1 : 0) };


            public static EmergencySettings Parse(byte[] data, Func<byte[], bool> responseTypeCheck)
            {
                if (!responseTypeCheck(data))
                    return null;

                if (data.Length > 3)
                    return new(data[2] != 0, data[3] != 0);

                return null;
            }
        }
    }
}
