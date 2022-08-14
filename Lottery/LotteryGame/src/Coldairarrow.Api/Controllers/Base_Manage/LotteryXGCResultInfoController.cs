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
    public class LotteryXGCResultInfoController : BaseApiController
    {
        #region DI

        public LotteryXGCResultInfoController(ILotteryXGCResultInfoBusiness lotteryXGCResultInfoBus)
        {
            _lotteryXGCResultInfoBus = lotteryXGCResultInfoBus;
        }

        ILotteryXGCResultInfoBusiness _lotteryXGCResultInfoBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryXGCResultInfo>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryXGCResultInfoBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryXGCResultInfo> GetTheData(IdInputDTO input)
        {
            return await _lotteryXGCResultInfoBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryXGCResultInfo data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryXGCResultInfoBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryXGCResultInfoBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryXGCResultInfoBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}