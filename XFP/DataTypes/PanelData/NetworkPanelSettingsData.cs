using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public class NetworkPanelSettingsData : ConfigData
    {
        public NetworkPanelSettingsData() { }

        internal NetworkPanelSettingsData(NetworkPanelSettingsData original) : this()
        {
            if (original is not null)
            {
                AcceptFaults       = original.AcceptFaults;
                AcceptAlarms       = original.AcceptAlarms;
                AcceptControls     = original.AcceptControls;
                AcceptDisablements = original.AcceptDisablements;
                AcceptOccupied     = original.AcceptOccupied;
            }
        }


        public bool AcceptFaults { get; set; }
        public bool AcceptAlarms { get; set; }
        public bool AcceptControls { get; set; }
        public bool AcceptDisablements { get; set; }
        public bool AcceptOccupied { get; set; }


        /// <summary>
        /// Returns an initialised NetworkConfigData object.
        /// </summary>
        internal new static NetworkPanelSettingsData InitialisedNew()
        {
            var data = new NetworkPanelSettingsData();
            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not NetworkPanelSettingsData od)
                return false;

            if (AcceptFaults       != od.AcceptFaults
             || AcceptAlarms       != od.AcceptAlarms
             || AcceptControls     != od.AcceptControls
             || AcceptDisablements != od.AcceptDisablements
             || AcceptOccupied     != od.AcceptOccupied)
                return false;

            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
