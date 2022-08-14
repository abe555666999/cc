using Quartz.NET.Web.Extensions;
using System.Text;

namespace Quartz.NET.Web.Utility
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
    }
}
