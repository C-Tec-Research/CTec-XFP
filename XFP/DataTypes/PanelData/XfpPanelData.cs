using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Windows.ApplicationModel.Core;
using CTecDevices.Protocol;

namespace Xfp.DataTypes.PanelData
{
    public partial class XfpPanelData : ConfigData
    {
        internal XfpPanelData() { }

        internal XfpPanelData(XfpPanelData original) : this()
        {
            if (original is null)
                return;

            Protocol           = original.Protocol;
            PanelNumber        = original.PanelNumber;
            PanelConfig        = new(original.PanelConfig);
            LoopConfig         = new(original.LoopConfig);
            DeviceNamesConfig  = new(original.DeviceNamesConfig);
            ZoneConfig         = new(original.ZoneConfig);
            ZonePanelConfig    = new(original.ZonePanelConfig);
            SetConfig          = new(original.SetConfig);
            GroupConfig        = new(original.GroupConfig);
            CEConfig           = new(original.CEConfig);
            NetworkConfig      = new(original.NetworkConfig);
            PanelNumber        = original.PanelNumber;
            //Loop1Available     = original.Loop1Available;
            //Loop2Available     = original.Loop2Available;

            ZonePanelConfig.PanelNameChanged = new((index, name) => NetworkConfig.RepeaterSettings.Repeaters[index].Name = name);
            NetworkConfig.PanelNameChanged   = new((index, name) => ZonePanelConfig.Panels[index].Name = name);
        }


        [JsonIgnore] public XfpPanelData.PanelNameChangeHandler PanelNameChanged;


        public int                   PanelNumber { get; set; }
        public PanelConfigData       PanelConfig { get; set; }
        public CEConfigData          CEConfig { get; set; }
        public LoopConfigData        LoopConfig { get; set; }
        public DeviceNamesConfigData DeviceNamesConfig { get; set; }
        public ZoneConfigData        ZoneConfig { get; set; }
        public ZonePanelConfigData   ZonePanelConfig { get; set; }
        public SetConfigData         SetConfig { get; set; }
        public GroupConfigData       GroupConfig { get; set; }
        public NetworkConfigData     NetworkConfig { get; set; }


        [JsonIgnore] public DeviceConfigData  Loop1Config { get => LoopConfig?.Loop1; }
        [JsonIgnore] public DeviceConfigData  Loop2Config { get => LoopConfig?.Loop2; }


        public delegate void PanelNameChangeHandler(int index, string name);


        private CTecDevices.ObjectTypes _protocol;

        public CTecDevices.ObjectTypes Protocol
        {
            get => _protocol;
            set
            {
                if (_protocol != value)
                {
                    _protocol = value;
                    LoopConfig?.NormaliseLoops();
                }
            }
        }


        //public string Loop2Available { get; set; }
        //public string Loop1Available { get; set; }



        public delegate string PlaceNameLookup(int id);


        public const int SystemNameLength = 16;
        
        public const int FirmwareVersionLength = 5;

        /// <summary>Number of sets in the data</summary>
        public const int NumSets = 16;

        public const int NumRelays = 3;
        public const int NumPanelInputs = 2;


        //internal bool IsReadOnly { get; set; }


        /// <summary>
        /// Returns an initialised XfpPanelData object
        /// </summary>
        /// <param name="protocol">Default protocol</param>
        internal static XfpPanelData InitialisedNew(CTecDevices.ObjectTypes protocol, int panelNumber = 1)
        {
            var data = new XfpPanelData
            {
                //IsReadOnly = true,
                Protocol          = protocol,
                PanelNumber       = panelNumber,
                PanelConfig       = PanelConfigData.InitialisedNew(),
                LoopConfig        = LoopConfigData.InitialisedNew(),
                DeviceNamesConfig = DeviceNamesConfigData.InitialisedNew(),
                ZoneConfig        = ZoneConfigData.InitialisedNew(),
                ZonePanelConfig   = ZonePanelConfigData.InitialisedNew(),
                SetConfig         = SetConfigData.InitialisedNew(),
                GroupConfig       = GroupConfigData.InitialisedNew(),
                CEConfig          = CEConfigData.InitialisedNew(),
                NetworkConfig     = NetworkConfigData.InitialisedNew(),
            };

            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not XfpPanelData od)
                return false;
            
            if (Protocol        != od.Protocol
             || PanelNumber     != od.PanelNumber
             //|| Loop1Available  != od.Loop1Available
             //|| Loop2Available  != od.Loop2Available
             )
                return false;

