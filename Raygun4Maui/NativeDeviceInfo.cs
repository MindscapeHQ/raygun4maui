using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raygun4Maui
{
    internal static class NativeDeviceInfo
    {



        public static ulong TotalPhysicalMemory()
        {
#if WINDOWS
            return 0;
#elif ANDROID
        var runtime = Java.Lang.Runtime.GetRuntime();
        if (runtime != null)
        {
          return (ulong)runtime.TotalMemory();
        }
        else
        {
          return 0;
        }
#elif IOS
            return 0;
#elif MACCATALYST
            return 0;

#else
            return 0;
#endif
        }


        public static ulong AvailablePhysicalMemory()
        {
#if WINDOWS
            return 0;
#elif ANDROID
        var runtime = Java.Lang.Runtime.GetRuntime();
        if (runtime != null)
        {
          return (ulong)runtime.FreeMemory();
        }
        else
        {
          return 0;
        }
#elif IOS
            return 0;
#elif MACCATALYST
            return 0;
#else
            return 0;
#endif
        }

        public static string Platform()
        {
#if WINDOWS
            return "Windows";
#elif ANDROID
        return "Android";
#elif IOS
             return "iOS";
#elif MACCATALYST
             return "MacCatalyst";
#else
            return "Unknown";
#endif
        }
    }
}
