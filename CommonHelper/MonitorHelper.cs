using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonHelper
{
    /// <summary>
    /// 将窗体显示到指定屏幕帮助类（wpf适用）
    /// </summary>
     public static class MonitorHelper
    {
        /// <summary>
        /// 将窗口显示到指定显示器，并居中显示
        /// If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
        /// </summary>
        /// <param name="window"></param>
        public static void DisplayToMonitor(this Window window,int MonitorIndex,bool IsFullScreen=true)
        {
            if (!window.IsLoaded)
                window.WindowStartupLocation = WindowStartupLocation.Manual;

            System.Windows.Forms.Screen Monitor= System.Windows.Forms.Screen.AllScreens.Where(x=>x.Primary).FirstOrDefault();
            if (System.Windows.Forms.Screen.AllScreens.Length>= MonitorIndex+1)
            {
                 Monitor = System.Windows.Forms.Screen.AllScreens[MonitorIndex];
            }
            if (IsFullScreen)
            {
                window.Left = Monitor.WorkingArea.Left;
                window.Top = Monitor.WorkingArea.Top;
                if (window.IsLoaded)
                    window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.Left = Monitor.WorkingArea.Left + (Monitor.WorkingArea.Width - window.Width) / 2;
                window.Top = Monitor.WorkingArea.Top + (Monitor.WorkingArea.Height - window.Height) / 2;
            }   
        }
        /// <summary>
        /// 将窗口显示到主显示器界面，并居中显示
        /// If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
        /// </summary>
        /// <param name="window"></param>
        public static void DisplayToPrimaryMonitor(this Window window, bool IsFullScreen = true)
        {
            var Monitor = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

            if (Monitor != null)
            {
                if (!window.IsLoaded)
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                if (IsFullScreen)
                {
                    window.Left = Monitor.WorkingArea.Left;
                    window.Top = Monitor.WorkingArea.Top;
                    if (window.IsLoaded)
                        window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.Left = Monitor.WorkingArea.Left + (Monitor.WorkingArea.Width - window.Width) / 2;
                    window.Top = Monitor.WorkingArea.Top + (Monitor.WorkingArea.Height - window.Height) / 2;
                }
            }
        }

        /// <summary>
        /// 将窗口显示到主显示器界面，并居中显示
        /// If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
        /// </summary>
        /// <param name="window"></param>
        public static void DisplayToNotPrimaryMonitor(this Window window, bool IsFullScreen = true)
        {
            var Monitor = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary==false).FirstOrDefault();

            if (Monitor != null)
            {
                if (!window.IsLoaded)
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                if (IsFullScreen)
                {
                    window.Left = Monitor.WorkingArea.Left;
                    window.Top = Monitor.WorkingArea.Top;
                    if (window.IsLoaded)
                        window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.Left = Monitor.WorkingArea.Left + (Monitor.WorkingArea.Width - window.Width) / 2;
                    window.Top = Monitor.WorkingArea.Top + (Monitor.WorkingArea.Height - window.Height) / 2;
                }
            }
        }
    }
}
