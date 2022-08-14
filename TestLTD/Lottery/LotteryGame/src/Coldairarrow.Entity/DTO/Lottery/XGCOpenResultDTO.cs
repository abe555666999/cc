using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Entity.DTO.Lottery
{
    public class XGCOpenResultDTO
    {
        /// <summary>
        /// 开奖结果
        /// </summary>
        public List<XGCOpenResutInfo> List { get; set; }

        /// <summary>
        /// 下期开奖信息
        /// </summary>
        public XGCNextNoOpenTimeInfo NextDrawInfo { get; set; }

        /// <summary>
        /// 未开奖信息
        /// </summary>
        public XGCNeedOpenInfo NeedOpenInfo { get; set; }
    }

    public class XGCNextNoOpenTimeInfo
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

    /// <summary>
    /// 未开奖信息
    /// </summary>
    public class XGCNeedOpenInfo
    {
        /// <summary>
        /// 未开奖期号
        /// </summary>
        public Int64 NeedLotNumber { get; set; }
    }


    [Map(typeof(LotteryXGCResultInfo))]
    public class XGCOpenResutInfo
    {

        /// <summary>
        /// 期号
        /// </summary>
        public Int64 LotNumber { get; set; }

        /// <summary>
        /// 开奖号
        /// </summary>
        public String LotResultNo { get; set; }

        /// <summary>
        /// 生肖
        /// </summary>
        public String LotSXResult { get; set; }

        /// <summary>
        /// 五行
        /// </summary>
        public String LotWuXingResult { get; set; }

        /// <summary>
        /// 特码号
        /// </summary>
        public String LotSpecialCode { get; set; }

        /// <summary>
        /// CreatTime
        /// </summary>
        public DateTime? CreatTime { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
        public String LotResultTime { get; set; }

    }
}
