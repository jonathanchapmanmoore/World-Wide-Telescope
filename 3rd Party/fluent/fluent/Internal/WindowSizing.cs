namespace Fluent.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using Fluent.Metro.Native;

    /// <summary>
    /// Encapsulates logic for window sizing (maximizing etc.)
    /// </summary>
    public class WindowSizing
    {
        private readonly RibbonWindow window;
        private IntPtr hwnd;
        private bool fixingNastyWindowChromeBug;

        /// <summary>
        /// Creates a new instance and binds it to <paramref name="window"/>
        /// </summary>
        public WindowSizing(RibbonWindow window)
        {
            this.window = window;

            this.window.StateChanged += this.HandleWindowStateChanged;
        }

        /// <summary>
        /// Called when <see cref="window"/> has been initialize
        /// </summary>
        public void WindowInitialized()
        {
            var hwndSource = PresentationSource.FromVisual(this.window) as HwndSource;
            if (hwndSource != null)
            {
                this.hwnd = hwndSource.Handle;
                hwndSource.AddHook(HwndHook);

                this.window.Dispatcher.BeginInvoke((Action)(this.FixNastyWindowChromeBug));
            }
        }

        private void HandleWindowStateChanged(object sender, EventArgs e)
        {
            this.window.Dispatcher.BeginInvoke((Action)(this.FixNastyWindowChromeBug));
        }

        private void FixNastyWindowChromeBug()
        {
            if (this.fixingNastyWindowChromeBug
                || this.window.WindowState != WindowState.Maximized)
            {
                return;
            }

            this.fixingNastyWindowChromeBug = true;

            var mmi = GetMinMaxInfo(hwnd, new MINMAXINFO());
            if (NativeMethods.IsDwmEnabled())
            {
                UnsafeNativeMethods.MoveWindow(this.hwnd, mmi.ptMaxPosition.X + 10, mmi.ptMaxPosition.Y, mmi.ptMaxSize.X, mmi.ptMaxSize.Y, true);
                UnsafeNativeMethods.MoveWindow(this.hwnd, mmi.ptMaxPosition.X, mmi.ptMaxPosition.Y, mmi.ptMaxSize.X, mmi.ptMaxSize.Y, true);
            }
            else
            {
                UnsafeNativeMethods.MoveWindow(this.hwnd, mmi.ptMaxPosition.X, mmi.ptMaxPosition.Y + 1, mmi.ptMaxSize.X, mmi.ptMaxSize.Y, true);
            }

            this.fixingNastyWindowChromeBug = false;
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (message)
            {
                case Constants.WM_GETMINMAXINFO:

                    WINDOWPLACEMENT windowPlacement;
                    UnsafeNativeMethods.GetWindowPlacement(hwnd, out windowPlacement);

                    if (windowPlacement.showCmd == 3)
                    {
                        /* http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                        WmGetMinMaxInfo(hWnd, lParam);

                        handled = true;
                    }
                    break;
            }

            return returnval;
        }

        #region WindowSize

        private bool IgnoreTaskBar()
        {
            //var ignoreTaskBar = this.AssociatedObject.IgnoreTaskbarOnMaximize 
            //    || this.AssociatedObject.WindowStyle == WindowStyle.None;

            return false;
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            mmi = GetMinMaxInfo(hwnd, mmi);

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private MINMAXINFO GetMinMaxInfo(IntPtr hwnd, MINMAXINFO mmi)
        {
            // Adjust the maximized size and position to fit the work area of the correct monitor
            var monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitor == IntPtr.Zero)
            {
                return mmi;
            }

            var monitorInfo = new MONITORINFO();
            UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
            var rcWorkArea = monitorInfo.rcWork;
            var rcMonitorArea = monitorInfo.rcMonitor;

            Debug.WriteLine("Monitor-Info");
            Debug.WriteLine(string.Format("Work: {0}", rcWorkArea));
            Debug.WriteLine(string.Format("Mon : {0}", rcMonitorArea));

            Debug.WriteLine(string.Format("Before: {0}", mmi));

            mmi.ptMaxPosition.X = rcWorkArea.left;
            mmi.ptMaxPosition.Y = rcWorkArea.top;

            var ignoreTaskBar = this.IgnoreTaskBar();

            var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
            var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
            mmi.ptMaxSize.X = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.right - x) : Math.Abs(monitorInfo.rcWork.right - x);
            mmi.ptMaxSize.Y = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.bottom - y) : Math.Abs(monitorInfo.rcWork.bottom - y);

            if (!ignoreTaskBar)
            {
                mmi.ptMaxTrackSize.X = mmi.ptMaxSize.X;
                mmi.ptMaxTrackSize.Y = mmi.ptMaxSize.Y;
                mmi = AdjustWorkingAreaForAutoHide(monitor, mmi);
            }

            Debug.WriteLine(string.Format("After: {0}", mmi));

            return mmi;
        }

        private static int GetEdge(RECT rc)
        {
            int uEdge;

            if (rc.top == rc.left
                && rc.bottom > rc.right)
            {
                uEdge = (int)ABEdge.ABE_LEFT;
            }
            else if (rc.top == rc.left
                && rc.bottom < rc.right)
            {
                uEdge = (int)ABEdge.ABE_TOP;
            }
            else if (rc.top > rc.left)
            {
                uEdge = (int)ABEdge.ABE_BOTTOM;
            }
            else
            {
                uEdge = (int)ABEdge.ABE_RIGHT;
            }

            return uEdge;
        }

        /// <summary>
        /// This method handles the window size if the taskbar is set to auto-hide.
        /// </summary>
        private static MINMAXINFO AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, MINMAXINFO mmi)
        {
            var hwnd = UnsafeNativeMethods.FindWindow("Shell_TrayWnd", null);
            var monitorWithTaskbarOnIt = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitorContainingApplication.Equals(monitorWithTaskbarOnIt) == false)
            {
                return mmi;
            }

            var abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref abd);
            var uEdge = GetEdge(abd.rc);
            var autoHide = UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd) == new IntPtr(1);

            if (!autoHide)
            {
                return mmi;
            }

            switch (uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    mmi.ptMaxPosition.X += 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    mmi.ptMaxSize.X -= 2;
                    break;

                case (int)ABEdge.ABE_RIGHT:
                    mmi.ptMaxSize.X -= 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    break;

                case (int)ABEdge.ABE_TOP:
                    mmi.ptMaxPosition.Y += 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    mmi.ptMaxSize.Y -= 2;
                    break;

                case (int)ABEdge.ABE_BOTTOM:
                    mmi.ptMaxSize.Y -= 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    break;

                default:
                    return mmi;
            }

            return mmi;
        }

        #endregion WindowSize
    }
}