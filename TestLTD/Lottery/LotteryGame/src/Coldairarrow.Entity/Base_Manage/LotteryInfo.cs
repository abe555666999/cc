using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// LotteryInfo
    /// </summary>
    [Table("LotteryInfo")]
    public class LotteryInfo
    {

        /// <summary>
        /// Id
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// LotName
        /// </summary>
        public String LotName { get; set; }

        /// <summary>
        /// LotType
        /// </summary>
        public Int32? LotType { get; set; }

        /// <summary>
        /// CreatTime
        /// </summary>
        public DateTime CreatTime { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// GrabURL
        /// </summary>
        public String GrabURL { get; set; }

        /// <summary>
        /// IsEnable
        /// </summary>
        public Boolean IsEnable { get; set; }

        /// <summary>
        /// Intervals
        /// </summary>
        public Int32? Intervals { get; set; }

        /// <summary>
        /// StartTime
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// EndTime
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public Int32? Category { get; set; }

        /// <summary>
        /// CategoryRemark
        /// </summary>
        public String CategoryRemark { get; set; }

    }
}