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
    public class LotteryXGCResultRecoreController : BaseApiController
    {
        #region DI

        public LotteryXGCResultRecoreController(ILotteryXGCResultRecoreBusiness lotteryXGCResultRecoreBus)
        {
            _lotteryXGCResultRecoreBus = lotteryXGCResultRecoreBus;
        }

        ILotteryXGCResultRecoreBusiness _lotteryXGCResultRecoreBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryXGCResultRecore>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryXGCResultRecoreBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryXGCResultRecore> GetTheData(IdInputDTO input)
        {
            return await _lotteryXGCResultRecoreBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryXGCResultRecore data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryXGCResultRecoreBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryXGCResultRecoreBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryXGCResultRecoreBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}