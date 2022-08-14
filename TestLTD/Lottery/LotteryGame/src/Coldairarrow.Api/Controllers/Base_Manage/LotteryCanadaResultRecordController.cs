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
    public class LotteryCanadaResultRecordController : BaseApiController
    {
        #region DI

        public LotteryCanadaResultRecordController(ILotteryCanadaResultRecordBusiness lotteryCanadaResultRecordBus)
        {
            _lotteryCanadaResultRecordBus = lotteryCanadaResultRecordBus;
        }

        ILotteryCanadaResultRecordBusiness _lotteryCanadaResultRecordBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryCanadaResultRecord>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryCanadaResultRecordBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryCanadaResultRecord> GetTheData(IdInputDTO input)
        {
            return await _lotteryCanadaResultRecordBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryCanadaResultRecord data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryCanadaResultRecordBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryCanadaResultRecordBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryCanadaResultRecordBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}