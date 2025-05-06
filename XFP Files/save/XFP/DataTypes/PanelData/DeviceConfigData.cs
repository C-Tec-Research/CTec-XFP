using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.ViewModels;
using Xfp.UI.Interfaces;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using CTecDevices.Protocol;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData : ConfigData, IConfigData
    {
        internal DeviceConfigData()
        {
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Device_Details);
        }

        internal DeviceConfigData(int loopNum) : this()
        {
            LoopNum = loopNum;
        }

        internal DeviceConfigData(DeviceConfigData original) : this()
        {
            LoopNum     = original.LoopNum;
            DeviceIndex = original.DeviceIndex;
            Group       = original.Group;

            if (original is not null)
                foreach (var d in original.Devices)
                    Devices.Add(new DeviceData(d));
        }


        /// <summary>
        /// Number of devices in the data<br/>
        /// NB: not a constant: it is set at runtime according to the current protocol
        /// </summary>
        internal static int NumDevices = 255;

        internal static int DefaultVolume = 3;
        internal static int MinSensitivity = 1;
        internal static int DefaultSensitivity = 3;
        internal static int MaxSensitivity = 5;
        internal static int MinSensitivityHigh = 75;
        internal static int DefaultSensitivityHigh = 100;
        internal static int MaxSensitivityHigh = 125;
        internal static int MinVolume = 1;
        internal static int MaxVolume => DeviceTypes.VolumeSettings.Count;

        [JsonIgnore] public int LoopNum { get; set; }
        public List<DeviceData> Devices { get; set; } = new();
        public int DeviceIndex { get; set; }
        public int Group { get; set; }


        /// <summary>
        /// Returns an initialised DeviceConfigData object.
        /// </summary>
        internal static DeviceConfigData InitialisedNew(int loopNum)
        {
            var data = new DeviceConfigData(loopNum);

            for (int i = 0; i < NumDevices; i++)
                data.Devices.Add(DeviceData.InitialisedNew(null, loopNum, i));

            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not DeviceConfigData od)
                return false;

            if (DeviceIndex != od.DeviceIndex
             || Group       != od.Group)
                return false;

            if (od.Devices.Count != Devices.Count) 
                return false;
            for (int i = 0; i < Devices.Count; i++)
                if (!od.Devices[i].Equals(Devices[i]))
                    return false;
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            foreach (var d in Devices)
            {
                if (!d.Validate())
                {
                    var epi = d.GetErrorItems();
                    var p = new ConfigErrorPageItems(d.Index, epi.Name);
                    foreach (var e in epi.ValidationCodes)
                        p.ValidationCodes.Add(e);
                    _pageErrorOrWarningDetails.Items.Add(p);
                }
            }

             return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
