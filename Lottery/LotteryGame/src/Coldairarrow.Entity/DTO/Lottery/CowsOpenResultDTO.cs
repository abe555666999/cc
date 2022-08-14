using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;

namespace Coldairarrow.Entity.DTO.Lottery
{

    /// <summary>
    /// 牛牛28
    /// </summary>
    public class CowsOpenResultDTO
    {
        /// <summary>
        /// 开奖结果
        /// </summary>
        public List<CowsOpenResutInfo> List { get; set; }

        /// <summary>
        /// 下期开奖信息
        /// </summary>
        public CowsNextNoOpenTimeInfo NextDrawInfo { get; set; }
    }

    [Map(typeof(LotteryCowsResultInfo))]
    public class CowsOpenResutInfo
    {
        /// <summary>
        /// 期号
        /// </summary>
        public long LotNumber { get; set; }

        /// <summary>
        /// 开奖号码1
        /// </summary>
        public String LotResultOneNo { get; set; }

        /// <summary>
        /// 开奖号码2
        /// </summary>
        public String LotResultTwoNo { get; set; }

        /// <summary>
        /// 开奖号码3
        /// </summary>
        public String LotResultThreeNo { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
        // public String LotResultTime { get; set; }
    }

    public class CowsNextNoOpenTimeInfo
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


}
