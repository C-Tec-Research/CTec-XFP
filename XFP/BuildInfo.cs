using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xfp
{
    public static class BuildInfo
    {     
        public static readonly CTecUtil.BuildInfo.BuildDetails Details = CTecUtil.BuildInfo.ParseProductVersionString(Assembly.GetExecutingAssembly());
    }
}
