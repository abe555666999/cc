using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO.Lottery;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryBaccaratResultInfoBusiness
    {
        Task<PageResult<LotteryBaccaratResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryBaccaratResultInfo> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryBaccaratResultInfo data);
        Task UpdateDataAsync(LotteryBaccaratResultInfo data);
        Task DeleteDataAsync(List<string> ids);

        /// <summary>
        /// Baccarat 28
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        Task GetBaccaratOpenResultInfo();

        /// <summary>
        /// 获取百家乐28开奖结果
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        Task<BaccaratOpenResultDTO> GetBaccaratOpenResultList(long? lotNumber);

        /// <summary>
        /// 抓取百家乐28数据
        /// </summary>
        /// <returns></returns>
        Task GetLotBaccaratOpenResultInfo();

        /// <summary>
        /// AddBaccaratOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task AddBaccaratOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list);
    }
}