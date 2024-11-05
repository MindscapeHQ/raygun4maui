using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if IOS || MACCATALYST
using Foundation;
using ObjCRuntime;
using UIKit;
using Raygun4Maui.AppEvents;
#endif

namespace Raygun4Maui.MauiRUM.EventTrackers.Apple;

public class RaygunUiViewControllerObserver
{
#if IOS || MACCATALYST
    public RaygunUiViewControllerObserver()
    {
    }

    private static IntPtr _originalLoadViewImp;
    private static IntPtr _originalViewDidLoadImp;
    private static IntPtr _originalViewWillAppearImp;
    private static IntPtr _originalViewDidAppearImp;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CaptureDelegate(IntPtr block, IntPtr self);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CaptureBooleanDelegate(IntPtr block, IntPtr self, bool b);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OriginalDelegate(IntPtr self);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OriginalBooleanDelegate(IntPtr self, bool b);

    public void Register()
    {
        RaygunSwizzle.Hijack(new UIViewController(), "loadView", ref _originalLoadViewImp, OnLoadViewCapture);
        RaygunSwizzle.Hijack(new UIViewController(), "viewDidLoad", ref _originalViewDidLoadImp, OnViewDidLoadCapture);
        RaygunSwizzle.Hijack(new UIViewController(), "viewWillAppear:", ref _originalViewWillAppearImp,
            OnViewWillAppearCapture);
        RaygunSwizzle.Hijack(new UIViewController(), "viewDidAppear:", ref _originalViewDidAppearImp,
            OnViewDidAppearCapture);
    }

    public void DeRegister()
    {
        RaygunSwizzle.Restore(new UIViewController(), "loadView", _originalLoadViewImp);
        RaygunSwizzle.Restore(new UIViewController(), "viewDidLoad", _originalViewDidLoadImp);
        RaygunSwizzle.Restore(new UIViewController(), "viewWillAppear:", _originalViewWillAppearImp);
        RaygunSwizzle.Restore(new UIViewController(), "viewDidAppear:", _originalViewDidAppearImp);
    }

    //
    // Load View
    //
    
    [MonoPInvokeCallback(typeof(CaptureDelegate))]
    private static void OnLoadViewCapture(IntPtr block, IntPtr self)
    {
        string name = GetName(Runtime.GetNSObject(self));

        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
        {
            Name = name,
            OccurredOn = DateTime.UtcNow.Ticks
        });

        var orig = Marshal.GetDelegateForFunctionPointer<OriginalDelegate>(_originalLoadViewImp);
        orig(self);
    }

    //
    // View Did Load
    //
    
    [MonoPInvokeCallback(typeof(CaptureDelegate))]
    private static void OnViewDidLoadCapture(IntPtr block, IntPtr self)
    {
        string name = GetName(Runtime.GetNSObject(self));

        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
        {
            Name = name,
            OccurredOn = DateTime.UtcNow.Ticks
        });

        var orig = Marshal.GetDelegateForFunctionPointer<OriginalDelegate>(_originalViewDidLoadImp);
        orig(self);
    }

    //
    // View Will Appear
    //

    [MonoPInvokeCallback(typeof(CaptureBooleanDelegate))]
    private static void OnViewWillAppearCapture(IntPtr block, IntPtr self, bool animated)
    {
        string name = GetName(Runtime.GetNSObject(self));

        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
        {
            Name = name,
            OccurredOn = DateTime.UtcNow.Ticks
        });

        var orig = Marshal.GetDelegateForFunctionPointer<OriginalBooleanDelegate>(_originalViewWillAppearImp);
        orig(self, animated);
    }

    //
    // View Did Appear
    //

    [MonoPInvokeCallback(typeof(CaptureBooleanDelegate))]
    private static void OnViewDidAppearCapture(IntPtr block, IntPtr self, bool animated)
    {
        var orig = Marshal.GetDelegateForFunctionPointer<OriginalBooleanDelegate>(_originalViewDidAppearImp);
        orig(self, animated);

        string name = GetName(Runtime.GetNSObject(self));

        RaygunAppEventPublisher.Publish(new ViewTimingFinished()
        {
            Name = name,
            OccurredOn = DateTime.UtcNow.Ticks
        });
    }

    private static string GetName(NSObject obj)
    {
        var objName = obj.ToString();
        string name = objName.Replace("<", string.Empty).Replace(">", string.Empty).Replace("_", ".");

        var index = name.IndexOf(':');
        if (index >= 0 && index < name.Length)
        {
            name = name.Substring(0, index);
        }

        return name;
    }
#endif
}