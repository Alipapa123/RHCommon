using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 身份证号码验证
    /// </summary>
    public class IdentityValidateHelper
    {
        #region 身份证号码验证
        /*
         * 身份证验证的工具（支持5位或18位省份证）
         * 身份证号码结构：
         * 17位数字和1位校验码：6位地址码数字，8位生日数字，3位出生时间顺序号，1位校验码。
         * 地址码（前6位）：表示对象常住户口所在县（市、镇、区）的行政区划代码，按GB/T2260的规定执行。
         * 出生日期码，（第七位 至十四位）：表示编码对象出生年、月、日，按GB按GB/T7408的规定执行，年、月、日代码之间不用分隔符。
         * 顺序码（第十五位至十七位）：表示在同一地址码所标示的区域范围内，对同年、同月、同日出生的人编订的顺序号，
         * 顺序码的奇数分配给男性，偶数分配给女性。 
         * 校验码（第十八位数）：
         * 十七位数字本体码加权求和公式 s = sum(Ai*Wi), i = 0,,16，先对前17位数字的权求和；   
         *  Ai:表示第i位置上的身份证号码数字值.Wi:表示第i位置上的加权因.Wi: 7 9 10 5 8 4 2 1 6 3 7 9 10 5 8 4 2；
         * 计算模 Y = mod(S, 11) 
         * 通过模得到对应的校验码 Y: 0 1 2 3 4 5 6 7 8 9 10 校验码: 1 0 X 9 8 7 6 5 4 3 2 
         */
        static Dictionary<int, String> zoneNum = new Dictionary<int, String>()
    {
        {11, "北京"  },
        {12, "天津"  } ,
        {13, "河北"  } ,
        {14, "山西"  } ,
        {15, "内蒙古" } ,
        {21, "辽宁"  } ,
        {22, "吉林"  } ,
        {23, "黑龙江" } ,
        {31, "上海"  } ,
        {32, "江苏"  } ,
        {33, "浙江"  } ,
        {34, "安徽"  } ,
        {35, "福建"  } ,
        {36, "江西"  } ,
        {37, "山东"  } ,
        {41, "河南"  } ,
        {42, "湖北"  } ,
        {43, "湖南"  } ,
        {44, "广东"  } ,
        {45, "广西"  } ,
        {46, "海南"  } ,
        {50, "重庆"  } ,
        {51, "四川"  } ,
        {52, "贵州"  } ,
        {53, "云南"  } ,
        {54, "西藏"  } ,
        {61, "陕西"  } ,
        {62, "甘肃"  } ,
        {63, "青海"  } ,
        {64, "宁夏"  } ,
        {65, "新疆"  } ,
        {71, "台湾"  } ,
        {81, "香港"  } ,
        {82, "澳门"  } ,
        {91, "外国" } ,
    };

        static readonly int[] PARITYBIT = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
        static readonly int[] POWER_LIST = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

        /// <summary>
        /// 身份证验证( 正规的验证需要接公安系统 )
        /// </summary>
        /// <param name="certNo">号码内容</param>
        /// <returns>是否有效</returns>
        public static bool IDCardValidate(string certNo)
        {
            if (certNo == null || (certNo.Length != 15 && certNo.Length != 18))
            {
                return false;
            }
            char[] cs = certNo.ToUpper().ToCharArray();
            //校验位数
            int power = 0;
            for (int i = 0; i < cs.Length; i++)
            {
                if (i == cs.Length - 1 && cs[i] == 'X')
                {
                    break;//最后一位可以 是X或x
                }
                if (cs[i] < '0' || cs[i] > '9')
                {
                    return false;
                }
                if (i < cs.Length - 1)
                {
                    power += (cs[i] - '0') * POWER_LIST[i];
                }
            }
            //校验区位码
            if (!zoneNum.ContainsKey(int.Parse(certNo.Substring(0, 2))))
            {
                return false;
            }

            //校验年份
            String year = certNo.Length == 15 ? "19" + certNo.Substring(6, 2) : certNo.Substring(6, 4);

            int iyear = int.Parse(year);
            if (iyear < 1900 || iyear > DateTime.Now.Year)
            {
                return false;//1900年的PASS，超过今年的PASS
            }
            //校验月份
            String month = certNo.Length == 15 ? certNo.Substring(8, 2) : certNo.Substring(10, 2);
            int imonth = int.Parse(month);
            if (imonth < 1 || imonth > 12)
            {
                return false;
            }

            //校验天数      
            String day = certNo.Length == 15 ? certNo.Substring(10, 2) : certNo.Substring(12, 2);
            int iday = int.Parse(day);
            if (iday < 1 || iday > 31)
            {
                return false;
            }
            //校验"校验码"
            if (certNo.Length == 15)
            {
                return true;
            }
            if (cs[cs.Length - 1] == PARITYBIT[power % 11])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据身份证号获取生日
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static DateTime GetBrithday(string IdCard)
        {
            string rtn = "1900-01-01";
            if (IdCard.Length == 15)
            {
                rtn = IdCard.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            }
            else if (IdCard.Length == 18)
            {
                rtn = IdCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            }

            return DateTime.Parse(rtn);
        }
        /// <summary>
        /// 根据身份证号获取生日
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static string GetBrithdayFromIdCard(string IdCard)
        {
            string rtn = "1900-01-01";
            if (IdCard.Length == 15)
            {
                rtn = IdCard.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            }
            else if (IdCard.Length == 18)
            {
                rtn = IdCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            }
            return rtn;
        }


        /// <summary>
        /// 根据身份证获取性别
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static string GetSexFromIdCard(string IdCard)
        {
            string rtn;
            string tmp = "";
            if (IdCard.Length == 15)
            {
                tmp = IdCard.Substring(IdCard.Length - 3);
            }
            else if (IdCard.Length == 18)
            {
                tmp = IdCard.Substring(IdCard.Length - 4);
                tmp = tmp.Substring(0, 3);
            }
            int sx = int.Parse(tmp);
            int outNum;
            Math.DivRem(sx, 2, out outNum);
            if (outNum == 0)
            {
                rtn = "女";
            }
            else
            {
                rtn = "男";
            }
            return rtn;
        }



        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDate">生日</param>
        /// <returns></returns>
        public static int GetAge(string IdCard)
        {
            string birthDay = GetBrithdayFromIdCard(IdCard);
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }
        #endregion
    }
}
