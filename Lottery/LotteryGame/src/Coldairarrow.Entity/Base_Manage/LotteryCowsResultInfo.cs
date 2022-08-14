using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// LotteryCowsResultInfo
    /// </summary>
    [Table("LotteryCowsResultInfo")]
    public class LotteryCowsResultInfo
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
        public long LotNumber { get; set; }

        /// <summary>
        /// LotResultOneNo
        /// </summary>
        public String LotResultOneNo { get; set; }

        /// <summary>
        /// LotResultTwoNo
        /// </summary>
        public String LotResultTwoNo { get; set; }

        /// <summary>
        /// LotResultThreeNo
        /// </summary>
        public String LotResultThreeNo { get; set; }

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