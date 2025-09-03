namespace Xfp.IO
{
    internal class XfpCommandCodes
    {
        /// <summary>Start byte of command to the panel</summary>
        internal const byte CommandStartByte = 0x01;

        internal const byte Ack = 0x06;
        internal const byte Nak = 0x15;


        internal const byte BaseFunction = 0x40;

        internal const byte LoggingPing = 0x40;
        internal const byte StandardPing = 0x41;
        internal const byte PollNvmLink = 0x42;
        
        internal const byte RequestPointName = BaseFunction + 0x03;             //device name
        internal const byte SetPointName = BaseFunction + 0x04;

        internal const byte RequestZoneName = BaseFunction + 0x05;
        internal const byte SetZoneName = BaseFunction + 0x06;

        internal const byte RequestMaintName = BaseFunction + 0x07;
        internal const byte SetMaintName = BaseFunction + 0x08;

        internal const byte RequestPData = BaseFunction + 0x09;
        internal const byte SetPData = BaseFunction + 0x0a;

        internal const byte RequestMaintDate = BaseFunction + 0x0b;
        internal const byte SetMaintDate = BaseFunction + 0x0c;

        internal const byte RequestZGroup = BaseFunction + 0x0d;
        internal const byte SetZGroup = BaseFunction + 0x0e;

        internal const byte RequestAL2Code = BaseFunction + 0x0f;
        internal const byte SetAL2Code = BaseFunction + 0x10;

        internal const byte RequestAL3Code = BaseFunction + 0x11;
        internal const byte SetAL3Code = BaseFunction + 0x12;

        internal const byte RequestPhasedSettings = BaseFunction + 0x13;
        internal const byte SetPhasedSettings = BaseFunction + 0x14;

        internal const byte SetDateTime = BaseFunction + 0x15;

        internal const byte RequestLoopType = BaseFunction + 0x16;              //panel device protocol + num loops
        internal const byte SetLoopType = BaseFunction + 0x17;                  //does nothing on panel (Cast)

        //internal const byte RequestFaultLockoutTime = BaseFunction + 0x18;    //hardcoded 0x00
        //internal const byte SetFaultLockoutTime = BaseFunction + 0x19;

        internal const byte RequestZoneTimers = BaseFunction + 0x1a;
        internal const byte SetZoneTimers = BaseFunction + 0x1b;

        internal const byte RequestMainVersion = BaseFunction + 0x1c;           //panel firmware version

        internal const byte RequestRepeaterName = BaseFunction + 0x1d;
        internal const byte SetRepeaterName = BaseFunction + 0x1e;

        //internal const byte RequesOutputName = BaseFunction + 0x1f;           //hardcoded "ABC"
        //internal const byte SetOutputName = BaseFunction + 0x20;

        //internal const byte RequestRepeaterFitted = BaseFunction + 0x21;      //hardcoded 0xff
        //internal const byte SetRepeaterFitted = BaseFunction + 0x22;

        //internal const byte RequestOutputFitted = BaseFunction + 0x23;        //hardcoded 0xff
        //internal const byte SetOutputFitted = BaseFunction + 0x24;

        //internal const byte RequestSegmentNum = BaseFunction + 0x25;          //hardcoded 0xff
        //internal const byte SetSegmentNum = BaseFunction + 0x26;

        //internal const byte RequestMainName = BaseFunction + 0x27;            //hardcoded "ABC"
        //internal const byte SetMainName = BaseFunction + 0x28;

        internal const byte RequestZSet = BaseFunction + 0x29;
        internal const byte SetZSet = BaseFunction + 0x2a;

        //internal const byte RequestNonFireFlags = BaseFunction + 0x2b;        //not used (Cast)
        //internal const byte SetNonFireFlags = BaseFunction + 0x2c;

        internal const byte RequestQuiescentName = BaseFunction + 0x2d;
        internal const byte SetQuiescentName = BaseFunction + 0x2e;

        internal const byte RequestDayNight = BaseFunction + 0x2f;
        internal const byte SetDayNight = BaseFunction + 0x30;

        internal const byte RequestEvent = BaseFunction + 0x31;

        internal const byte RequestCEEvent = BaseFunction + 0x33;
        internal const byte SetCEEvent = BaseFunction + 0x34;

        internal const byte RequestNetPanelData = BaseFunction + 0x35;
        internal const byte SetNetPanelData = BaseFunction + 0x36;

        //internal const byte RequestTextString = BaseFunction + 0x37;          //not used (intended for language variant strings)
        //internal const byte SetTextString = BaseFunction + 0x38;
    }
}
