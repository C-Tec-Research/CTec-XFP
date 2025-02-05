using CTecDevices.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Markup;
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


            //foreach (var s in SetNames)
            //{
            //}

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
