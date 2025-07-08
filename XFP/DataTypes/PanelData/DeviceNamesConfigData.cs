using CTecDevices.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Linq;
using Windows.UI.Composition.Interactions;
using Xfp.IO;
using Xfp.UI.Interfaces;
using Xfp.UI.Views.PanelTools;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Xfp.DataTypes.PanelData.XfpPanelData;

namespace Xfp.DataTypes.PanelData
{
    public class DeviceNamesConfigData : ConfigData, IConfigData
    {
        public DeviceNamesConfigData()
        {
            //#0 on the panel is 'No name allocated'
            DeviceNames = new();
            _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Device_Details);
        }

        public DeviceNamesConfigData(DeviceNamesConfigData original) : this()
        {
            if (original is not null)
            {
                var tmp = new string[original.DeviceNames.Count];
                original.DeviceNames.CopyTo(tmp);
                DeviceNames = [.. tmp];
            }
        }


        /// <summary>Number of chars in a device name</summary>
        public const int DeviceNameLength = 24;


        public List<string> DeviceNames { get; set; }


        /// <summary>Returns an initialised DeviceNamesConfigData object.</summary>
        public new static DeviceNamesConfigData InitialisedNew()
        {
            var data = new DeviceNamesConfigData();
            //'No name allocated' is always item 0
            data.Add(Cultures.Resources.No_Name_Allocated);
            return data;
        }



        /// <summary>
        /// Updates the names list for the specified index.  An new entry will be added if it's not present.<br/>
        /// NB: the returned index may be different from the original if the name already existed.<br/>
        /// NB2: item 0 cannot be changed as it's the 'No name allocated' entry.
        /// </summary>
        /// <returns>The index of the updated or inserted name.</returns>
        public int Update(int index, string value)
        {
            if (index > 0 && string.IsNullOrEmpty(value))
                return Remove(index);

            //if the new name is already in the list return that index
            var idx = GetNameIndex(value);
            if (idx > 0 && index != idx)
                return idx;

            //update existing name
            if (index > 0 && index < DeviceNames.Count)
            {
                DeviceNames[index] = value;
                return index;
            }

            //else add it
            return Add(value);
        }


        /// <summary>
        /// Adds the given value to the DeviceNames list.  The name will overwrite the first null entry, if any.<br/>
        /// NB: the returned index may be different from the original if a vacant slot was used.
        /// </summary>
        /// <returns>The index of the added name.</returns>
        public int Add(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            var emptySlot = DeviceNames.IndexOf(null);
            if (emptySlot > 0)
            {
                DeviceNames[emptySlot] = value;
                return emptySlot;
            }

            //no free slots so insert at end
            var newIdx = DeviceNames.Count;
            DeviceNames.Insert(newIdx, value);
            return newIdx;
        }


        /// <summary>
        /// 'Removes' the name at the specified index by setting it to null.
        /// </summary>
        /// <returns>Returns 0, i.e. pointing to the 'No name allocated' entry.</returns>
        public int Remove(int index)
        {
            if (ContainsIndex(index))
                DeviceNames[index] = null;
            return 0;
        }


        /// <summary>Returns True if the index is within the limits of the DeviceNames list</summary>
        public bool ContainsIndex(int index) => index >= 0 && index < DeviceNames.Count;


        /// <summary>Returns True if the DeviceNames list contains the given value.</summary>
        public bool ContainsValue(string value) => GetNameIndex(value) > 0;


        /// <summary>
        /// Returns the name at the specified index within the DeviceNames list, or an empty string if not present.<br/>
        /// NB: returns an empty string for index 0 - this is the 'No name allocated' entry, which is essentially hard-coded on the panel but we don't display it.
        /// </summary>
        public string GetName(int index) => (index > 0 && index < DeviceNames.Count) ? DeviceNames[index] : "";


        /// <summary>Finds the index of the given value within the DeviceNames list, or 0 if not present.</summary>
        public int GetNameIndex(string value)
        {
            for (int i = 0; i < DeviceNames.Count; i++)
                if (DeviceNames[i] == value)
                    return i;
            return 0;
        }

        
        [JsonIgnore]
        public string TotalNamesUsedText
        {
            get
            {
                var result = 0;
                foreach (var n in DeviceNames)
                    if (!string.IsNullOrEmpty(n))
                        result++;
                return result.ToString();
            }
        }

        [JsonIgnore]
        public int BytesUsed
        {
            get
            {
                var result = 0;
                foreach (var n in DeviceNames)
                    if (!string.IsNullOrEmpty(n))
                        result += n.Length + 1;
                return result;
            }
        }

        [JsonIgnore]
        public int BytesRemaining => 0x2000 - BytesUsed;


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not DeviceNamesConfigData od)
                return false;
            if (od.DeviceNames.Count != DeviceNames.Count)
                return false;
            for (int i = 0; i < od.DeviceNames.Count; i++)
                try
                {
                    if (od.DeviceNames[i] != DeviceNames[i])
                        return false;
                } catch { return false; }
            return true;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            //foreach (var dn in DeviceNames)
            //    if (string.IsNullOrWhiteSpace(dn.Value.Name))
            //        _pageErrorOrWarningDetails.Items.Add(new(Cultures.Resources.Number_Symbol + (dn.Value.Index - 1), dn.Value.Index, ValidationCodes.DeviceNamesDataBlankEntry));
            //    else if (dn.Value.Name.Length > DeviceNameLength)
            //        _pageErrorOrWarningDetails.Items.Add(new(Cultures.Resources.Number_Symbol + (dn.Value.Index - 1), dn.Value.Index, ValidationCodes.DeviceNamesDataTooLong));

            if (BytesRemaining < 0)
                _pageErrorOrWarningDetails.Items.Add(new(Cultures.Resources.Device_Names_Data_Limit, 0, ValidationCodes.DeviceNamesTooManyBytes));

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
