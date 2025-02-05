using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Xfp.DataTypes.PanelData.SetConfigData;
using static Xfp.DataTypes.PanelData.SiteConfigData;

namespace Xfp.DataTypes.PanelData
{
    public partial class GroupConfigData
    {
        /// <summary>
        /// Subset of SiteConfigData, containing Night Mode properties plus Call Follow
        /// </summary>
        public class GroupBundle
        {
            public GroupBundle() { }


            public int Index { get; set; }
            public List<AlarmTypes> Alarms { get; set; }
            public int PanelSounderGroup1 { get; set; }
            public int PanelSounderGroup2 { set; get; }
            public int AlertTone { get; set; }
            public int EvacTone { get; set; }
            public bool NewFireCausesResound { get; set; }
        

            public byte[] ToByteArray()
            {
                var result = new byte[22];

                int offset = 0;
                result[offset++] = (byte)(Index + 1);

                for (int i = 0; i < NumSounderGroups; i++)
                    result[offset + i] = (byte)Alarms[i];

                offset += NumSounderGroups;
                result[offset++] = (byte)PanelSounderGroup1;
                result[offset++] = (byte)PanelSounderGroup2;
                result[offset++] = (byte)(AlertTone + 1);
                result[offset++] = (byte)(EvacTone + 1);
                result[offset++] = (byte)(NewFireCausesResound ? 1 : 0);

                return result;
            }


            public static GroupBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? index, int startOffset = 2)
            {
                if (!responseTypeCheck(data))
                    return null;

                if (data.Length > startOffset + 21)
                {
                    var result = new GroupBundle();
                    result.Alarms = new();
                    for (int i = 0; i < NumSounderGroups; i++)
                        result.Alarms.Add(AlarmTypes.Off);

                    result.Index = index??0;

                    for (int i = 0; i < NumSounderGroups; i++)
                        result.Alarms[i] = EnumConversions.IntToAlarmType(data[i + startOffset]);

                    var offset = NumSounderGroups + startOffset;
                    result.PanelSounderGroup1   = data[offset++];
                    result.PanelSounderGroup2   = data[offset++];
                    result.AlertTone            = data[offset++] - 1;
                    result.EvacTone             = data[offset++] - 1;
                    result.NewFireCausesResound = data[offset++] > 0;

                    return result;
                }

                return null;
            }
        }
    }
}
