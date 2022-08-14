using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business.EFDbContext
{
	public class DBConnectionAttribute : Attribute
	{
		public string DBName { get; set; }
	}
}
