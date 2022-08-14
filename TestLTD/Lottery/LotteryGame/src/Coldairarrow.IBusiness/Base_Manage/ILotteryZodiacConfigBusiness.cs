using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryZodiacConfigBusiness
    {
        Task<PageResult<LotteryZodiacConfig>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryZodiacConfig> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryZodiacConfig data);
        Task UpdateDataAsync(LotteryZodiacConfig data);
        Task DeleteDataAsync(List<string> ids);
        Task<LotteryZodiacConfig> GetLotZodiacConfigInfo(int openNo);
    }
}