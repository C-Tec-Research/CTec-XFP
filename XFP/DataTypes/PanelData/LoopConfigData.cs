using Newtonsoft.Json;
using System.Collections.Generic;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class LoopConfigData : ConfigData, IConfigData
    {
        internal LoopConfigData(int numLoops)
        {
            NumLoops = numLoops;
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Device_Details);
        }

        internal LoopConfigData(LoopConfigData original) : this(original.NumLoops)
        {
            if (original is not null)
            {
                Loop1 = new DeviceConfigData(original.Loop1);
                Loop2 = NumLoops > 1 ? new DeviceConfigData(original.Loop2) : new(1);
            }
        }


        /// <summary>Number of devices in the data</summary>
        internal const int MaxLoops = 2;
        internal const int MinManualCallpoints = 1;
        internal const int MaxManualCallpoints = 15;
        internal const int MinInterfaceUnits = 1;
        internal const int MaxInterfaceUnits = 15;
        internal const int MinDetectors = 1;
        internal const int MaxDetectors = 60;


        public static int? DetectedLoops { get; set; } = null;
        public int NumLoops           { get; set; } = MaxLoops;

        public DeviceConfigData Loop1 { get; set; } = new(0);
        public DeviceConfigData Loop2 { get; set; } = new(1);
        [JsonIgnore] public List<DeviceConfigData> Loops => new() { Loop1, Loop2 };


        public void NormaliseLoops()
        {
            //ensure Loop #1 has correct device count
            while (Loop1.Devices.Count < DeviceConfigData.NumDevices)
                Loop1.Devices.Add(DeviceData.InitialisedNew(null, 0, Loop1.Devices.Count));
            while (Loop1.Devices.Count > DeviceConfigData.NumDevices)
                Loop1.Devices.RemoveAt(Loop1.Devices.Count - 1);

            if (NumLoops > 1)
            {
                //ensure Loop #2 has correct device count
                while (Loop2.Devices.Count < DeviceConfigData.NumDevices)
                    Loop2.Devices.Add(DeviceData.InitialisedNew(null, 1, Loop2.Devices.Count));
                while (Loop2.Devices.Count > DeviceConfigData.NumDevices)
                    Loop2.Devices.RemoveAt(Loop2.Devices.Count - 1);
            }
            else
            {
                //no loop #2
                Loop2.Devices.Clear();
            }
        }


        /// <summary>
        /// Returns an initialised DeviceConfigData object.
        /// </summary>
        internal static LoopConfigData InitialisedNew(int numLoops)
        {
            var data = new LoopConfigData(numLoops);
            data.Loop1 = DeviceConfigData.InitialisedNew(0);
            if (numLoops > 1)
                data.Loop2 = DeviceConfigData.InitialisedNew(1);
            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not LoopConfigData od)
                return false;

            if (od.NumLoops != NumLoops)
                return false;
            if (!od.Loop1.Equals(Loop1))
                return false;
            if (!od.Loop2.Equals(Loop2))
                return false;
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            if (!Loop1.Validate())
            {
                var epi = Loop1.GetErrorItems();
                var p = new ConfigErrorPageItems(1, epi.Name);
                foreach (var e in epi.ValidationCodes)
                    p.ValidationCodes.Add(e);
                _pageErrorOrWarningDetails.Items.Add(p);
            }

            if (NumLoops > 1)
            {
                if (!Loop2.Validate())
                {
                    var epi = Loop2.GetErrorItems();
                    var p = new ConfigErrorPageItems(2, epi.Name);
                    foreach (var e in epi.ValidationCodes)
                        p.ValidationCodes.Add(e);
                    _pageErrorOrWarningDetails.Items.Add(p);
                }
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
