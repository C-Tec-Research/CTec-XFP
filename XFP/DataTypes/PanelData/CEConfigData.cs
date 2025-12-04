using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xfp.UI.Interfaces;
using CTecUtil.StandardPanelDataTypes;

namespace Xfp.DataTypes.PanelData
{
    public partial class CEConfigData : ConfigData, IConfigData
    {
        public CEConfigData()
        {
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_C_And_E_Configuration);
        }

        public CEConfigData(CEConfigData original) : this()
        {
            Events = new();
            for (int i = 0; i < NumEvents; i++)
            {
                if (original is not null)
                    Events.Add(new(original.Events[i]));
                else
                    Events.Add(new() { Index = i });
            }

            TimerEventTimes = new();
            for (int i = 0; i < NumEvents; i++)
            {
                if (original is not null)
                    TimerEventTimes.Add(new(original.TimerEventTimes[i].Hours, original.TimerEventTimes[i].Minutes, original.TimerEventTimes[i].Seconds));
                else
                    TimerEventTimes.Add(TimeOfDay.Midnight);
            }
        }


        [JsonIgnore]
        /// <summary>Number of zones in the data</summary>
        public const int NumEvents = 16;
        [JsonIgnore]
        public const int NumConditions = 6;

        public static bool IsValidTimerEventTime(TimeSpan? time) => time.HasValue && time?.CompareTo(new(0, 0, 59)) > 0 && time?.CompareTo(new(0, 23, 59, 0, 0)) <= 0;


        public List<CEEvent> Events { get; set; }
        public List<TimeSpan> TimerEventTimes { get; set; }


        /// <summary>
        /// Returns an initialised GroupData object.
        /// </summary>
        internal new static CEConfigData InitialisedNew()
        {
            var result = new CEConfigData();

            result.Events = new();
            for (int i = 0; i < NumEvents; i++)
                result.Events.Add(new() { Index = i });

            result.TimerEventTimes = new();
            for (int i = 0; i < NumEvents; i++)
                result.TimerEventTimes.Add(TimeOfDay.Midnight);

            result.Events[0].ActionType = CEActionTypes.SounderEvac;
            result.Events[0].ActionParam = 0;
            result.Events[0].TriggerType = CETriggerTypes.PanelInput;
            result.Events[0].TriggerParam = 0;
            result.Events[0].ResetType = CETriggerTypes.PanelInput;
            result.Events[0].ResetParam = 0;
            result.Events[0].ResetCondition = false;

            result.Events[1].ActionType = CEActionTypes.SounderAlert;
            result.Events[1].ActionParam = 0;
            result.Events[1].TriggerType = CETriggerTypes.PanelInput;
            result.Events[1].TriggerParam = 1;
            result.Events[1].ResetType = CETriggerTypes.PanelInput;
            result.Events[1].ResetParam = 1;
            result.Events[1].ResetCondition = false;

            //default all the trigger conditions to True
            foreach (var e in result.Events)
                e.TriggerCondition = true;

            return result;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not CEConfigData od)
                return false;
            if (od.Events.Count != Events.Count)
                return false;
            for (int i = 0; i < NumEvents; i++)
                if (od.Events[i].ActionType       != Events[i].ActionType
                 || od.Events[i].ActionParam      != Events[i].ActionParam
                 || od.Events[i].TriggerType      != Events[i].TriggerType
                 || od.Events[i].TriggerParam     != Events[i].TriggerParam
                 || od.Events[i].TriggerParam2    != Events[i].TriggerParam2
                 || od.Events[i].TriggerCondition != Events[i].TriggerCondition
                 || od.Events[i].ResetType        != Events[i].ResetType
                 || od.Events[i].ResetParam       != Events[i].ResetParam
                 || od.Events[i].ResetParam2      != Events[i].ResetParam2
                 || od.Events[i].ResetCondition   != Events[i].ResetCondition)
                    return false;
            for (int i = 0; i < NumEvents; i++)
                if (od.TimerEventTimes[i] != TimerEventTimes[i])
                    return false;
            return true;
        }


        public override bool Validate()
        {
            try
            {
                _pageErrorOrWarningDetails.Items.Clear();

                foreach (var e in Events)
                {
                    if (!e.Validate())
                    {
                        var epi = e.GetErrorItems();
                        var p = new ConfigErrorPageItems(e.Number, epi.Name);
                        foreach (var vc in epi.ValidationCodes)
                            p.ValidationCodes.Add(vc);
                        _pageErrorOrWarningDetails.Items.Add(p);
                    }
                }
            }
            catch { }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }


        public byte[] ToByteArray()
        {
            var result = new byte[1];                
            return result;
        }
    }
}
