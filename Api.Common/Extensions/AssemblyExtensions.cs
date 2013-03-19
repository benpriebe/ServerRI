using System.Reflection;

namespace Api.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static string ToVersion(this Assembly assembly)
        {
            var ver = assembly.GetName().Version;
            return ver.ToString();
        }

        public static bool IsVersionCompatible(this Assembly assembly, string otherVersion)
        {
            // according to http://semver.org/ versions are compatible so long as they are part of the same major version
            var ver = new System.Version(otherVersion);
            return assembly.GetName().Version.Major == ver.Major;
        }
    }
}
