using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class DPIHelper
    {

        #region Win32 API
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
            IntPtr hdc, // handle to DC
            int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        #endregion

        #region DeviceCaps常量
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;
        #endregion

        #region 属性

        /// <summary>
        /// 当前系统DPI_X 大小 一般为96
        /// </summary>
        public static int DpiX
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                ReleaseDC(IntPtr.Zero, hdc);
                return dpiX;
            }
        }

        /// <summary>
        /// 当前系统DPI_Y 大小 一般为96
        /// </summary>
        public static int DpiY
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var dpiX = GetDeviceCaps(hdc, LOGPIXELSY);
                ReleaseDC(IntPtr.Zero, hdc);
                return dpiX;
            }
        }

        /// <summary>
        /// 获取真实设置的桌面分辨率大小
        /// </summary>
        public static Size Desktop
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var size = new Size
                {
                    Width = GetDeviceCaps(hdc, DESKTOPHORZRES),
                    Height = GetDeviceCaps(hdc, DESKTOPVERTRES)
                };
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }

        /// <summary>
        /// 获取屏幕分辨率当前物理大小
        /// </summary>
        public static Size WorkingArea
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var size = new Size
                {
                    Width = GetDeviceCaps(hdc, HORZRES),
                    Height = GetDeviceCaps(hdc, VERTRES)
                };
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }

        /// <summary>
        /// 获得横轴缩放比
        /// 此参数为[控制面板-设置-显示-更改文本、应用等项目的大小]所显示的百分比
        /// </summary>
        public static float ScaleX => DpiX / 96f;

        /// <summary>
        /// 获得纵轴缩放比
        /// 此参数为[控制面板-设置-显示-更改文本、应用等项目的大小]所显示的百分比
        /// </summary>
        public static float ScaleY => DpiY / 96f;

        #endregion

        public static float GetPositionScale()
        {
            var hdc = GetDC(IntPtr.Zero);
            var dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
            ReleaseDC(IntPtr.Zero, hdc);
            var scale = Convert.ToInt32(dpiX / 96f * 100);
            switch (scale)
            {
                case 125:
                    return .8f;
                case 150:
                    return .65f;
                case 175:
                    return .575f;
                default:
                    return 1f;
            }
        }




    /////获取当前系统的dpi数值
    //public static void SystemDpi(out int x, out int y)
    //    {
    //        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
    //        {
    //            x = (int)g.DpiX;
    //            y = (int)g.DpiY;
    //            g.Dispose();
    //        }
    //    }

    //    ///根据当前系统dpi数值匹配 当前系统的桌面缩放比例
    //    public static double Scaling()//x或y都一样
    //    {
    //        int x_DPI=0, y_DPI=0;
    //        SystemDpi(out x_DPI, out y_DPI);
    //        switch (x_DPI)
    //        {
    //            case 96: return 1;
    //            case 120: return 1.25;
    //            case 144: return 1.5;
    //            case 168: return 1.75;
    //            case 192: return 2;
    //        }
    //        return 1;
    //    }
    }
}
