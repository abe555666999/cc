using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryFortunateResultRecoreBusiness
    {
        Task<PageResult<LotteryFortunateResultRecore>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryFortunateResultRecore> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryFortunateResultRecore data);
        Task UpdateDataAsync(LotteryFortunateResultRecore data);
        Task DeleteDataAsync(List<string> ids);
    }
}