using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Util.Primitives
{
	public class PageSelInput
	{
		public string files { get; set; }

		public string tableName { get; set; }

		public string where { get; set; }

		public string orderby { get; set; }

		public int pageIndex { get; set; }

		public int pageSize { get; set; }

		public int total { get; set; }

		public CommandType commandType { get; set; }

		public bool beginTransaction { get; set; }
	}
}
