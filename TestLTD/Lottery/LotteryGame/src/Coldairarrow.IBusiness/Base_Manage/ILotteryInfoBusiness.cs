using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryInfoBusiness
    {
        Task<PageResult<LotteryInfo>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryInfo> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryInfo data);
        Task UpdateDataAsync(LotteryInfo data);
        Task DeleteDataAsync(List<string> ids);

        Task<List<LotteryInfo>> GetLotteryInfoList();



    }
}