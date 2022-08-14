using Microsoft.AspNetCore.Hosting;
using Coldairarrow.Util.ServerMapPath;
using Microsoft.Extensions.Hosting;

namespace Coldairarrow.Util.ServerMapPath
{
	public interface IPathProvider : IDependency
	{
		string MapPath(string path);

		string MapPath(string path, bool rootPath);

		IHostEnvironment GetHostingEnvironment();

		
	}
}
