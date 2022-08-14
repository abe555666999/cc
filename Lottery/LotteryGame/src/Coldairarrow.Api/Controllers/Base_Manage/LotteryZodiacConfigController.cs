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
    public class LotteryZodiacConfigController : BaseApiController
    {
        #region DI

        public LotteryZodiacConfigController(ILotteryZodiacConfigBusiness lotteryZodiacConfigBus)
        {
            _lotteryZodiacConfigBus = lotteryZodiacConfigBus;
        }

        ILotteryZodiacConfigBusiness _lotteryZodiacConfigBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryZodiacConfig>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryZodiacConfigBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryZodiacConfig> GetTheData(IdInputDTO input)
        {
            return await _lotteryZodiacConfigBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryZodiacConfig data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryZodiacConfigBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryZodiacConfigBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryZodiacConfigBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}