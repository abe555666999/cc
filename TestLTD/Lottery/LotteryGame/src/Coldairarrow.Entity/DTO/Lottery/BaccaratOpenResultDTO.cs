using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Entity.DTO.Lottery
{

    /// <summary>
    /// 百家乐28
    /// </summary>
    public class BaccaratOpenResultDTO
    {
        /// <summary>
        /// 开奖结果
        /// </summary>
        public List<BaccaratOpenResutInfo> List { get; set; }

        /// <summary>
        /// 下期开奖信息
        /// </summary>
        public BaccaratNextNoOpenTimeInfo NextDrawInfo { get; set; }
    }

    public class BaccaratNextNoOpenTimeInfo
    {
        /// <summary>
        /// 开奖间隔单位秒
        /// </summary>
        public int Intervals { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 下期开奖时间
        /// </summary>
        public string NextDrawTime { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime { get; set; }

        /// <summary>
        /// 当前时间(校对后的北京时间)
        /// </summary>
        public string CurrentBJTime { get; set; }
    }

    [Map(typeof(LotteryBaccaratResultInfo))]
    public class BaccaratOpenResutInfo
    {

        /// <summary>
        /// 期号
        /// </summary>
        public long LotNumber { get; set; }

        /// <summary>
        /// 开奖号
        /// </summary>
        public String LotResultNo { get; set; }

        /// <summary>
        /// 庄闲
        /// </summary>
        public String Pretend { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public String OutCome { get; set; }

        /// <summary>
        /// CreatTime
        /// </summary>
        // public DateTime? CreatTime { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
       // public String LotResultTime { get; set; }
    }
}
