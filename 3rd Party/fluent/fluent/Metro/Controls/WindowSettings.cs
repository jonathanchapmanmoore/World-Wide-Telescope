using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Fluent.Metro.Native;

namespace Fluent.Metro.Controls
{
    /// <summary>
    /// Stores settings for a <see cref="Window"/>
    /// </summary>
    public class WindowSettings
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for Save.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SaveProperty = DependencyProperty.RegisterAttached("Save", typeof(bool), typeof(WindowSettings), new FrameworkPropertyMetadata(OnSaveInvalidated));

        /// <summary>
        /// Sets Save for dependencyObject
        /// </summary>
        public static void SetSave(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(SaveProperty, enabled);
        }

        internal class WindowApplicationSettings : ApplicationSettingsBase
        {
            public WindowApplicationSettings(WindowSettings windowSettings)
                : base(windowSettings._window.GetType().FullName)
            {
            }

            [UserScopedSetting]
            public WINDOWPLACEMENT? Placement
            {
                get
                {
                    if (this["Placement"] != null)
                    {
                        return ((WINDOWPLACEMENT)this["Placement"]);
                    }

                    return null;
                }
                set
                {
                    this["Placement"] = value;
                }
            }
        }

        private Window _window;

        /// <summary>
        /// Creates a new instance which uses <paramref name="window"/>
        /// </summary>
        public WindowSettings(Window window)
        {
            _window = window;
        }

        private static void OnSaveInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null || !((bool)e.NewValue))
            {
                return;
            }

            var settings = new WindowSettings(window);
            settings.Attach();
        }

        /// <summary>
        /// Loads the <see cref="Settings"/>
        /// </summary>
        protected virtual void LoadWindowState()
        {
            Settings.Reload();

            if (Settings.Placement == null)
            {
                return;
            }

            try
            {
                var wp = Settings.Placement.Value;

                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == Constants.SW_SHOWMINIMIZED ? Constants.SW_SHOWNORMAL : wp.showCmd);
                var hwnd = new WindowInteropHelper(_window).Handle;
                UnsafeNativeMethods.SetWindowPlacement(hwnd, ref wp);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Failed to load window state:\r\n{0}", ex));
            }
        }

        /// <summary>
        /// Saves the <see cref="Settings"/>
        /// </summary>
        protected virtual void SaveWindowState()
        {
            WINDOWPLACEMENT wp;
            var hwnd = new WindowInteropHelper(_window).Handle;
            UnsafeNativeMethods.GetWindowPlacement(hwnd, out wp);
            Settings.Placement = wp;
            Settings.Save();
        }

        private void Attach()
        {
            if (_window == null) return;
            _window.Closing += WindowClosing;
            _window.SourceInitialized += WindowSourceInitialized;
        }

        private void WindowSourceInitialized(object sender, EventArgs e)
        {
            LoadWindowState();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            SaveWindowState();
            _window.Closing -= WindowClosing;
            _window.SourceInitialized -= WindowSourceInitialized;
            _window = null;
        }

        private WindowApplicationSettings _windowApplicationSettings;

        internal virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
        {
            return new WindowApplicationSettings(this);
        }

        [Browsable(false)]
        internal WindowApplicationSettings Settings
        {
            get { return _windowApplicationSettings ?? (_windowApplicationSettings = CreateWindowApplicationSettingsInstance()); }
        }
    }
}