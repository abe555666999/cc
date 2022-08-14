using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Entity.DTO.Lottery
{
    public class LotZodiacConfig
    {
        public LotZodiacConfig()
        {
            List = new List<int>();
        }

        public string Id { get; set; }

        public string ZodiacName { get; set; }

        public List<int> List { get; set; }
    }
}
