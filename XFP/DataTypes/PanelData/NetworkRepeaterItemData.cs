using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public class NetworkRepeaterItemData : ConfigData, IConfigData
    {
        public NetworkRepeaterItemData() { }

        public NetworkRepeaterItemData(NetworkRepeaterItemData original) : this()
        {
            Index    = original.Index;
            Name     = original.Name;
            Fitted   = original.Fitted;
            Location = original.Location;
        }

        
        [JsonIgnore] public int    Index { get; set; }
        [JsonIgnore] public int    Number => Index + 1;
        [JsonIgnore] public string Name { get; set; }
        [JsonIgnore] public string DefaultName => string.Format(Cultures.Resources.Panel_x, Number);
        [JsonIgnore] public string DisplayName => string.IsNullOrEmpty(Name) ? DefaultName : Name == DefaultName ? Name : DefaultName + " - " + Name;

        public bool   Fitted { get; set; }
        public string Location { get; set; }


        internal void SetPanelName(string name) { Name = name; PanelNameChanged?.Invoke(Index, name); }


        [JsonIgnore] public XfpPanelData.PanelNameChangeHandler PanelNameChanged;


        public new static NetworkRepeaterItemData InitialisedNew()
        {
            return new NetworkRepeaterItemData();
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not NetworkRepeaterItemData od)
                return false;

            if (Number   != od.Number
             || Name     != od.Name
             || Fitted   != od.Fitted
             || Location != Location)
                return false;
            
            return true;
        }


        public override bool Validate()
        {
            _errorItems = new(Number, string.Format(string.Format(Cultures.Resources.Panel_x, Number)));

            if (Name is null || Name.Length > ZoneConfigData.MaxNameLength)
                _errorItems.ValidationCodes.Add(ValidationCodes.NetworkConfigInvalidPanelName);

            if (Fitted && string.IsNullOrWhiteSpace(Location))
                _errorItems.ValidationCodes.Add(ValidationCodes.NetworkConfigNoPanelLocation);

            return _errorItems.ValidationCodes.Count == 0;
        }

        public static NetworkRepeaterItemData ParseFitted(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        {
            if (responseTypeCheck is not null && !responseTypeCheck(data))
                return null;

            var result = new NetworkRepeaterItemData() { Index = requestedIndex??0 };
            try
            {
                result.Fitted = data[2] > 0;
            }
            catch { }
            return result;
        }
        
        public static List<NetworkPanelSettingsData> ParseNetworkPanelData(byte[] data, Func<byte[], bool> responseTypeCheck)
        {
            if (responseTypeCheck is not null && !responseTypeCheck(data))
                return null;

            var result = new List<NetworkPanelSettingsData>();
            try
            {
                //result.Fitted = data[2] > 0;
            }
            catch { }
            return result;
        }
    }
}
