#if IOS || MACCATALYST
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
#endif

namespace Raygun4Maui.MauiRUM.EventTrackers.Apple;

public class RaygunSwizzle
{
#if IOS || MACCATALYST
    [DllImport("/usr/lib/libobjc.dylib")]
    public extern static IntPtr class_getInstanceMethod(IntPtr classHandle, IntPtr Selector);

    [DllImport("/usr/lib/libobjc.dylib")]
    public extern static IntPtr imp_implementationWithBlock(ref BlockLiteral block);

    [DllImport("/usr/lib/libobjc.dylib")]
    public extern static void method_setImplementation(IntPtr method, IntPtr imp);

    [DllImport("/usr/lib/libobjc.dylib")]
    public extern static IntPtr method_getImplementation(IntPtr method);

    public delegate void CaptureDelegate(IntPtr block, IntPtr self);

    public delegate void CaptureBooleanDelegate(IntPtr block, IntPtr self, bool b);
    
    [MonoNativeFunctionWrapper]
    public delegate void OriginalBooleanDelegate(IntPtr self, bool b);

    public static void Hijack(NSObject obj, string selector, ref IntPtr originalImpl, CaptureDelegate captureDelegate)
    {
        var method = class_getInstanceMethod(obj.ClassHandle, new Selector(selector).Handle);
        originalImpl = method_getImplementation(method);

        if (originalImpl != IntPtr.Zero)
        {
            var block_value = new BlockLiteral();
            block_value.SetupBlock(captureDelegate, null);

            var imp = imp_implementationWithBlock(ref block_value);
            method_setImplementation(method, imp);
        }
    }

    public static void Hijack(NSObject obj, string selector, ref IntPtr originalImpl,
        CaptureBooleanDelegate captureDelegate)
    {
        var method = class_getInstanceMethod(obj.ClassHandle, new Selector(selector).Handle);
        originalImpl = method_getImplementation(method);

        if (originalImpl != IntPtr.Zero)
        {
            var block_value = new BlockLiteral();
            block_value.SetupBlock(captureDelegate, null);

            var imp = imp_implementationWithBlock(ref block_value);
            method_setImplementation(method, imp);
        }
    }

    public static void Restore(NSObject obj, string selector, IntPtr originalImpl)
    {
        if (originalImpl != IntPtr.Zero)
        {
            var method = class_getInstanceMethod(obj.ClassHandle, new Selector(selector).Handle);
            method_setImplementation(method, originalImpl);
        }
    }
#endif
}