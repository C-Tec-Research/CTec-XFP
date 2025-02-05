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

namespace Xfp.DataTypes.PanelData
{
    public class NameAndAddressData
    {
        internal NameAndAddressData() => Address = new();

        internal NameAndAddressData(NameAndAddressData original) : this()
        {
            Name = original.Name;
            foreach (var a in original.Address)
                Address.Add(a);
            Postcode = original.Postcode;
        }


        public string Name { get; set; }
        public List<string> Address { get; set; }
        public string Postcode { get; set; }


        /// <summary>
        /// Returns an initialised NameAndAddressData object.
        /// </summary>
        internal static NameAndAddressData InitialisedNew() => new NameAndAddressData { Address = new() { "", "", "", "" } };


        internal bool Equals(NameAndAddressData otherData)
        {
            if (otherData.Name          != Name
             || otherData.Postcode      != Postcode
             || otherData.Address.Count != Address.Count)
                return false;

            for (int i = 0; i < Address.Count; i++)
                if (otherData.Address[i] != Address[i])
                    return false;
            
            return true;
        }


        /// <summary>
        /// True if all address and postcode fields are empty or white space.
        /// </summary>
        internal bool AddressIsEmpty()
        {
            foreach (var a in Address)
                if (!string.IsNullOrWhiteSpace(a))
                    return false;
            return string.IsNullOrWhiteSpace(Postcode);
        }
    }
}
