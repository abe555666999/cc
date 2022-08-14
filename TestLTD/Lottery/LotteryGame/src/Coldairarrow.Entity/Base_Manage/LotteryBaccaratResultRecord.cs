using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// LotteryBaccaratResultRecord
    /// </summary>
    [Table("LotteryBaccaratResultRecord")]
    public class LotteryBaccaratResultRecord
    {

        /// <summary>
        /// Id
        /// </summary>
        [Key, Column(Order = 1)]
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
        /// Pretend
        /// </summary>
        public String Pretend { get; set; }

        /// <summary>
        /// OutCome
        /// </summary>
        public String OutCome { get; set; }

        /// <summary>
        /// CreatTime
        /// </summary>
        public DateTime? CreatTime { get; set; }

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
        /// BJCreatTime
        /// </summary>
        public DateTime? BJCreatTime { get; set; }

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