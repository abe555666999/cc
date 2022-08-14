using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Coldairarrow.Business;

namespace Coldairarrow.Business
{
	public static class StaticHttpContextExtensions
	{
		public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
		{
			IHttpContextAccessor httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
			Coldairarrow.Business.HttpContext.Configure(httpContextAccessor);
			return app;
		}
	}
}
