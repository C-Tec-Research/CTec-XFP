using System.Collections.ObjectModel;
using Xfp.UI.Interfaces;
using CTecDevices.Protocol;
using Newtonsoft.Json;

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
            //DeviceIndex = original.DeviceIndex;
            //Group       = original.Group;

            if (original is not null)
                foreach (var d in original.Devices)
                    Devices.Add(new DeviceData(d));
        }


        /// <summary>
        /// Number of devices in the data<br/>
        /// NB: not a constant: it is dependent on the current protocol
        /// </summary>
        internal static int NumDevices
        {
            get => DeviceTypes.CurrentProtocolType switch
            {
                CTecDevices.ObjectTypes.XfpApollo => CTecDevices.Protocol.XfpApollo.NumDevices,
                _                                 => CTecDevices.Protocol.XfpCast.NumDevices,
            };
        }


        private static CTecDevices.ObjectTypes _protocol = CTecDevices.ObjectTypes.XfpCast;
        public static void SetDeviceProtocol(CTecDevices.ObjectTypes protocol) => _protocol = protocol;
        

        [JsonIgnore]internal const  int DefaultVolume = 3;
        [JsonIgnore]internal const  int MinSensitivity = 1;
        [JsonIgnore]internal const  int DefaultSensitivity = 3;
        [JsonIgnore]internal const  int MaxSensitivity = 5;
        [JsonIgnore]internal const  int MinSensitivityHigh = 75;
        [JsonIgnore]internal const  int DefaultSensitivityHigh = 100;
        [JsonIgnore]internal const  int MaxSensitivityHigh = 125;
        [JsonIgnore]internal const  int DefaultMode = 1;
        [JsonIgnore]internal const  int MinVolume = 1;
        [JsonIgnore]internal static int MaxVolume => DeviceTypes.VolumeSettings.Count;

        [JsonIgnore] public int LoopNum { get; set; }
        public ObservableCollection<DeviceData> Devices { get; set; } = new();
        //public int DeviceIndex { get; set; }
        //public int Group { get; set; }


        public static bool IsValidVolume(int? value)                                    => value.HasValue && value >= MinVolume - 1 && value <= MaxVolume - 1;
        public static bool IsValidMode(int? deviceType, int? value)                     => value.HasValue && DeviceTypes.ModeIsValid(deviceType, value);
        public static bool IsValidSensitivity(int? value, bool isSensitivityHighDevice) => value.HasValue && value >= (isSensitivityHighDevice ? MinSensitivityHigh : MinSensitivity) 
                                                                                                          && value <= (isSensitivityHighDevice ? MaxSensitivityHigh : MaxSensitivity);


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

            //if (DeviceIndex != od.DeviceIndex
            // || Group       != od.Group)
            //if (Group != od.Group)
            //    return false;

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
