using System;
using System.Collections.Generic;

namespace Coldairarrow.Entity.DTO.Lottery
{
    public class YCOpenResultInfo
    {
        public int total { get; set; }
        public string pageSize { get; set; }
        public int totalPage { get; set; }
        public int echo { get; set; }
        public int totalRecords { get; set; }

        public List<YUResultInfo> data { get; set; }
    }

    public class YUResultInfo
    {
        public long section { get; set; }
        public bool isOpen { get; set; }
        public long openTime { get; set; }
        public string middleCode { get; set; }
        public int openNum { get; set; }
        public string openTime_s { get; set; }
        public int yq { get; set; }
        public int eq { get; set; }
        public int sq { get; set; }
    }
}
