using System;
using System.Text;
namespace Coldairarrow.Util.Helper
{
    public class ConvertHelper
    {

        /// <summary>
        /// Sign (appId&secret&token&randomNumber)
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="token"></param>
        /// <param name="randomNumber"></param>
        /// <returns></returns>
        public static string GetSignInfo(string appId, string secret, string token, string randomNumber)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(appId);
            stringBuilder.Append("&");
            stringBuilder.Append(secret);
            stringBuilder.Append("&");
            stringBuilder.Append(token);
            stringBuilder.Append("&");
            stringBuilder.Append(randomNumber);

            var sign = stringBuilder.ToString().ToMD5String();

            return sign.ToUpper();
        }

        /// <summary>
        /// 当前是第几周
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public static int GetWeekNumInMouth(DateTime dayTime)
        {
            int dayInMouth = dayTime.Day;

            //本月第一天
            DateTime firstDay = dayTime.AddDays(1 - dayTime.Day);
            //本月第一天是周几
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;

            //本月第一周有几天
            int firstWeekEndDay = 7 - (weekday - 1);

            //当前日期和第一周之差
            int diffday = dayInMouth - firstWeekEndDay;

            diffday = diffday > 0 ? diffday : 1;

            //当前是第几周，如果整除7就减1天
            int WeekNumInMouth = ((diffday % 7) == 0
                ? (diffday / 7 - 1)
                : (diffday / 7)) + 1 + (dayInMouth > firstWeekEndDay ? 1 : 0);

            return WeekNumInMouth;

        }

        public static string GetWeek(string weekName)
        {
            string week="";
            switch (weekName)
            {
                case "Sunday":
                    week = "星期日";
                    break;
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期五";
                    break;
            }
            return week;
        }

        public static string GetCowsResultNo(int num)
        {
            var result = "";
            switch (num)
            {
                case 0:
                    result = "牛牛";
                    break;
                case 1:
                    result = "牛一";
                    break;
                case 2:
                    result = "牛二";
                    break;
                case 3:
                    result = "牛三";
                    break;
                case 4:
                    result = "牛四";
                    break;
                case 5:
                    result = "牛五";
                    break;
                case 6:
                    result = "牛六";
                    break;
                case 7:
                    result = "牛七";
                    break;
                case 8:
                    result = "牛八";
                    break;
                case 9:
                    result = "牛九";
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string GetBaccaratZXD(string[] str) {
            var result = "";
            if (str.Length>0 && str.Length==3)
            {
                var one = Convert.ToInt32(str[0]);
                var two = Convert.ToInt32(str[1]);
                var three = Convert.ToInt32(str[2]);

                if (one==two)
                {
                    result = "庄对";
                }
                if (two == three)
                {
                    result = "闲对";
                }
                if (one==three)
                {
                    return "和";
                }
            }
            return result;
        }

        public static string GetBaccaratResult(string[] str)
        {
            var result = "";
            if (str.Length > 0 && str.Length == 3)
            {
                var one = Convert.ToInt32(str[0]);
                var two = Convert.ToInt32(str[1]);
                var three = Convert.ToInt32(str[2]);

                if (one > three)   //庄赢
                {
                    result = "庄赢";
                }
                if (three > one)    //闲赢
                {
                    result = "闲赢";
                }
                if (one == three)
                {
                    return "和";
                }
            }
            return result;
        }
    }
}
