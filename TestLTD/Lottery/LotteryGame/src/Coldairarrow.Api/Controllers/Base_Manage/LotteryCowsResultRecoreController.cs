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
    public class LotteryCowsResultRecoreController : BaseApiController
    {
        #region DI

        public LotteryCowsResultRecoreController(ILotteryCowsResultRecoreBusiness lotteryCowsResultRecoreBus)
        {
            _lotteryCowsResultRecoreBus = lotteryCowsResultRecoreBus;
        }

        ILotteryCowsResultRecoreBusiness _lotteryCowsResultRecoreBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<LotteryCowsResultRecore>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _lotteryCowsResultRecoreBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<LotteryCowsResultRecore> GetTheData(IdInputDTO input)
        {
            return await _lotteryCowsResultRecoreBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(LotteryCowsResultRecore data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _lotteryCowsResultRecoreBus.AddDataAsync(data);
            }
            else
            {
                await _lotteryCowsResultRecoreBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _lotteryCowsResultRecoreBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}