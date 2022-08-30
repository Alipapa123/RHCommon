using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 字节转换帮助类
    /// </summary>
    public static class ByteConvertHelper
    {
        /// <summary>
        /// 将字节数组转化为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Split"></param>
        /// <returns></returns>
        public static string ByteArrayToString(IEnumerable<byte> bytes, string Split=" ",string StringFormat="X2")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString(StringFormat));
                sb.Append(Split);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将16进制字符串转化为字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="Split"></param>
        /// <returns></returns>
        public static List<byte> X2StringToHexByteArray(string hexString, string Split=" ")
        {
            //验证是否为正确的16进制字符串；
            var MatchResult1 = Regex.Match(hexString, "[0-9a-fA-F]{3,}");
            var hexStringTem = hexString.Replace(Split,"");
            var MatchResult2= Regex.Match(hexStringTem, "[^0-9a-fA-F]");
            
            if (MatchResult1.Success|| MatchResult2.Success)
            {
                throw new Exception("非法的二进制字符串，除分割字符串外，要转换的字符串中不能包含0-9,a-f,A-F字符之外的字符，且字符串中每个二进制数位数不大于2位");
            }
            var StringArray = Regex.Split(hexString, Split);
         
            List<byte> returnBytes = new List<byte>();
            foreach (var item in StringArray)
            {
                returnBytes.Add(Convert.ToByte(item, 16));
            }
            return returnBytes;
        }
    }
}
