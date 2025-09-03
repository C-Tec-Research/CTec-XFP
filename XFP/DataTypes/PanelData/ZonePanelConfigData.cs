using System.Collections.Generic;
using Newtonsoft.Json;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public class ZonePanelConfigData : ConfigData, IConfigData
    {
        public ZonePanelConfigData()
        {
            Panels = new();
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Zone_Configuration);
        }

        public ZonePanelConfigData(ZonePanelConfigData original) : this()
        {
            if (original is not null)
            {
                foreach (var p in original.Panels)
                    Panels.Add(new ZonePanelData(p));
            }
        }


        [JsonIgnore] public XfpPanelData.PanelNameChangeHandler PanelNameChanged;


        /// <summary>Number of zones in the data</summary>
        public const int NumZonePanels = 8;

        public List<ZonePanelData> Panels { get; set; }


        /// <summary>
        /// Returns an initialised ZoneConfigData object.
        /// </summary>
        public new static ZonePanelConfigData InitialisedNew()
        {
            var data = new ZonePanelConfigData();
            data.Panels = new();
            for (int i = 0; i < ZonePanelConfigData.NumZonePanels; i++)
            {
                var panel = ZonePanelData.InitialisedNew(i);
                panel.PanelNameChanged = new((index, name) => data.PanelNameChanged?.Invoke(index, name));
                data.Panels.Add(panel);
            }
            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not ZonePanelConfigData od)
                return false;
            if (od.Panels.Count != Panels.Count)
                return false;
            for (int i = 0; i < Panels.Count; i++)
                if (!od.Panels[i].Equals(Panels[i]))
                    return false;
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            foreach (var p in Panels)
            {
                if (!p.Validate())
                {
                    var epi = p.GetErrorItems();
                    var pi = new ConfigErrorPageItems(p.Index + 1, epi.Name);
                    foreach (var e in epi.ValidationCodes)
                        pi.ValidationCodes.Add((ValidationCodes)e);
                    _pageErrorOrWarningDetails.Items.Add(pi);
                }
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
