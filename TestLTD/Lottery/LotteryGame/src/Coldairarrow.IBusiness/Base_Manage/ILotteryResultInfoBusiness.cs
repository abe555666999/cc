using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO.Lottery;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface ILotteryResultInfoBusiness
    {
        Task<PageResult<LotteryResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<LotteryResultInfo> GetTheDataAsync(string id);
        Task AddDataAsync(LotteryResultInfo data);
        Task UpdateDataAsync(LotteryResultInfo data);
        Task DeleteDataAsync(List<string> ids);
        Task GetCanadaOpenResultInfo();

        // <summary>
        /// 抓取去加拿大28数据
        /// </summary>
        /// <returns></returns>
        Task GetLotCanadaOpenResultInfo();

        Task<CanadaOpenResultDTO> GetCanadaOpenResultList(long? lotNumber);

        Task<string> GetNextDrawTime(string lastWestExpectTime, LotteryInfo lotInfo);

        /// <summary>
        /// 加拿大28、牛牛28 、百家乐28
        /// </summary>
        /// <returns></returns>
        Task GetLotManyOpenResultInfo();
    }
}