using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO.Lottery;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryCowsResultInfoBusiness
    {
        Task<PageResult<LotteryCowsResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryCowsResultInfo> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryCowsResultInfo data);
        Task UpdateDataAsync(LotteryCowsResultInfo data);
        Task DeleteDataAsync(List<string> ids);

        Task GetCowsOpenResultInfo();

        /// <summary>
        /// Cows 28 Open Result
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        Task<CowsOpenResultDTO> GetCowsOpenResultList(long? lotNumber);

        /// <summary>
        /// 抓取牛牛28数据
        /// </summary>
        /// <returns></returns>
        Task GetLotCowsOpenResultInfo();

        /// <summary>
        /// AddCowsOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task AddCowsOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list);
    }
}