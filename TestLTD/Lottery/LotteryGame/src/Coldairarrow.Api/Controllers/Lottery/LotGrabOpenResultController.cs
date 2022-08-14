using Coldairarrow.Api.Filters;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace Coldairarrow.Api.Controllers.Lottery
{
    /// <summary>
    /// 抓取数据
    /// </summary>
    [Route("/Lottery/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [OpenApiTag("抓取数据")]
    public class LotGrabOpenResultController : BaseApiController
    {

        #region DI

        public LotGrabOpenResultController(ILotteryResultInfoBusiness lotteryResultInfoBus, ILotteryCowsResultInfoBusiness lotteryCowsResultInfoBus, ILotteryBaccaratResultInfoBusiness lotteryBaccaratResultInfoBus, ILotteryFortunateResultInfoBusiness lotteryFortunateResultInfoBus, ILotteryXGCResultInfoBusiness lotteryXGCResultInfoBus, ILotteryZodiacConfigBusiness lotteryZodiacConfigBus, ILotteryInfoBusiness lotteryInfoBus)
        {
            _lotteryInfoBus = lotteryInfoBus;
            _lotteryResultInfoBus = lotteryResultInfoBus;
            _lotteryCowsResultInfoBus = lotteryCowsResultInfoBus;
            _lotteryBaccaratResultInfoBus = lotteryBaccaratResultInfoBus;
            _lotteryFortunateResultInfoBus = lotteryFortunateResultInfoBus;
            _lotteryXGCResultInfoBus = lotteryXGCResultInfoBus;
            _lotteryZodiacConfigBus = lotteryZodiacConfigBus;
        }

        readonly ILotteryInfoBusiness _lotteryInfoBus;
        readonly ILotteryResultInfoBusiness _lotteryResultInfoBus;
        readonly ILotteryCowsResultInfoBusiness _lotteryCowsResultInfoBus;
        readonly ILotteryBaccaratResultInfoBusiness _lotteryBaccaratResultInfoBus;
        readonly ILotteryFortunateResultInfoBusiness _lotteryFortunateResultInfoBus;
        readonly ILotteryXGCResultInfoBusiness _lotteryXGCResultInfoBus;
        readonly ILotteryZodiacConfigBusiness _lotteryZodiacConfigBus;

        #endregion

        #region LotAPI

        /// <summary>
        /// 抓取去加拿大28数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetLotCanadaOpenResultInfo()
        {
            await _lotteryResultInfoBus.GetLotCanadaOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 抓取牛牛28数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetLotCowsOpenResultInfo()
        {
            await _lotteryCowsResultInfoBus.GetLotCowsOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 抓取百家乐28数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetLotBaccaratOpenResultInfo()
        {
            await _lotteryBaccaratResultInfoBus.GetLotBaccaratOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 抓取澳洲幸运28数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetLotFortunateOpenResultInfo()
        {
            await _lotteryFortunateResultInfoBus.GetLotFortunateOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 抓取香港彩数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetLotXGCOpenResultInfo()
        {
            await _lotteryXGCResultInfoBus.GetLotXGCOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 加拿大28、牛牛28 、百家乐28
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<AjaxResult> GetLotManyOpenResultInfo()
        {
            await _lotteryResultInfoBus.GetLotManyOpenResultInfo();
            return Success();
        }

       
        #endregion

    }
}
