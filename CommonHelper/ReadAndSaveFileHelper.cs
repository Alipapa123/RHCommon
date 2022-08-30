using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class ReadAndSaveFileHelper
    {
        public static void SaveToText(string Content, string FileName, string ExtentName = "txt")
        {
            string DirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "Data\\";
            string LogFilePath = DirectoryPath + "\\" + FileName + "." + ExtentName; //文件路径
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
                File.Delete(LogFilePath);
                //fs = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write);
                fs = new FileStream(LogFilePath, FileMode.Create, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(LogFilePath, FileMode.Create, FileAccess.Write);
            }
            sw = new StreamWriter(fs);
            sw.WriteLine(Content);
            sw.Close();
            fs.Close();
        }

        public static string ReadFileContent(string FilePath)
        {
            string Res = "";
            if (File.Exists(FilePath))
            //验证文件是否存在，有则追加，无则创建
            {
                Res = File.ReadAllText(FilePath);
            }
            return Res;
        }
    }
}
