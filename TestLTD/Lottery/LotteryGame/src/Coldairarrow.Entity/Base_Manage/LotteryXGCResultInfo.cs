using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// LotteryXGCResultInfo
    /// </summary>
    [Table("LotteryXGCResultInfo")]
    public class LotteryXGCResultInfo
    {

        /// <summary>
        /// Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// LotId
        /// </summary>
        public String LotId { get; set; }

        /// <summary>
        /// LotNumber
        /// </summary>
        public Int64 LotNumber { get; set; }

        /// <summary>
        /// LotResultNo
        /// </summary>
        public String LotResultNo { get; set; }

        /// <summary>
        /// LotSXResult
        /// </summary>
        public String LotSXResult { get; set; }

        /// <summary>
        /// LotWuXingResult
        /// </summary>
        public String LotWuXingResult { get; set; }

        /// <summary>
        /// LotSpecialCode
        /// </summary>
        public String LotSpecialCode { get; set; }

        /// <summary>
        /// CreatTime
        /// </summary>
        public DateTime? CreatTime { get; set; }

        /// <summary>
        /// BJCreatTime
        /// </summary>
        public DateTime? BJCreatTime { get; set; }

        /// <summary>
        /// LotResultTime
        /// </summary>
        public String LotResultTime { get; set; }

        /// <summary>
        /// IsResult
        /// </summary>
        public Boolean? IsResult { get; set; }

        /// <summary>
        /// LotType
        /// </summary>
        public Int32 LotType { get; set; }

        /// <summary>
        /// URLConfigId
        /// </summary>
        public String URLConfigId { get; set; }

        /// <summary>
        /// SourceName
        /// </summary>
        public String SourceName { get; set; }

    }
}