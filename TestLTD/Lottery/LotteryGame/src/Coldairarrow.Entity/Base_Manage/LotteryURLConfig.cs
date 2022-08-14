using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// LotteryURLConfig
    /// </summary>
    [Table("LotteryURLConfig")]
    public class LotteryURLConfig
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
        /// LotName
        /// </summary>
        public String LotName { get; set; }

        /// <summary>
        /// SourceName
        /// </summary>
        public String SourceName { get; set; }

        /// <summary>
        /// LotType
        /// </summary>
        public Int32? LotType { get; set; }

        /// <summary>
        /// GrabURL
        /// </summary>
        public String GrabURL { get; set; }

        /// <summary>
        /// SiteName
        /// </summary>
        public String SiteName { get; set; }

        /// <summary>
        /// IsEnable
        /// </summary>
        public Boolean IsEnable { get; set; }

        /// <summary>
        /// CreatTime
        /// </summary>
        public DateTime? CreatTime { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public String Remark { get; set; }

    }
}