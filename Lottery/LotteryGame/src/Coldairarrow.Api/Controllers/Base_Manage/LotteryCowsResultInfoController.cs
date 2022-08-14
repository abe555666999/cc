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
    public class LotteryCowsResultInfoController : BaseApiController
    {
        #region DI

        public LotteryCowsResultInfoController(ILotteryCowsResultInfoBusiness lotteryCowsResultInfoBus)
        {
            _lotteryCowsResultInfoBus = lotteryCowsResultInfoBus;
        }

        ILotteryCowsResultInfoBusiness _lotteryCowsResultInfoBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryCowsResultInfo>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryCowsResultInfoBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryCowsResultInfo> GetTheData(IdInputDTO input)
        {
            return await _lotteryCowsResultInfoBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryCowsResultInfo data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryCowsResultInfoBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryCowsResultInfoBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryCowsResultInfoBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}