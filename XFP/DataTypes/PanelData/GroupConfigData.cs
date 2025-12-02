using System;
using System.Collections.Generic;
using CTecDevices.Protocol;
using Windows.ApplicationModel.SocialInfo;
using Windows.ApplicationModel.Store;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class GroupConfigData : ConfigData, IConfigData
    {
        public GroupConfigData()
        {
            PanelSounder1Group = 1;
            PanelSounder2Group = 2;
            IntermittentTone = 0;
            ContinuousTone   = 0;
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Group_Configuration);
        }

        internal GroupConfigData(GroupConfigData original) : this()
        {
            if (original is not null)
            {
                PanelSounder1Group = original.PanelSounder1Group;
                PanelSounder2Group = original.PanelSounder2Group; 
                IntermittentTone   = original.IntermittentTone;
                ContinuousTone     = original.ContinuousTone;
                ReSoundFunction    = original.ReSoundFunction;
                PhasedDelay        = original.PhasedDelay;
            }
        }


        internal const  int NumSounderGroups = 16;
        internal static int NumToneMessagePairs => DeviceTypes.CurrentProtocolIsXfpApollo ? 15 : 31;
        internal static readonly TimeSpan MaxPhasedDelay = new(0, 10, 0);

        public static bool IsValidGroup(int? group, bool allowNull) => !group.HasValue ? allowNull : group >= 0 && group <= NumSounderGroups;
        public static bool IsValidPanelSounderGroup(int group)      => group >= 0 && group <= NumSounderGroups;
        public static bool IsValidAlarmTone(int? tone)              => tone >= 0 && tone < NumToneMessagePairs;
        public static bool IsValidPhasedDelay(TimeSpan? delay)      => delay.HasValue && ((TimeSpan)delay).CompareTo(new(0, 0, 0)) >= 0 && ((TimeSpan)delay).CompareTo(MaxPhasedDelay) <= 0;

        
        public int PanelSounder1Group { get; set; }
        public int PanelSounder2Group { get; set; }
        public int IntermittentTone { get; set; }
        public int ContinuousTone { get; set; }
        public bool ReSoundFunction { get; set; }
        public TimeSpan PhasedDelay { get; set; }


        /// <summary>
        /// Returns an initialised GroupConfigData object.
        /// </summary>
        internal new static GroupConfigData InitialisedNew() => new GroupConfigData();


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not GroupConfigData od)
                return false;
            
            if (PanelSounder1Group != od.PanelSounder1Group
             || PanelSounder2Group != od.PanelSounder2Group
             || IntermittentTone   != od.IntermittentTone
             || ContinuousTone     != od.ContinuousTone
             || ReSoundFunction    != od.ReSoundFunction
             || PhasedDelay        != od.PhasedDelay)
                return false;
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            var sounder1GroupErr = !IsValidPanelSounderGroup(PanelSounder1Group);
            var sounder2GroupErr = !IsValidPanelSounderGroup(PanelSounder2Group);
            var evacToneErr      = !IsValidAlarmTone(ContinuousTone);
            var alertToneErr     =  DeviceTypes.CurrentProtocolIsXfpCast && !IsValidAlarmTone(IntermittentTone);
            var phasedDelayErr   = !IsValidPhasedDelay(PhasedDelay);

            if (sounder1GroupErr || sounder2GroupErr)
            {
                ConfigErrorPageItems groupErrs = new(0, Cultures.Resources.Panel_Sounder_Groups);
                if (sounder1GroupErr) groupErrs.ValidationCodes.Add(ValidationCodes.PanelSounder1SounderGroup);
                if (sounder2GroupErr) groupErrs.ValidationCodes.Add(ValidationCodes.PanelSounder2SounderGroup);
                _pageErrorOrWarningDetails.Items.Add(groupErrs);
            }

            if (evacToneErr || alertToneErr || phasedDelayErr)
            {
                ConfigErrorPageItems alarmErrs = new(0, Cultures.Resources.Alarms);
                if (evacToneErr)    alarmErrs.ValidationCodes.Add(ValidationCodes.EvacTone);
                if (alertToneErr)   alarmErrs.ValidationCodes.Add(ValidationCodes.AlertTone);
                if (phasedDelayErr) alarmErrs.ValidationCodes.Add(ValidationCodes.PhasedDelay);
                _pageErrorOrWarningDetails.Items.Add(alarmErrs);
            }   
            
            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
        

        public class SounderGroup
        {
            public SounderGroup() { Alarms = new(); }
            public SounderGroup(SounderGroup original)
            {
                Index = original.Index;
                Name = original.Name;
                Alarms = new();
                foreach (var g in original.Alarms)
                    Alarms.Add(g);
            }

            public int Index { get; set; }
            public string Name { get; set; }
            public List<AlarmTypes> Alarms { get; set; }
        }
    }
}
