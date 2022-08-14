﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business
{
	public class AutofacContainerModule
	{
		public static TService GetService<TService>() where TService : class
		{
			return typeof(TService).GetService() as TService;
		}
	}
}
