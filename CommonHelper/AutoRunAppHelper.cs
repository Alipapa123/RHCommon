using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 开机自启帮助类
    /// </summary>
    public static class AppAutoRunHelper
    {
        public static void SetAppAutoRun(String KeyName= "AutoRunApp")
        {
            string AppPath = Assembly.GetEntryAssembly().Location;
            RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            RKey.SetValue(KeyName, AppPath);
        }

        public static void DeleteAppAutoRun(String KeyName = "AutoRunApp")
        {
            string AppPath = Assembly.GetEntryAssembly().Location;
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        }
    }
}
