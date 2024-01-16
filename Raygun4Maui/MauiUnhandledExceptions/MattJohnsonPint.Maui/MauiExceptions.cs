/*
 * This file was taken from https://gist.github.com/mattjohnsonpint/7b385b7a2da7059c4a16562bc5ddb3b7#file-mauiexceptions-cs
 * It uses the MIT License
 */

namespace Raygun4Maui.MauiUnhandledExceptions.MattJohnsonPint.Maui
{
    public static class MauiExceptions
    {
#if WINDOWS
        private static Exception _lastFirstChanceException;
#endif
        public static void CaptureWindowsUnhandeledExceptions()
        {


#if WINDOWS

            // For WinUI 3:
            //
            // * Exceptions on background threads are caught by AppDomain.CurrentDomain.UnhandledException,
            //   not by Microsoft.UI.Xaml.Application.Current.UnhandledException
            //   See: https://github.com/microsoft/microsoft-ui-xaml/issues/5221
            //
            // * Exceptions caught by Microsoft.UI.Xaml.Application.Current.UnhandledException have details removed,
            //   but that can be worked around by saved by trapping first chance exceptions
            //   See: https://github.com/microsoft/microsoft-ui-xaml/issues/7160
            //

            AppDomain.CurrentDomain.FirstChanceException += (_, args) =>
            {
                _lastFirstChanceException = args.Exception;
            };

            Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, args) =>
            {
                var exception = args.Exception;

                if (exception.StackTrace is null)
                {
                    exception = _lastFirstChanceException;
                }
                
                // TODO: Hook into Raygun4Net unhandelled exception
                // UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(exception, true));
            };
#endif
        }
    }
}
