using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryCowsResultRecoreBusiness
    {
        Task<PageResult<LotteryCowsResultRecore>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryCowsResultRecore> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryCowsResultRecore data);
        Task UpdateDataAsync(LotteryCowsResultRecore data);
        Task DeleteDataAsync(List<string> ids);
    }
}