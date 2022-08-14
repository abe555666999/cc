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
    public class LotteryBaccaratResultRecordController : BaseApiController
    {
        #region DI

        public LotteryBaccaratResultRecordController(ILotteryBaccaratResultRecordBusiness lotteryBaccaratResultRecordBus)
        {
            _lotteryBaccaratResultRecordBus = lotteryBaccaratResultRecordBus;
        }

        ILotteryBaccaratResultRecordBusiness _lotteryBaccaratResultRecordBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryBaccaratResultRecord>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryBaccaratResultRecordBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryBaccaratResultRecord> GetTheData(IdInputDTO input)
        {
            return await _lotteryBaccaratResultRecordBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryBaccaratResultRecord data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryBaccaratResultRecordBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryBaccaratResultRecordBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryBaccaratResultRecordBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}