using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business
{
	public static class ServiceProviderManagerExtension
	{
		public static object GetService(this Type serviceType)
		{
			return HttpContext.Current.RequestServices.GetService(serviceType);
		}
	}

}
