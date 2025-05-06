using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class SetConfigData : ConfigData, IConfigData
    {
        public SetConfigData()
        {
            Sets = new();
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Set_Configuration);
        }

        public SetConfigData(SetConfigData original) : this()
        {
            if (original is not null)
            {
                foreach (var s in original.Sets)
                    Sets.Add(new SetData(s));
                DelayTimer = original.DelayTimer;
            }
        }


        public const int NumOutputSetTriggers  = 16;
        public const int NumPanelRelayTriggers = 2;
        public const int DefaultDelayTimerSeconds = 300;


        public List<SetData> Sets { get; set; }
        public TimeSpan DelayTimer { get; set; }


        public new static SetConfigData InitialisedNew()
        {
            var data = new SetConfigData();
            data.Sets = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                data.Sets.Add(SetData.InitialisedNew(i));
            for (int i = 0; i < ZoneConfigData.NumPanels; i++)
                data.Sets.Add(SetData.InitialisedNew(i, true));
            data.DelayTimer = new(0, DefaultDelayTimerSeconds / 60, DefaultDelayTimerSeconds % 60);
            return data;
        }
        
        
        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not SetConfigData od)
                return false;
            if (od.Sets.Count != Sets.Count
              || od.DelayTimer != DelayTimer)
                return false;
            for (int i = 0; i < Sets.Count; i++)
                if (!od.Sets[i].Equals(Sets[i]))
                    return false;
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            if (DelayTimer.CompareTo(ZoneConfigData.MaxSetDelay) > 0)
                _pageErrorOrWarningDetails.Items.Add(new(0, Cultures.Resources.Delay_Time) { ValidationCode = ValidationCodes.SetConfigDelayTimerTooLong });

            foreach (var s in Sets)
            {
                if (!s.Validate())
                {
                    var epi = s.GetErrorItems();
                    var p = new ConfigErrorPageItems(s.Number, epi.Name);
                    foreach (var e in epi.ValidationCodes)
                        p.ValidationCodes.Add(e);
                    _pageErrorOrWarningDetails.Items.Add(p);
                }
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }

    }
}
