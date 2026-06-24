using System.Reflection;

namespace Xfp
{
    public static class BuildInfo
    {     
        public static readonly CTecUtil.BuildInfo.BuildDetails BuildDetails = CTecUtil.BuildInfo.ParseProductVersionString(Assembly.GetExecutingAssembly());

        public static readonly bool IsBetaRelease = false;
    }
}
