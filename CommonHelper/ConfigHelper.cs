using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 更新指定的键值
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Value"></param>
        public static void UpdateKey(string KeyName, string Value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(KeyName))
            {
                ConfigurationManager.AppSettings.Add(KeyName, Value);
                config.Save();
            }
            else
            {
                config.AppSettings.Settings[KeyName].Value = Value;//修改子节点
                config.Save(ConfigurationSaveMode.Modified);//只有加保存功能,*.vshost.exe.Config才会作改变
            }
            ConfigurationManager.RefreshSection("AppSettings");
        }
        /// <summary>
        /// 获取指定的键值
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public static string GetKeyValue(string KeyName)
        {
            ConfigurationManager.RefreshSection("AppSettings");
            var res = ConfigurationManager.AppSettings[KeyName];
            return res;
        }

        public static void AddKey(string KeyName,string Value="")
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(KeyName))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Add(KeyName, Value);
                config.Save();
                ConfigurationManager.RefreshSection("AppSettings");
            }
        }
    }
}
