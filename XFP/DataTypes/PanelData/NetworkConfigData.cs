using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class NetworkConfigData : ConfigData, IConfigData
    {
        public NetworkConfigData()
        {
            RepeaterSettings = new();
            //for (int i = 0; i < NetworkRepeaterData.NumRepeaters; i++)
            //    RepeaterSettings.Repeaters.Add(new() { Number = i + 1, Name = String.Format(Cultures.Resources.Panel_x, i) });

            PanelSettings = new();
            //for (int i = 0; i < NumPanelSettings; i++)
            //    PanelSettings.Add(new());
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Network_Configuration);
        }


        internal NetworkConfigData(NetworkConfigData original) : this()
        {
            if (original is not null)
            {
                PanelSettings = new();

                for (int i = 0; i < original.PanelSettings.Count; i++)
                    PanelSettings.Add(new(original.PanelSettings[i]));
            }
        }


        public const int NumPanelSettings = 8;


        public NetworkRepeaterData            RepeaterSettings { get; set; }
        public List<NetworkPanelSettingsData> PanelSettings { get; set; }


        [JsonIgnore] public XfpPanelData.PanelNameChangeHandler PanelNameChanged;


        /// <summary>
        /// Returns an initialised NetworkConfigData object.
        /// </summary>
        internal new static NetworkConfigData InitialisedNew()
        {
            var result = new NetworkConfigData();

            for (int i = 0; i < NetworkRepeaterData.NumRepeaters; i++)
            {
                NetworkRepeaterItemData repeater = new() { Index = i, Name = String.Format(Cultures.Resources.Panel_x, i + 1) };
                repeater.PanelNameChanged = new((index, name) => result.PanelNameChanged?.Invoke(index, name));
                result.RepeaterSettings.Repeaters.Add(repeater);
            }

            for (int i = 0; i < NumPanelSettings; i++)
                result.PanelSettings.Add(NetworkPanelSettingsData.InitialisedNew());
            return result;
        }

        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not NetworkConfigData od)
                return false;

            for (int i = 0; i < PanelSettings.Count; i++)
                if (PanelSettings[i].AcceptFaults       != od.PanelSettings[i].AcceptFaults
                 || PanelSettings[i].AcceptAlarms       != od.PanelSettings[i].AcceptAlarms
                 || PanelSettings[i].AcceptControls     != od.PanelSettings[i].AcceptControls
                 || PanelSettings[i].AcceptDisablements != od.PanelSettings[i].AcceptDisablements
                 || PanelSettings[i].AcceptOccupied     != od.PanelSettings[i].AcceptOccupied)
                return false;

            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            foreach (var r in RepeaterSettings.Repeaters)
            {
                if (!r.Validate())
                {
                    var epi = r.GetErrorItems();
                    var p = new ConfigErrorPageItems(r.Number, epi.Name);
                    foreach (var e in epi.ValidationCodes)
                        p.ValidationCodes.Add(e);
                    _pageErrorOrWarningDetails.Items.Add(p);
                }
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
