using System.Runtime.InteropServices;
using System.Diagnostics;

#if WINDOWS
#elif ANDROID
using Android.OS;
#elif IOS || MACCATALYST
using Foundation;
using UIKit;
using ObjCRuntime;
#endif


namespace Raygun4Maui
{
    internal static class NativeDeviceInfo
    {
#if IOS || MACCATALYST
        private const string MEM_AVAILABLE_PROP_NAME = "hw.usermem";
        private const string MEM_TOTAL_PROP_NAME = "hw.physmem";
        private const string MACHINE_PROP_NAME = "hw.machine";

        [DllImport(ObjCRuntime.Constants.SystemLibrary)]
        private static extern int sysctlbyname([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

        private static uint GetUIntSysCtl(string propertyName)
        {
            // get the length of the string that will be returned
            var pLen = Marshal.AllocHGlobal(sizeof(int));
            sysctlbyname(propertyName, IntPtr.Zero, pLen, IntPtr.Zero, 0);

            var length = Marshal.ReadInt32(pLen);

            // check to see if we got a length
            if (length <= 0)
            {
                Marshal.FreeHGlobal(pLen);
                return 0;
            }

            // get the hardware string
            var pStr = Marshal.AllocHGlobal(length);
            sysctlbyname(propertyName, pStr, pLen, IntPtr.Zero, 0);

            // convert the native string into a C# integer

            var memoryCount = Marshal.ReadInt32(pStr);
            uint memoryVal = (uint)memoryCount;

            if (memoryCount < 0)
            {
                memoryVal = (uint)((uint)int.MaxValue + (-memoryCount));
            }

            var ret = memoryVal;

            // cleanup
            Marshal.FreeHGlobal(pLen);
            Marshal.FreeHGlobal(pStr);

            return ret;
        }

        private static string GetStringSysCtl(string propertyName)
        {
            // get the length of the string that will be returned
            var pLen = Marshal.AllocHGlobal(sizeof(int));
            sysctlbyname(propertyName, IntPtr.Zero, pLen, IntPtr.Zero, 0);

            var length = Marshal.ReadInt32(pLen);

            // check to see if we got a length
            if (length <= 0)
            {
                Marshal.FreeHGlobal(pLen);
                return "Unknown";
            }

            // get the hardware string
            var pStr = Marshal.AllocHGlobal(length);
            sysctlbyname(propertyName, pStr, pLen, IntPtr.Zero, 0);

            // convert the native string into a C# integer

            var hardwareStr = Marshal.PtrToStringAnsi(pStr);


            // cleanup
            Marshal.FreeHGlobal(pLen);
            Marshal.FreeHGlobal(pStr);

            return hardwareStr;
        }

#elif WINDOWS
        private static GCMemoryInfo gcMemoryInfo = GC.GetGCMemoryInfo();
#endif

        public static ulong TotalPhysicalMemory()
        {
#if WINDOWS
            return (ulong)(gcMemoryInfo.TotalAvailableMemoryBytes);
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
#elif IOS || MACCATALYST
            return GetUIntSysCtl(MEM_TOTAL_PROP_NAME);
#else
            return 0;
#endif
        }


        public static ulong AvailablePhysicalMemory()
        {
#if WINDOWS
            Process p = Process.GetCurrentProcess();
            return (ulong)p.PrivateMemorySize64; //Not sure if this is the right value to use
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
#elif IOS || MACCATALYST
            return GetUIntSysCtl(MEM_AVAILABLE_PROP_NAME);
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

        public static string Architecture()
        {
#if WINDOWS
            return RuntimeInformation.ProcessArchitecture.ToString();
#elif ANDROID
        var supportedABIs = Build.SupportedAbis;
        if (supportedABIs != null && supportedABIs.Count > 0)
        {
          return supportedABIs[0];
        }
        else
        {
          return RuntimeInformation.ProcessArchitecture.ToString();
        }

#elif IOS
            return RuntimeInformation.ProcessArchitecture.ToString();
#elif MACCATALYST
             return RuntimeInformation.ProcessArchitecture.ToString();
#else
            return RuntimeInformation.ProcessArchitecture.ToString();
#endif
        }
    }
}