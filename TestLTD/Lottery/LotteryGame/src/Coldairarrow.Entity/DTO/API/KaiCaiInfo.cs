using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Entity.DTO.API
{
    public class KaiCaiInfo
    {
        public int code { get; set; }

        public string msg { get; set; }
        public List<KCInfo> data { get; set; }
    }

    public class KCInfo
    {
        public string issue { get; set; }
        public string drawResult { get; set; }
        public string drawTime { get; set; }
        public string code { get; set; }
    }

}
