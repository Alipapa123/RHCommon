using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class LogHelper
    {
        /// <summary>
        /// 写一条日志，日志路径为当前目录中的LOG文件夹中
        /// </summary>
        /// <param name="strLog"></param>
        public static void WriteLog(string strLog)
        {
            string DirectoryPath = AppDomain.CurrentDomain.BaseDirectory +"Log\\" + DateTime.Now.ToString("yyyyMM");
            string LogFilePath = DirectoryPath + "\\Log" + DateTime.Now.ToString("dd") + ".log"; //文件路径
            if (!Directory.Exists(DirectoryPath))//验证路径是否存在
            {
                Directory.CreateDirectory(DirectoryPath);
                //不存在则创建
            }
            FileStream fs;
            StreamWriter sw;
            if (File.Exists(LogFilePath))
            //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(LogFilePath, FileMode.Create, FileAccess.Write);
            }
            sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "   ---   " + strLog);
            sw.Close();
            fs.Close();
        }
    }
}
