using Coldairarrow.Api.Filters;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using EFCore.Sharding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Coldairarrow.Util.Configuration;
using System.Collections.Generic;

namespace Coldairarrow.Api.Controllers
{
    [Route("/Test/[controller]/[action]")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    [OpenApiTag("测试")]
    public class TestController : BaseController
    {
        readonly IDbAccessor _repository;
        readonly ILogger _logger;

        public TestController(IDbAccessor repository, ILogger<TestController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task PressTest()
        {
            Base_User base_User = new Base_User
            {
                Id = Guid.NewGuid().ToString(),
                Birthday = DateTime.Now,
                CreateTime = DateTime.Now,
                CreatorId = Guid.NewGuid().ToString(),
                DepartmentId = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                RealName = Guid.NewGuid().ToString(),
                Sex = Sex.Man,
                UserName = Guid.NewGuid().ToString()
            };

            await _repository.InsertAsync(base_User);
            await _repository.UpdateAsync(base_User);
            await _repository.GetIQueryable<Base_User>().Where(x => x.Id == base_User.Id).FirstOrDefaultAsync();
            await _repository.DeleteAsync(base_User);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<PageResult<Base_UserLog>> GetLogList()
        {
            return await _repository.GetIQueryable<Base_UserLog>().GetPageResultAsync(new PageInput());
        }

        /// <summary>
        /// 测试模拟获取签名信息
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="secret">secret</param>
        /// <param name="Authorization">token</param>
        /// <param name="randomNumber">随机数</param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> TestGetSign(string appId, string secret, string Authorization, string randomNumber)
        {
            //[(appId:appId secret:secret token  randomNumber:随机数]  MD5(appId&secret&token&randomNumber)

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(appId);
            stringBuilder.Append("&");
            stringBuilder.Append(secret);
            stringBuilder.Append("&");
            stringBuilder.Append(Authorization);
            stringBuilder.Append("&");
            stringBuilder.Append(randomNumber);

            var signUPStr = stringBuilder.ToString();

            var signStr = ConvertHelper.GetSignInfo(appId, secret, Authorization, randomNumber);

            string strJson = $"签名字符：{signUPStr} -- 前面后的字符：{signStr}";

            return await Task.FromResult(new ContentResult
            {
                Content = strJson,
                ContentType = "application/json"
            });
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
       // [HttpGet, CheckWebSignAttribute, AllowAnonymous]
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public AjaxResult GetTestInfo()
        {
            var request = HttpContext.Request;

            string randomNumberStr = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            string appId = request.Headers["AppId"].ToString();
            string secret = request.Headers["Secret"].ToString();
            var authorization = request.Headers["Authorization"];
            string randomNumber = request.Headers["RandomNumber"].ToString();
            string sign = request.Headers["Sign"].ToString();


            var secretInfo = AppSetting.SecretInfo;

            var exectStr = $"GetTestInfo  - appId:{appId} - secret:{secret} - authorization:{authorization}  - randomNumber:{randomNumber}-  sign:{sign}";

            var currentData = DateTime.Now;

            var curYear = currentData.Year;
            var curMouth = currentData.Month;

            _logger.LogInformation(exectStr);
            return Success();
        }


        /// <summary>
        /// 测试发起请求
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public AjaxResult GetTestSendAsk(string appId, string secret, string authorization, string randomNumber)
        {

            var headDic = new Dictionary<string, string>();

            var signStr = ConvertHelper.GetSignInfo(appId, secret, authorization, randomNumber);

            headDic.Add("AppId", appId);
            headDic.Add("Secret", secret);
            headDic.Add("Authorization", authorization);
            headDic.Add("RandomNumber", randomNumber);
            headDic.Add("Sign", signStr);

            var result = HttpHelper.HttpGet("http://localhost:5000/Test/Test/GetTestInfo", headDic);

            var exectStr = $"GetTestInfo randomNumber:{randomNumber} - appId:{appId} - secret:{secret} - sign:{signStr} - token:{authorization} -- Result:{Convert.ToString(result)}";

            _logger.LogInformation(exectStr);

            return Success();
        }

        /// <summary>
        /// 测试获取当前时间
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public IActionResult GetCurrentDateTime()
        {
            var nowTime = DateTime.Now;
            var curTime = $"当前时间:{nowTime.ToString("yyyy-MM-dd HH:mm:ss")}";

            var bjTime = nowTime.AddHours(AppSetting.TimeZoneHour).ToString("yyyy-MM-dd HH:mm:ss");

            var curTaday = DateTime.Today.DayOfWeek.ToString();

            var timeStr = $" 时区时间:{curTime}  -- 校对后的北京时间:{bjTime}";

            return Content(timeStr);
        }

    }
}