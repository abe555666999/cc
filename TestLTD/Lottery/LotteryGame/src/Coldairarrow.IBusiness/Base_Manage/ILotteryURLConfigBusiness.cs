using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryURLConfigBusiness
    {
        Task<PageResult<LotteryURLConfig>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryURLConfig> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryURLConfig data);
        Task UpdateDataAsync(LotteryURLConfig data);
        Task DeleteDataAsync(List<string> ids);
    }
}