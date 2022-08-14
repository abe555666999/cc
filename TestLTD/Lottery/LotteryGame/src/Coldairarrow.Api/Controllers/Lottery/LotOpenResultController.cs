using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Threading.Tasks;
using System;
using Coldairarrow.Entity.DTO.Lottery;
using Microsoft.Extensions.Logging;
using Coldairarrow.Api.Filters;

namespace Coldairarrow.Api.Controllers.Lottery
{

    /// <summary>
    /// 开奖结果
    /// </summary>
    [Route("/Lottery/[controller]/[action]")]
    [ApiController]
    [OpenApiTag("开奖结果")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LotOpenResultController : BaseApiController
    {
        #region DI

        public LotOpenResultController(ILotteryResultInfoBusiness lotteryResultInfoBus, ILotteryCowsResultInfoBusiness lotteryCowsResultInfoBus, ILotteryBaccaratResultInfoBusiness lotteryBaccaratResultInfoBus, ILotteryFortunateResultInfoBusiness lotteryFortunateResultInfoBus, ILotteryXGCResultInfoBusiness lotteryXGCResultInfoBus, ILogger<LotOpenResultController> logger)
        {
            _lotteryResultInfoBus = lotteryResultInfoBus;
            _lotteryCowsResultInfoBus = lotteryCowsResultInfoBus;
            _lotteryBaccaratResultInfoBus = lotteryBaccaratResultInfoBus;
            _lotteryFortunateResultInfoBus = lotteryFortunateResultInfoBus;
            _lotteryXGCResultInfoBus = lotteryXGCResultInfoBus;
            _logger = logger;
        }

        readonly ILotteryResultInfoBusiness _lotteryResultInfoBus;
        readonly ILotteryCowsResultInfoBusiness _lotteryCowsResultInfoBus;
        readonly ILotteryBaccaratResultInfoBusiness _lotteryBaccaratResultInfoBus;
        readonly ILotteryFortunateResultInfoBusiness _lotteryFortunateResultInfoBus;
        readonly ILotteryXGCResultInfoBusiness _lotteryXGCResultInfoBus;
        readonly ILogger _logger;

        #endregion

        /// <summary>
        /// 获取加拿大28开奖结果(最多返回100条数据)
        /// </summary>
        /// <param name="lotNumber">期号(不传递期号：返回最新100条数据，传递>期号的数据)</param>
        /// <returns></returns>
        [HttpGet, CheckWebSignAttribute,  AllowAnonymous]
        public async Task<CanadaOpenResultDTO> GetCanadaOpenResultList(long? lotNumber)
        {
            return await _lotteryResultInfoBus.GetCanadaOpenResultList(lotNumber);
        }

        /// <summary>
        /// 获取牛牛28开奖结果(最多返回100条数据)
        /// </summary>
        /// <param name="lotNumber">期号(不传递期号：返回最新100条数据，传递>期号的数据)</param>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        [HttpGet, CheckWebSignAttribute, AllowAnonymous]
        public async Task<CowsOpenResultDTO> GetCowsOpenResultList(long? lotNumber)
        {
            return await _lotteryCowsResultInfoBus.GetCowsOpenResultList(lotNumber);
        }

        /// <summary>
        /// 获取百家乐28开奖结果(最多返回100条数据)
        /// </summary>
        /// <returns></returns>
        /// <param name="lotNumber">期号(不传递期号：返回最新100条数据，传递>期号的数据)</param>
        /// <exception cref="BusException"></exception>
        [HttpGet, CheckWebSignAttribute, AllowAnonymous]
        public async Task<BaccaratOpenResultDTO> GetBaccaratOpenResultList(long? lotNumber)
        {
            return await _lotteryBaccaratResultInfoBus.GetBaccaratOpenResultList(lotNumber);
        }

        /// <summary>
        /// 获取澳洲幸运28开奖结果(最多返回100条数据)
        /// </summary>
        /// <param name="lotNumber">期号(不传递期号：返回最新100条数据，传递>期号的数据)</param>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        [HttpGet, CheckWebSignAttribute, AllowAnonymous]
        public async Task<FortunateOpenResultDTO> GetFortunateOpenResultList(long? lotNumber)
        {
            return await _lotteryFortunateResultInfoBus.GetFortunateOpenResultList(lotNumber);
        }

        /// <summary>
        /// 获取香港六合彩开奖结果(最多返回100条数据)
        /// </summary>
        /// <param name="lotNumber">期号(不传递期号：返回最新100条数据，传递>期号的数据)</param>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        [HttpGet, CheckWebSignAttribute, AllowAnonymous]
        public async Task<XGCOpenResultDTO> GetXGCOpenResultList(long? lotNumber)
        {
            return await _lotteryXGCResultInfoBus.GetXGCOpenResultList(lotNumber);
        }

        /// <summary>
        /// 加拿大28
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckRequestSignAttribute, AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<AjaxResult> GetCanadaOpenResultInfo()
        {
            await _lotteryResultInfoBus.GetCanadaOpenResultInfo();

            _logger.LogInformation($"GetCanadaOpenResultInfo:{DateTime.Now}");

            return Success();
        }


    }
}
