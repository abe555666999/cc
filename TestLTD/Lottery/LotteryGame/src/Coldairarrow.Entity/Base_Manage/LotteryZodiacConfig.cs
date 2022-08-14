using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{
    /// <summary>
    /// LotteryZodiacConfig
    /// </summary>
    [Table("LotteryZodiacConfig")]
    public class LotteryZodiacConfig
    {

        /// <summary>
        /// Id
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// OpenNo
        /// </summary>
        public String OpenNo { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// IsEnable
        /// </summary>
        public Boolean IsEnable { get; set; }

    }
}