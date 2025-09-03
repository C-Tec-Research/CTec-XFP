using System;

namespace Xfp.DataTypes.PanelData
{
    public class ZoneDependency : ConfigData
    {
        public ZoneDependency() : base()
        {
            DependencyOption = ZoneDependencyOptions.Normal;
            DetectorReset    = new(0, 1, 0);
            AlarmReset       = new(0, 5, 0);
        }

        public ZoneDependency(ZoneDependency original) : base()
        {
            DependencyOption = original.DependencyOption;
            DetectorReset    = original.DetectorReset;
            AlarmReset       = original.AlarmReset;
        }

        public ZoneDependencyOptions DependencyOption { get; set; }
        public TimeSpan              DetectorReset    { get; set; }
        public TimeSpan              AlarmReset       { get; set; }
        
        public bool ShowDetectorReset => DependencyOption == ZoneDependencyOptions.A;
        public bool ShowAlarmReset    => DependencyOption switch { ZoneDependencyOptions.A or ZoneDependencyOptions.B => true, _ => false };


        public new static ZoneDependency InitialisedNew() => new ZoneDependency() { DependencyOption = ZoneDependencyOptions.Normal };


        public override bool Equals(ConfigData otherData) => otherData is ZoneDependency od
                                                                    && DependencyOption == od.DependencyOption
                                                                    && DetectorReset == od.DetectorReset
                                                                    && AlarmReset == AlarmReset;

        public override bool Validate()
        {
            _errorItems = new(0, Cultures.Resources.Dependency_Options);

            if ((int)DependencyOption < 0 || (int)DependencyOption >= Enum.GetNames(typeof(ZoneDependencyOptions)).Length)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataDayOptionInvalid);

            if (ShowDetectorReset && DetectorReset.CompareTo(ZoneConfigData.MaxDetectorReset) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataDayDetectorResetTooLong);

            if (ShowAlarmReset && AlarmReset.CompareTo(ZoneConfigData.MaxAlarmReset) > 0)
                _errorItems.ValidationCodes.Add(ValidationCodes.ZoneConfigDataDayAlarmResetTooLong);

            return _errorItems.ValidationCodes.Count == 0;
        }

        //public byte[] ToByteArray()
        //{
        //    var result = new byte[1];                
        //    return result;
        //}

        //public static ZoneData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        //{
        //    ZoneData result = null;
        //    return result;
        //}
    }
}