            return PanelConfig.Equals(od.PanelConfig)
                && LoopConfig.Equals(od.LoopConfig)
                && DeviceNamesConfig.Equals(od.DeviceNamesConfig)
                && ZoneConfig.Equals(od.ZoneConfig)
                && ZonePanelConfig.Equals(od.ZonePanelConfig)
                && SetConfig.Equals(od.SetConfig)
                && GroupConfig.Equals(od.GroupConfig)
                && CEConfig.Equals(od.CEConfig)
                && NetworkConfig.Equals(od.NetworkConfig);
        }


        internal static void ErrorFlagsAddUnique(List<ValidationCodes> validationCodes, ValidationCodes validationCode)
        {
            if (!validationCodes.Contains(validationCode))
                validationCodes.Add(validationCode);
        }


        internal static void ErrorFlagsAddUnique(List<ValidationCodes> errorFlags, List<ValidationCodes> errorCodes)
        {
            foreach (var e in errorCodes)
                ErrorFlagsAddUnique(errorFlags, e);
        }


        public List<ConfigErrorPage> GetErrorsAndWarnings() => _errorsAndWarnings;

        /// <summary>Current errors or warnings</summary>
        private List<ConfigErrorPage> _errorsAndWarnings = new();


        public override bool Validate()
        {
            _errorsAndWarnings.Clear();

            if (!PanelConfig.Validate())       _errorsAndWarnings.Add(PanelConfig.GetPageErrorDetails());
            if (!Loop1Config.Validate())       _errorsAndWarnings.Add(Loop1Config.GetPageErrorDetails());
            if (!Loop2Config.Validate())       _errorsAndWarnings.Add(Loop2Config.GetPageErrorDetails());
            if (!DeviceNamesConfig.Validate()) _errorsAndWarnings.Add(DeviceNamesConfig.GetPageErrorDetails());
            if (!ZoneConfig.Validate())        _errorsAndWarnings.Add(ZoneConfig.GetPageErrorDetails());
            if (!ZonePanelConfig.Validate())   _errorsAndWarnings.Add(ZonePanelConfig.GetPageErrorDetails());
            if (!SetConfig.Validate())         _errorsAndWarnings.Add(SetConfig.GetPageErrorDetails());
            if (!GroupConfig.Validate())       _errorsAndWarnings.Add(GroupConfig.GetPageErrorDetails());
            if (!CEConfig.Validate())          _errorsAndWarnings.Add(CEConfig.GetPageErrorDetails());
            if (!NetworkConfig.Validate())     _errorsAndWarnings.Add(NetworkConfig.GetPageErrorDetails());

            return !HasErrorsOrWarnings();
        }


        internal new bool HasErrorsOrWarnings() => PanelConfig.HasErrorsOrWarnings()
                                                || Loop1Config.HasErrorsOrWarnings()
                                                || Loop2Config.HasErrorsOrWarnings()
                                                || DeviceNamesConfig.HasErrorsOrWarnings()
                                                || ZoneConfig.HasErrorsOrWarnings()
                                                || ZonePanelConfig.HasErrorsOrWarnings()
                                                || SetConfig.HasErrorsOrWarnings()
                                                || GroupConfig.HasErrorsOrWarnings()
                                                || CEConfig.HasErrorsOrWarnings()
                                                || NetworkConfig.HasErrorsOrWarnings();

        internal new bool HasErrors() => PanelConfig.HasErrors()
                                      || Loop1Config.HasErrors()
                                      || Loop2Config.HasErrors()
                                      || DeviceNamesConfig.HasErrors()
                                      || ZoneConfig.HasErrors()
                                      || ZonePanelConfig.HasErrors()
                                      || SetConfig.HasErrors()
                                      || GroupConfig.HasErrors()
                                      || CEConfig.HasErrors()
                                      || NetworkConfig.HasErrors();

        internal new bool HasWarnings() => PanelConfig.HasWarnings()
                                        || Loop1Config.HasWarnings()
                                        || Loop2Config.HasWarnings()
                                        || DeviceNamesConfig.HasWarnings()
                                        || ZoneConfig.HasWarnings()
                                        || ZonePanelConfig.HasWarnings()
                                        || SetConfig.HasWarnings()
                                        || GroupConfig.HasWarnings()
                                        || CEConfig.HasWarnings()
                                        || NetworkConfig.HasWarnings();
    }
}