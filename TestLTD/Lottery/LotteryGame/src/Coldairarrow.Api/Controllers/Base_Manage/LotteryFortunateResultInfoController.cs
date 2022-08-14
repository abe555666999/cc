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
    public class LotteryFortunateResultInfoController : BaseApiController
    {
        #region DI

        public LotteryFortunateResultInfoController(ILotteryFortunateResultInfoBusiness lotteryFortunateResultInfoBus)
        {
            _lotteryFortunateResultInfoBus = lotteryFortunateResultInfoBus;
        }

        ILotteryFortunateResultInfoBusiness _lotteryFortunateResultInfoBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryFortunateResultInfo>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryFortunateResultInfoBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryFortunateResultInfo> GetTheData(IdInputDTO input)
        {
            return await _lotteryFortunateResultInfoBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryFortunateResultInfo data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryFortunateResultInfoBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryFortunateResultInfoBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryFortunateResultInfoBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}