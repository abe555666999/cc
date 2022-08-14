using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz.NET.Web.Constant;

namespace Quartz.NET.Web.Utility
{
    public static class AppSetting
    {
        public static string TokenHeaderName = "Authorization";

        public static IConfiguration Configuration { get; private set; }

        public static string CurrentPath { get; private set; } = null;


        public static string DownLoadPath => CurrentPath + "\\Download\\";

        public static TaskInfo TaskInfo;

        public static void Init(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;

            services.Configure<TaskInfo>(configuration.GetSection("TaskInfo"));

            ServiceProvider provider = services.BuildServiceProvider();

            TaskInfo = provider.GetRequiredService<IOptions<TaskInfo>>().Value;
        }

        public static string GetSettingString(string key)
        {
            return Configuration[key];
        }

        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }
    }
}
