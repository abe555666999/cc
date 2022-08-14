using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO.Lottery;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryXGCResultInfoBusiness
    {
        Task<PageResult<LotteryXGCResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryXGCResultInfo> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryXGCResultInfo data);
        Task UpdateDataAsync(LotteryXGCResultInfo data);
        Task DeleteDataAsync(List<string> ids);

        Task GetXGCOpenResultInfo();

        /// <summary>
        /// XGC Open Result
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        Task<XGCOpenResultDTO> GetXGCOpenResultList(long? lotNumber);

        /// <summary>
        /// 抓取香港彩数据
        /// </summary>
        /// <returns></returns>
        Task GetLotXGCOpenResultInfo();
    }
}