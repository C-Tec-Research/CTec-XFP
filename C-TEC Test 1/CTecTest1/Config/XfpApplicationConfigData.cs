using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using CTecUtil.Config;
using CTecUtil.IO;

namespace Xfp.Config
{
    public class XfpApplicationConfigData : ApplicationConfigData
    {
        public XfpApplicationConfigData() => Protocol = CTecDevices.Enums.ObjectTypeName(CTecDevices.ObjectTypes.XfpCast);
        public WindowSizeParams MonitorWindow { get; set; }
    }
}
