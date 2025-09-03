using System.Reflection;

namespace Xfp
{
    public static class BuildInfo
    {     
        public static readonly CTecUtil.BuildInfo.BuildDetails Details = CTecUtil.BuildInfo.ParseProductVersionString(Assembly.GetExecutingAssembly());
    }
}
