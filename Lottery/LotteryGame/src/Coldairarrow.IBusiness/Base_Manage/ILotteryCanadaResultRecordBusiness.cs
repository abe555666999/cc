using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryCanadaResultRecordBusiness
    {
        Task<PageResult<LotteryCanadaResultRecord>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryCanadaResultRecord> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryCanadaResultRecord data);
        Task UpdateDataAsync(LotteryCanadaResultRecord data);
        Task DeleteDataAsync(List<string> ids);
    }
}