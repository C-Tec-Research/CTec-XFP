using CTecUtil.Config;

namespace Xfp.Config
{
    public class XfpApplicationConfigData : ApplicationConfigData
    {
        public XfpApplicationConfigData() => Protocol = CTecDevices.Enums.ObjectTypeName(CTecDevices.ObjectTypes.XfpCast);
        public WindowSizeParams MonitorWindow { get; set; }
    }
}
