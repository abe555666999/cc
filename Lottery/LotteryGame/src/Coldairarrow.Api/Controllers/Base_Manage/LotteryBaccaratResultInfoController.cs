using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Base_Manage
{
    [Route("/Base_Manage/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LotteryBaccaratResultInfoController : BaseApiController
    {
        #region DI

        public LotteryBaccaratResultInfoController(ILotteryBaccaratResultInfoBusiness lotteryBaccaratResultInfoBus)
        {
            _lotteryBaccaratResultInfoBus = lotteryBaccaratResultInfoBus;
        }

        ILotteryBaccaratResultInfoBusiness _lotteryBaccaratResultInfoBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryBaccaratResultInfo>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryBaccaratResultInfoBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryBaccaratResultInfo> GetTheData(IdInputDTO input)
        {
            return await _lotteryBaccaratResultInfoBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryBaccaratResultInfo data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryBaccaratResultInfoBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryBaccaratResultInfoBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryBaccaratResultInfoBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}