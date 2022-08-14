using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryXGCResultRecoreBusiness
    {
        Task<PageResult<LotteryXGCResultRecore>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryXGCResultRecore> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryXGCResultRecore data);
        Task UpdateDataAsync(LotteryXGCResultRecore data);
        Task DeleteDataAsync(List<string> ids);
    }
}