// Namespace is different, doesn't include Raygun4Maui because we may want to move this to Raygun4Net
// ReSharper disable once CheckNamespace
namespace Raygun4Net.BuildPlatforms
{
    public static class Raygun4NetBuildPlatforms
    {
        public static string GetBuildPlatform()
        {
#if WINDOWS
            return "Windows";
#elif IOS
            return "iOS";
#elif MACCATALYST
            return "MacCatalyst";
#elif ANDROID
            return "Android";
#else
            return "UknownBuildPlatform";
#endif
        }
    }
}
