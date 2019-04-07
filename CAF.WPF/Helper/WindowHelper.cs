#region Usings

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CompositeApplicationFramework.Utility;

#endregion

namespace CompositeApplicationFramework.Helper
{

    #region Dependencies

    #endregion

    public static class WindowHelper
    {
        private static readonly ConcurrentDictionary<Type, Window> Windows = new ConcurrentDictionary<Type, Window>();

        public static void ShowWindow<T>() where T : Window
        {
            Debug.WriteLine("Show Window : {0}", typeof (T));

            if (Windows.ContainsKey(typeof (T)))
            {
                CloseWindow<T>();
            }

            var newThread = new Thread(
                () =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                    var window = FastActivator.CreateInstance<T>();

                    if (window != null)
                    {
                        Windows.TryAdd(typeof (T), window);

                        window.Closed +=
                            (s, e) =>
                                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                        window.Show();
                    }

                    Dispatcher.Run();
                });

            newThread.SetApartmentState(ApartmentState.STA);

            newThread.IsBackground = true;

            newThread.Start();
        }

        public static void ShowWindow<T, TParam>(TParam param) where T : Window
        {
            Debug.WriteLine("Show Window : {0}", typeof (T));

            if (Windows.ContainsKey(typeof (T)))
            {
                CloseWindow<T>();
            }

            var newThread = new Thread(
                () =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                    var window = FastActivator<TParam>.CreateInstance<T>(param);

                    if (window != null)
                    {
                        Windows.TryAdd(typeof (T), window);

                        window.Closed +=
                            (s, e) =>
                                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                        window.Show();
                    }

                    Dispatcher.Run();
                });

            newThread.SetApartmentState(ApartmentState.STA);

            newThread.IsBackground = true;

            newThread.Start();
        }

        public static void ShowWindow<T, TParamA, TParamB>(TParamA paramA, TParamB paramB) where T : Window
        {
            Debug.WriteLine("Show Window : {0}", typeof (T));

            if (Windows.ContainsKey(typeof (T)))
            {
                CloseWindow<T>();
            }

            var newThread = new Thread(
                () =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                    var window = FastActivator<TParamA, TParamB>.CreateInstance<T>(paramA, paramB);

                    if (window != null)
                    {
                        Windows.TryAdd(typeof (T), window);

                        window.Closed +=
                            (s, e) =>
                                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                        window.Show();
                    }

                    Dispatcher.Run();
                });

            newThread.SetApartmentState(ApartmentState.STA);

            newThread.IsBackground = true;

            newThread.Start();
        }

        public static void CloseWindow<T>()
        {
            Debug.WriteLine("Close Window : {0}", typeof (T));

            Window window;

            Windows.TryRemove(typeof (T), out window);

            if (window == null)
            {
                return;
            }

            window.Dispatcher.BeginInvoke((Action) (window.Close));

            // ReSharper disable once SuspiciousTypeConversion.Global
            (window as IDisposable)?.Dispose();
        }

        public static bool IsModal(this Window window)
        {
            var fieldInfo = typeof (Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldInfo != null && (bool) fieldInfo.GetValue(window);
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        public static void SetCaptionHeight(this Window window, int height)
        {
            var pMargins = new Margins
            {
                cxLeftWidth = 0,
                cxRightWidth = 0,
                cyTopHeight = height,
                cyBottomHeight = 0
            };

            DwmExtendFrameIntoClientArea(new WindowInteropHelper(window).Handle, ref pMargins);
        }

        public static Color? GetChromeColor()
        {
            bool isEnabled;
            var hr1 = DwmIsCompositionEnabled(out isEnabled);
            if ((hr1 != 0) || !isEnabled) // 0 means S_OK.
            {
                return null;
            }

            DWMCOLORIZATIONPARAMS parameters;
            try
            {
                // This API is undocumented and so may become unusable in future versions of OSes.
                var hr2 = DwmGetColorizationParameters(out parameters);
                if (hr2 != 0) // 0 means S_OK.
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

            // Convert colorization color parameter to Color ignoring alpha channel.
            var targetColor = Color.FromRgb(
                (byte) (parameters.colorizationColor >> 16),
                (byte) (parameters.colorizationColor >> 8),
                (byte) parameters.colorizationColor);

            // Prepare base gray color.
            var baseColor = Color.FromRgb(217, 217, 217);

            // Blend the two colors using colorization color balance parameter.
            return BlendColor(targetColor, baseColor, 100 - parameters.colorizationColorBalance);
        }

        private static Color BlendColor(Color color1, Color color2, double color2Perc)
        {
            if ((color2Perc < 0) || (100 < color2Perc))
            {
                throw new ArgumentOutOfRangeException(nameof(color2Perc));
            }

            return Color.FromRgb(
                BlendColorChannel(color1.R, color2.R, color2Perc),
                BlendColorChannel(color1.G, color2.G, color2Perc),
                BlendColorChannel(color1.B, color2.B, color2Perc));
        }

        private static byte BlendColorChannel(double channel1, double channel2, double channel2Perc)
        {
            var buff = channel1 + (channel2 - channel1)*channel2Perc/100D;
            return Math.Min((byte) Math.Round(buff), (byte) 255);
        }

        [DllImport("Dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool pfEnabled);

        [DllImport("Dwmapi.dll", EntryPoint = "#127")] // Undocumented API
        private static extern int DwmGetColorizationParameters(out DWMCOLORIZATIONPARAMS parameters);

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            public int cxLeftWidth;

            public int cxRightWidth;

            public int cyTopHeight;

            public int cyBottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        private struct DWMCOLORIZATIONPARAMS
        {
            public readonly uint colorizationColor;

            public readonly uint colorizationAfterglow;

            public readonly uint colorizationColorBalance; // Ranging from 0 to 100

            public readonly uint colorizationAfterglowBalance;

            public readonly uint colorizationBlurBalance;

            public readonly uint colorizationGlassReflectionIntensity;

            public readonly uint colorizationOpaqueBlend;
        }
    }
}