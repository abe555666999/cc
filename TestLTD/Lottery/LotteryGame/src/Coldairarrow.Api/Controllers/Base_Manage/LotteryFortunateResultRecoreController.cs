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
    public class LotteryFortunateResultRecoreController : BaseApiController
    {
        #region DI

        public LotteryFortunateResultRecoreController(ILotteryFortunateResultRecoreBusiness lotteryFortunateResultRecoreBus)
        {
            _lotteryFortunateResultRecoreBus = lotteryFortunateResultRecoreBus;
        }

        ILotteryFortunateResultRecoreBusiness _lotteryFortunateResultRecoreBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryFortunateResultRecore>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryFortunateResultRecoreBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryFortunateResultRecore> GetTheData(IdInputDTO input)
        {
            return await _lotteryFortunateResultRecoreBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryFortunateResultRecore data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryFortunateResultRecoreBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryFortunateResultRecoreBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryFortunateResultRecoreBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}