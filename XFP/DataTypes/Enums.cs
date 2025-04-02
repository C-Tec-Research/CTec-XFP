namespace Xfp.DataTypes
{
    public enum DataSources
    {
        Default,
        Device,
        File
    };


    public enum LevelOfCheck
    {
        Full,
        WriteableDataOnly,
        FileChanges
    }

    //public enum IOTypes
    //{
    //    Input,
    //    Output,
    //    NotUsed,
    //}

    public enum AccessLevels
    {
        User = 0,
        Engineer = 1
    }

    //public enum CallLevels
    //{
    //    Reset = 0,
    //    Call = 1,
    //    Presence = 2,
    //    Assist = 3,
    //    Emergency = 4,
    //    Attack = 5,
    //    Cardiac = 6
    //}

    public enum DayNight
    {
        Day   = 0,
        Night = 1
    }

    //public enum EquationTypes
    //{
    //    Area,
    //    Group,
    //    Device
    //}

    //public enum EquationParseResult
    //{
    //    Ok,
    //    InvalidItem,
    //    InvalidToken,
    //    ParseError,
    //    RangeError,
    //    TooManyItems,
    //}

    public enum CEActionTypes
    {
        None = 0,
        TriggerLoop1Device = 10,
        TriggerLoop2Device = 20,
        PanelRelay = 30,
        SounderAlert = 40,
        SounderEvac = 50,
        SilencePanel = 60,
        ResetPanel = 70,
        ZoneDisable = 80,
        OutputDisable = 90,
        TriggerOutputSet = 100,
        SetToOccupied = 110,
        AbstractEvent = 120,
        MutePanel = 130,
        SetToUnoccupied = 140,
        OutputDelaysDisable = 150,
        GroupDisable = 160,
        Loop1DeviceDisable = 170,
        Loop2DeviceDisable = 180,
        EndPhasedEvacuation = 190,
        EndZoneDelays = 200,
        PutZoneIntoAlarm = 210,
        TriggerNetworkEvent = 220,
        TriggerBeacons = 230,
    }

    public enum CETriggerTypes
    {
        None = 0,
        Loop1DeviceTriggered = 5,
        Loop2DeviceTriggered = 10,
        ZoneOrPanelInFire = 15,
        AnyZoneInFire = 20,
        PanelSilenced = 25,
        PanelReset = 30,
        AnyPrealarm = 35,
        Loop1DevicePrealarm = 40,
        Loop2DevicePrealarm = 45,
        AnyFault = 50,
        PanelInput = 55,
        OtherEventTriggered = 60,
        EventAnd = 65,
        PanelOccupied = 70,
        PanelUnoccupied = 75,
        TimerEventTn = 80,
        ZoneHasDeviceInAlarm = 85,
        AnyDeviceInAlarm = 90,
        AnyRemotePanelInFire = 95,
        AnyDisablement = 100,
        MoreThanOneAlarm = 105,
        MoreThanOneZoneInAlarm = 110,
        NetworkEventTriggered = 115,
        AnyDwellingAndCommunal = 120,
        ZoneAnd = 125,
        NotSet = -1,
    }

    public enum AlarmTypes
    {
        Off,
        Alert,
        Evacuate,
    }

    public enum SetTriggerTypes
    {
        NotTriggered,
        Pulsed,
        Continuous,
        Delayed,
    }

    public enum ZoneDependencyOptions
    {
        NotSet,
        A,
        B,
        C,
        Normal,
        Investigation,
        Dwelling,
    }


    public enum ValidationCodes
    {
        Ok = 0,
        
        // - DeviceConfigData config errors in 100s
        DeviceConfigDataInvalidDeviceType = 100,
        DeviceConfigDataInvalidGroup = 102,
        DeviceConfigDataInvalidZone = 103,
        DeviceConfigDataDeviceNameTooLong = 104,
        DeviceConfigDataInvalidSounderGroup = 105,
        DeviceConfigDataInvalidIOInputOutput = 110,
        DeviceConfigDataInvalidIOChannel = 111,
        DeviceConfigDataInvalidIOZone = 112,
        DeviceConfigDataInvalidIOSet = 113,
        DeviceConfigDataIODescriptionTooLong = 114,
        DeviceConfigDataInvalidDaySensitivity = 115,
        DeviceConfigDataInvalidNightSensitivity = 116,
        DeviceConfigDataInvalidDayVolume = 117,
        DeviceConfigDataInvalidNightVolume = 118,
        DeviceConfigDataInvalidDayMode = 119,
        DeviceConfigDataInvalidNightMode = 120,

        // - DeviceNamesConfigData
        DeviceNamesTooManyBytes = 130,
        DeviceNamesDataTooLong = 131,
        DeviceNamesDataBlankEntry = 132,

        // - ZoneConfigData config errors in 200s
        ZoneConfigDataInvalidZoneNum = 200,
        ZoneConfigDataZoneNameTooLong = 201,
        ZoneConfigDataSounderDelayTooLong = 202,
        ZoneConfigDataRelay1DelayTooLong = 203,
        ZoneConfigDataRelay2DelayTooLong = 204,
        ZoneConfigDataOutputDelayTooLong = 205,
        ZoneConfigDataTotalDelayTooLong = 206,
        ZoneConfigDataDayOptionInvalid = 207,
        ZoneConfigDataDayDetectorResetTooLong = 208,
        ZoneConfigDataDayAlarmResetTooLong = 209,
        ZoneConfigDataNightOptionInvalid = 210,
        ZoneConfigDataNightDetectorResetTooLong = 211,
        ZoneConfigDataNightAlarmResetTooLong = 212,

        // - ZonePanelConfigData config errors in 300s
        ZonePanelConfigDataInvalidPanelNum = 300,
        ZonePanelConfigDataPanelNameTooLong = 301,
        ZonePanelConfigDataSounderDelayTooLong = 302,
        ZonePanelConfigDataRelay1DelayTooLong = 303,
        ZonePanelConfigDataRelay2DelayTooLong = 304,
        ZonePanelConfigDataOutputDelayTooLong = 305,
        ZonePanelConfigDataTotalDelayTooLong = 306,
        
        // - GroupConfigData config errors in 400s
        
        // - SetConfigData config errors in 500s
        SetConfigDelayTimerTooLong = 500,
        
        // - SiteConfigData config errors in 600s
        SiteConfigNoSystemNameWarning = 600,
        SiteConfigNoClientName = 602,
        SiteConfigNoClientAddress = 603,
        SiteConfigNoClientTel = 604,
        SiteConfigNoInstallerName = 610,
        SiteConfigNoInstallerAddress = 611,
        SiteConfigNoInstallerTel = 612,
        SiteConfigNoEngineerName = 613,
        SiteConfigNoInstallDate = 614,
        SiteConfigNoCommissionDate = 615,
        
        // - SitePanelData config errors 650+
        SiteConfigQuiescentStringBlank = 650,
        SiteConfigQuiescentStringTooLong = 651,
        SiteConfigMaintenanceStringBlank = 652,
        SiteConfigMaintenanceStringTooLong = 653,
        SiteConfigAL2CodeError = 660,
        SiteConfigAL3CodeError = 661,

        // - CECConfigData config errors in 700s
        CEActionLoop1DeviceNotSet = 700,
        CEActionLoop2DeviceNotSet = 701,
        CEActionDeviceUnassigned = 702,
        CEActionPanelRelayNotSet = 703,
        CEActionGroupNotSet = 704,
        CEActionSounderGroupNotSet = 705,
        CEActionBeaconGroupNotSet = 706,
        CEActionZoneNotSet = 707,
        CEActionOutputSetNotSet = 708,
        CEActionNetworkEventNotSet = 709,
        CETriggerTypeNotSet = 720,
        CETriggerLoop1DeviceNotSet = 721,
        CETriggerLoop2DeviceNotSet = 722,
        CETriggerDeviceUnassigned = 723,
        CETriggerZoneOrPanelNotSet = 724,
        CETriggerPanelInputNotSet = 725,
        CETriggerEventNotSet = 726,
        CETriggerEvent2NotSet = 727,
        CETriggerTimerNotSet = 728,
        CETriggerNetworkEventNotSet = 729,
        CETriggerZoneNotSet = 730,
        CETriggerZone2NotSet = 731,
        CETriggerConditionNotSet = 732,
        CEResetTypeNotSet = 740,
        CEResetLoop1DeviceNotSet = 741,
        CEResetLoop2DeviceNotSet = 742,
        CEResetDeviceUnassigned = 743,
        CEResetZoneOrPanelNotSet = 744,
        CEResetPanelInputNotSet = 745,
        CEResetEventNotSet = 746,
        CEResetEvent2NotSet = 747,
        CEResetTimerNotSet = 748,
        CEResetNetworkEventNotSet = 749,
        CEResetZoneNotSet = 750,
        CEResetZone2NotSet = 751,
        CEResetConditionNotSet = 752,

        // - NetworkConfigData config errors in 800s
        NetworkConfigInvalidPanelName = 800,
        NetworkConfigNoPanelLocation = 801,

        // - miscellaneous error in 900s
    }

    public enum ErrorLevels
    {
        OK,
        Warning,
        Error
    }
}
