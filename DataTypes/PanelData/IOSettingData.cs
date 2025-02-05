using CTecDevices;
using CTecDevices.Protocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace Xfp.DataTypes.PanelData
{
    public class IOSettingData : ConfigData
    {
        public IOSettingData(int index, int? deviceType)
        {
            Index = index;
            DeviceType = deviceType;
            InputOutput = DeviceTypes.DefaultIOOutputType(deviceType, index, DeviceTypes.CurrentProtocolType);
            //InputOutput = index switch { 0 => IOTypes.Input, 1 => IOTypes.Output, _ => IOTypes.NotUsed };
            //Channel = index == 2 ? 2 : 1;
            //ZoneGroupSet = index switch { 0 => 1, 1 => 1, _ => 0 };
            NameIndex = -1;
        }

        internal IOSettingData(IOSettingData original)
        {
            Index        = original.Index;
            DeviceType   = original.DeviceType;
            InputOutput  = original.InputOutput;
            Channel      = original.Channel;
            ZoneGroupSet = original.ZoneGroupSet;
            NameIndex    = original.NameIndex;
        }


        public int      Index { get; set; }
        public int?     DeviceType { private get; set; }
        public IOTypes  InputOutput { get; set; }
        public int?     Channel { get; set; } = 0;
        public int?     ZoneGroupSet { get; set; } = 1;
        public int      NameIndex { get; set; }

        public const int NumInputChannels  = 2;
        public const int NumOutputChannels = 3;


        //internal static IOSettingData InitialisedNew(int index) => new IOSettingData(index);


        internal bool Equals(IOSettingData otherData)
        {
            if (otherData is not IOSettingData od)
                return false;
            
            return od.Index == Index
                && od.DeviceType == DeviceType
                && od.InputOutput == InputOutput
                && od.Channel == Channel
                && od.ZoneGroupSet == ZoneGroupSet
                && od.NameIndex == NameIndex;
        }


        public override bool Validate()
        {
            _errorItems = new(Index, string.Format(Cultures.Resources.Subaddress_x, Index + 1));
                
            if (InputOutput < 0 || (int)InputOutput >= Enum.GetNames(typeof(IOTypes)).Length)
                _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOInputOutput);

            if (InputOutput != IOTypes.NotUsed)
            {
                if (Channel < 0 || Channel > (InputOutput == IOTypes.Input ? IOSettingData.NumInputChannels : IOSettingData.NumOutputChannels))
                    _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOChannel);
                
                if (InputOutput == IOTypes.Input)
                {
                    if (ZoneGroupSet < 0 || ZoneGroupSet > ZoneConfigData.NumZones)
                        _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOSet);
                }
                else if (DeviceTypes.IOOutputIsGrouped(DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    if (ZoneGroupSet < 0 || ZoneGroupSet > GroupConfigData.NumSounderGroups)
                        _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOSet);
                }
                else
                {
                    if (ZoneGroupSet < 0 || ZoneGroupSet > DeviceData.NumIOSets)
                        _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOSet);
                }
            }

            return _errorItems.ValidationCodes.Count == 0;
        }


        internal byte[] ToByteArray()
        {
            return new byte[] { 0 };
        }


        internal static DeviceData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex) => null;
    }
}
