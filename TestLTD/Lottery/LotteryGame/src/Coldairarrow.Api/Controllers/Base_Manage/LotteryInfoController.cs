using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;

namespace Coldairarrow.Api.Controllers.Base_Manage
{
    [Route("/Base_Manage/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LotteryInfoController : BaseApiController
    {
        #region DI

        public LotteryInfoController(ILotteryInfoBusiness lotteryInfoBus)
        {
            _lotteryInfoBus = lotteryInfoBus;
        }

        ILotteryInfoBusiness _lotteryInfoBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryInfo>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryInfoBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryInfo> GetTheData(IdInputDTO input)
        {
            return await _lotteryInfoBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryInfo data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);
                if (data!=null)
                {
                    data.IsEnable = true;
                    data.CreatTime = DateTime.Now;
                }

                await _lotteryInfoBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryInfoBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryInfoBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}