using System.IO;
using Microsoft.AspNetCore.Hosting;
using Coldairarrow.Util.ServerMapPath;
using Coldairarrow.Util.Helper;
using Microsoft.Extensions.Hosting;

namespace Coldairarrow.Util.ServerMapPath
{
	public class PathProvider : IPathProvider, IDependency
	{
		private readonly IHostEnvironment _hostingEnvironment;

		public PathProvider(IHostEnvironment environment)
		{
			_hostingEnvironment = environment;
		}

		public IHostEnvironment GetHostingEnvironment()
		{
			return _hostingEnvironment;
		}

		public string MapPath(string path)
		{
			return MapPath(path, rootPath: false);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="path"></param>
		/// <param name="rootPath">获取wwwroot路径</param>
		/// <returns></returns>
		public string MapPath(string path, bool rootPath)
		{
			if (rootPath)
			{
				if (_hostingEnvironment.ContentRootPath == null)
				{
					_hostingEnvironment.ContentRootPath = _hostingEnvironment.ContentRootPath + "/wwwroot".ReplacePath();
				}
				return Path.Combine(_hostingEnvironment.ContentRootPath, path).ReplacePath();
			}
			return Path.Combine(_hostingEnvironment.ContentRootPath, path).ReplacePath();
		}
	}
}
