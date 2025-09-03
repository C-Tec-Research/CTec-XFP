using System;
using CTecUtil.StandardPanelDataTypes;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData
    {
        /// <summary>
        /// Subset of SiteConfigData, containing the current date and time
        /// </summary>
        public class PcTime
        {
            public static byte[] ToByteArray() => new Date(DateTime.Now).ToByteArray("yyMMddHHmmss");
        }
    }
}
