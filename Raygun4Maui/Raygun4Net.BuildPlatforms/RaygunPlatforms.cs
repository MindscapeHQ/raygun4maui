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
