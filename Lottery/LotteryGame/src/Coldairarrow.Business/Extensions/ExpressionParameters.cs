using Coldairarrow.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business
{
	public class ExpressionParameters
	{
		public string Field { get; set; }

		public LinqExpressionType ExpressionType { get; set; }

		public object Value { get; set; }
	}
}
