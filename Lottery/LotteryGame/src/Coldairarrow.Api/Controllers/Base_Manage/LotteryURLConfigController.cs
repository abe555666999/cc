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
    public class LotteryURLConfigController : BaseApiController
    {
        #region DI

        public LotteryURLConfigController(ILotteryURLConfigBusiness lotteryURLConfigBus)
        {
            _lotteryURLConfigBus = lotteryURLConfigBus;
        }

        ILotteryURLConfigBusiness _lotteryURLConfigBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryURLConfig>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryURLConfigBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryURLConfig> GetTheData(IdInputDTO input)
        {
            return await _lotteryURLConfigBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryURLConfig data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryURLConfigBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryURLConfigBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryURLConfigBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}