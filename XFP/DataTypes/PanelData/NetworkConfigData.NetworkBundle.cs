using System;
using System.Collections.Generic;

namespace Xfp.DataTypes.PanelData
{
    public partial class NetworkConfigData
    {
        /// <summary>
        /// Subset of NetworkConfigData containing the fitted and accepts flags
        /// </summary>
        public class NetworkBundle
        {
            public NetworkBundle() { }

            
            public List<bool> RepeatersFitted { get; set; }
            public List<NetworkPanelSettingsData> PanelSettings { get; set; }


            public byte[] ToByteArray()
            {
                byte[] result = new byte[PanelSettings.Count];
                for (int i = 0; i < PanelSettings.Count; i++)
                {
                    byte acc = 0;
                    if (RepeatersFitted[i])                  acc |= 0x01;
                    if (PanelSettings[i].AcceptFaults)       acc |= 0x02;
                    if (PanelSettings[i].AcceptAlarms)       acc |= 0x04;
                    if (PanelSettings[i].AcceptControls)     acc |= 0x08;
                    if (PanelSettings[i].AcceptDisablements) acc |= 0x10;
                    if (PanelSettings[i].AcceptOccupied)     acc |= 0x20;
                    result[i] = acc;
                }

                return result;
            }


            public static NetworkBundle Parse(byte[] data, Func<byte[], bool> responseTypeCheck)
            {
                if (!responseTypeCheck(data))
                    return null;

                var result = new NetworkBundle();

                result.RepeatersFitted = new();
                result.PanelSettings = new();

                for (int i = 0; i < data[1]; i++)
                {
                    result.RepeatersFitted.Add((data[2 + i] & 0x01) > 0);

                    var panel = new NetworkPanelSettingsData();
                    panel.AcceptFaults       = (data[2 + i] & 0x02) > 1;
                    panel.AcceptAlarms       = (data[2 + i] & 0x04) > 1;
                    panel.AcceptControls     = (data[2 + i] & 0x08) > 1;
                    panel.AcceptDisablements = (data[2 + i] & 0x10) > 1;
                    panel.AcceptOccupied     = (data[2 + i] & 0x20) > 1;
                    result.PanelSettings.Add(panel);
                }
                
                return result;
            }
        }
    }
}
