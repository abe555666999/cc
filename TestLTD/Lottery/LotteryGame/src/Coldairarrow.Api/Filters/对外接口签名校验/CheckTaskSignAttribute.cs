using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Coldairarrow.Util.Configuration;
using Coldairarrow.Util.Helper;

namespace Coldairarrow.Api.Filters
{
    public class CheckTaskSignAttribute : BaseActionFilterAsync
    {
        /// <summary>
        /// Action执行之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public async override Task OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断是否需要签名
            if (filterContext.ContainsFilter<IgnoreSignAttribute>())
                return;
            var request = filterContext.HttpContext.Request;
            IServiceProvider serviceProvider = filterContext.HttpContext.RequestServices;
            IBase_AppSecretBusiness appSecretBus = serviceProvider.GetService<IBase_AppSecretBusiness>();
            ILogger logger = serviceProvider.GetService<ILogger<CheckWebSignAttribute>>();
            var cache = serviceProvider.GetService<IDistributedCache>();

            var secretInfo = AppSetting.SecretInfo;

            string appId = request.Headers["AppId"].ToString();
            string secret = request.Headers["Secret"].ToString();
            string token = request.Headers["Authorization"].ToString();
            string randomNumber = request.Headers["RandomNumber"].ToString();
            string sign = request.Headers["Sign"].ToString();

            if (appId.IsNullOrEmpty())
            {
                ReturnError("缺少Header:AppId");
                return;
            }
            if (secret.IsNullOrEmpty())
            {
                ReturnError("缺少Header:Secret");
                return;
            }
            if (token.IsNullOrEmpty())
            {
                ReturnError("缺少Header:Authorization");
                return;
            }
            if (randomNumber.IsNullOrEmpty())
            {
                ReturnError("缺少Header:RandomNumber");
                return;
            }
            if (sign.IsNullOrEmpty())
            {
                ReturnError("缺少Header:Sign");
                return;
            }

            if (appId != secretInfo.AppId)
            {
                ReturnError("Header:appId不正确");
                return;
            }
            if (secret != secretInfo.Secret)
            {
                ReturnError("Header:appId不正确");
                return;
            }
            if (token != secretInfo.Authorization)
            {
                ReturnError("Header:Authorization不正确");
                return;
            }
            if (appId != secretInfo.AppId)
            {
                ReturnError("Header:appId不正确");
                return;
            }
            if (appId != secretInfo.AppId)
            {
                ReturnError("Header:appId不正确");
                return;
            }

            request.EnableBuffering();
            string body = await request.Body.ReadToStringAsync();

            var signStr = ConvertHelper.GetSignInfo(appId, secret, token, randomNumber);
            if (sign.ToUpper() != signStr)
            {
                logger.LogError($"请求信息：headers:{request.Headers.ToJson()}, body:{body}");

                ReturnError("Header:Sign不正确");
                return;
            }

            void ReturnError(string msg)
            {
                filterContext.Result = Error(msg);
            }
        }

        /// <summary>
        /// 生成接口签名sign
        /// 注：md5(appId+time+guid+body+appSecret)
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="appSecret">应用密钥</param>
        /// <param name="guid">唯一GUID</param>
        /// <param name="time">时间</param>
        /// <param name="body">请求体</param>
        /// <returns></returns>
        private string BuildApiSign(string appId, string appSecret, string guid, DateTime time, string body)
        {
            return $"{appId}{time.ToString("yyyy-MM-dd HH:mm:ss")}{guid}{body}{appSecret}".ToMD5String();
        }

    }
}