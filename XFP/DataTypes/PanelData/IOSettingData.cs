using CTecDevices;
using CTecDevices.Protocol;
using System;
using System.Security.Policy;

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


        public static bool IsValidIOType(int ioType)                    => ioType >= 0 && ioType < Enum.GetNames(typeof(IOTypes)).Length;
        public static bool IsValidChannel(int? channel, IOTypes ioType) => channel.HasValue && channel >= 0 && channel <= (ioType == IOTypes.Input ? NumInputChannels : NumOutputChannels);


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

            if (!IsValidIOType((int)InputOutput))
                _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOInputOutput);

            if (InputOutput != IOTypes.NotUsed)
            {
                if (!IsValidChannel(Channel, InputOutput))
                    _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOChannel);
                
                if (InputOutput == IOTypes.Input)
                {
                    if (!ZoneConfigData.IsValidZone(ZoneGroupSet))
                        _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOSet);
                }
                else if (DeviceTypes.IOOutputIsGrouped(DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    if (!GroupConfigData.IsValidGroup(ZoneGroupSet))
                        _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOSet);
                }
                else
                {
                    if (!SetConfigData.isValidSet(ZoneGroupSet))
                        _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidIOSet);
                }
            }

            return _errorItems.ValidationCodes.Count == 0;
        }


        internal byte[] ToByteArray()
        {
            return [0];
        }


        internal static DeviceData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex) => null;
    }
}
