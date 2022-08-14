using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryBaccaratResultRecordBusiness
    {
        Task<PageResult<LotteryBaccaratResultRecord>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryBaccaratResultRecord> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryBaccaratResultRecord data);
        Task UpdateDataAsync(LotteryBaccaratResultRecord data);
        Task DeleteDataAsync(List<string> ids);
    }
}