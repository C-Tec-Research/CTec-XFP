using System.Windows;
using System.Windows.Media;

namespace Xfp.UI
{
    public class Styles : CTecControls.UI.Styles
    {
        private static SolidColorBrush _alarmAlertBrush      = Application.Current.FindResource("AlarmAlertBrush") as SolidColorBrush;
        private static SolidColorBrush _alarmEvacBrush       = Application.Current.FindResource("AlarmEvacBrush") as SolidColorBrush;
        private static SolidColorBrush _alarmOffBrush        = Application.Current.FindResource("AlarmOffBrush") as SolidColorBrush;
        private static SolidColorBrush _alarmAlertPrintBrush = Application.Current.FindResource("AlarmAlertPrintBrush") as SolidColorBrush;
        private static SolidColorBrush _alarmEvacPrintBrush  = Application.Current.FindResource("AlarmEvacPrintBrush") as SolidColorBrush;
        private static SolidColorBrush _alarmOffPrintBrush   = Application.Current.FindResource("AlarmOffPrintBrush") as SolidColorBrush;
        
        private static SolidColorBrush _triggerPulsedBrush       = Application.Current.FindResource("TriggerPulsedBrush") as SolidColorBrush;
        private static SolidColorBrush _triggerContinuousBrush   = Application.Current.FindResource("TriggerContinuousBrush") as SolidColorBrush;
        private static SolidColorBrush _triggerDelayedBrush      = Application.Current.FindResource("TriggerDelayedBrush") as SolidColorBrush;
        private static SolidColorBrush _triggerNotTriggeredBrush = Application.Current.FindResource("TriggerNotTriggeredBrush") as SolidColorBrush;

        private static SolidColorBrush _columnSeparatorBrush       =  Application.Current.FindResource("ColumnSeparatorBrush") as SolidColorBrush;

        private static Style _alarmIconStyle = Application.Current.FindResource("AlarmIcon") as Style;

        public static SolidColorBrush AlarmAlertBrush            => _alarmAlertBrush;
        public static SolidColorBrush AlarmEvacBrush             => _alarmEvacBrush;
        public static SolidColorBrush AlarmOffBrush              => _alarmOffBrush;
        public static SolidColorBrush AlarmAlertPrintBrush       => _alarmAlertPrintBrush;
        public static SolidColorBrush AlarmEvacPrintBrush        => _alarmEvacPrintBrush;
        public static SolidColorBrush AlarmOffPrintBrush         => _alarmOffPrintBrush;
        public static SolidColorBrush TriggerPulsedBrush         => _triggerPulsedBrush;
        public static SolidColorBrush TriggerContinuousBrush     => _triggerContinuousBrush;
        public static SolidColorBrush TriggerDelayedBrush        => _triggerDelayedBrush;
        public static SolidColorBrush TriggerNotTriggeredBrush   => _triggerNotTriggeredBrush;
        public static SolidColorBrush ColumnSeparatorBrush       => _columnSeparatorBrush;

        public static Style AlarmIconStyle => _alarmIconStyle;
    }
}
