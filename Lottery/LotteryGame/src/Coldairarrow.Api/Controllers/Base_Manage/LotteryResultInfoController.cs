using Coldairarrow.Api.Filters;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Base_Manage
{

    [Route("/Base_Manage/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LotteryResultInfoController : BaseApiController
    {
        #region DI

        public LotteryResultInfoController(ILotteryResultInfoBusiness lotteryResultInfoBus, ILotteryCowsResultInfoBusiness lotteryCowsResultInfoBus, ILotteryBaccaratResultInfoBusiness lotteryBaccaratResultInfoBus, ILotteryFortunateResultInfoBusiness lotteryFortunateResultInfoBus, ILotteryXGCResultInfoBusiness lotteryXGCResultInfoBus, ILotteryZodiacConfigBusiness lotteryZodiacConfigBus)
        {
            _lotteryResultInfoBus = lotteryResultInfoBus;
            _lotteryCowsResultInfoBus = lotteryCowsResultInfoBus;
            _lotteryBaccaratResultInfoBus = lotteryBaccaratResultInfoBus;
            _lotteryFortunateResultInfoBus = lotteryFortunateResultInfoBus;
            _lotteryXGCResultInfoBus = lotteryXGCResultInfoBus;
            _lotteryZodiacConfigBus = lotteryZodiacConfigBus;
        }

        readonly ILotteryResultInfoBusiness _lotteryResultInfoBus;
        readonly ILotteryCowsResultInfoBusiness _lotteryCowsResultInfoBus;
        readonly ILotteryBaccaratResultInfoBusiness _lotteryBaccaratResultInfoBus;
        readonly ILotteryFortunateResultInfoBusiness _lotteryFortunateResultInfoBus;
        readonly ILotteryXGCResultInfoBusiness _lotteryXGCResultInfoBus;
        readonly ILotteryZodiacConfigBusiness _lotteryZodiacConfigBus;

        #endregion

        #region 获取

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<PageResult<LotteryResultInfo>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryResultInfoBus.GetDataListAsync(input);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<LotteryResultInfo> GetTheData(IdInputDTO input)
        {
            return await _lotteryResultInfoBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region Lot

        /// <summary>
        /// 加拿大28(One)
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetCanadaOpenResultInfo()
        {
            await _lotteryResultInfoBus.GetCanadaOpenResultInfo();
            return Success();
        }


        /// <summary>
        /// 牛牛28
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetCowsOpenResultInfo()
        {
            await _lotteryCowsResultInfoBus.GetCowsOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 百家乐28
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetBaccaratOpenResultInfo()
        {
            await _lotteryBaccaratResultInfoBus.GetBaccaratOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 澳洲幸运28 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetFortunateOpenResultInfo()
        {
            await _lotteryFortunateResultInfoBus.GetFortunateOpenResultInfo();
            return Success();
        }

        /// <summary>
        /// 香港彩
        /// </summary>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetXGCOpenResultInfo()
        {
            await _lotteryXGCResultInfoBus.GetXGCOpenResultInfo();
            return Success();
        }


        /// <summary>
        /// 测试获取生肖信息
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet, CheckTaskSignAttribute, AllowAnonymous]
        public async Task<AjaxResult> GetLotZodiacConfigInfo(int num)
        {
            var entity = await _lotteryZodiacConfigBus.GetLotZodiacConfigInfo(num);
            return Success(entity);
        }

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


        #endregion

        #region 提交

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task SaveData(LotteryResultInfo data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryResultInfoBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryResultInfoBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryResultInfoBus.DeleteDataAsync(ids);
        }



        #endregion
    }
}