using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO.Lottery;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryFortunateResultInfoBusiness
    {
        Task<PageResult<LotteryFortunateResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryFortunateResultInfo> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryFortunateResultInfo data);
        Task UpdateDataAsync(LotteryFortunateResultInfo data);
        Task DeleteDataAsync(List<string> ids);
        Task GetFortunateOpenResultInfo();
        Task<FortunateOpenResultDTO> GetFortunateOpenResultList(long? lotNumber);

        /// <summary>
        /// 抓取澳洲幸运28数据
        /// </summary>
        /// <returns></returns>
        Task GetLotFortunateOpenResultInfo();
    }
}