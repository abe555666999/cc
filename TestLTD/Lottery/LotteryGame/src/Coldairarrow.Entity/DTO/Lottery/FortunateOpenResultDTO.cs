using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
namespace Coldairarrow.Entity.DTO.Lottery
{
    public class FortunateOpenResultDTO
    {
        /// <summary>
        /// 开奖结果
        /// </summary>
        public List<FortunateOpenResutInfo> List { get; set; }

        /// <summary>
        /// 下期开奖信息
        /// </summary>
        public FortunateNextNoOpenTimeInfo NextDrawInfo { get; set; }
    }

    public class FortunateNextNoOpenTimeInfo
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

    [Map(typeof(LotteryFortunateResultInfo))]
    public class FortunateOpenResutInfo
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
        /// CreatTime
        /// </summary>
        // public DateTime? CreatTime { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
        public String LotResultTime { get; set; }
    }
}

