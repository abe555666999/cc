using Coldairarrow.Util.Const;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Coldairarrow.Util.Configuration
{
    public static class AppSetting
    {
        public static string TokenHeaderName = "Authorization";

        public static IConfiguration Configuration { get; private set; }

        private static Connection _connection;

        public static CreateMember CreateMember { get; private set; }

        public static ModifyMember ModifyMember { get; private set; }

        public static string DbConnectionString => _connection.ConnectionString;
        public static string CurrentPath { get; private set; } = null;

        public static int TimeZoneHour
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetSettingString("TimeZoneHour").IsNullOrEmpty() ? 0 : GetSettingString("TimeZoneHour"));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }  //

        public static int DifTimeHour
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetSettingString("DifTimeHour").IsNullOrEmpty() ? 0 : GetSettingString("DifTimeHour"));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static int MondayDelayHour
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetSettingString("MondayDelayHour").IsNullOrEmpty() ? 0 : GetSettingString("MondayDelayHour"));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static int OtherDelayHour
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetSettingString("OtherDelayHour").IsNullOrEmpty() ? 0 : GetSettingString("OtherDelayHour"));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static string DownLoadPath => CurrentPath + "\\Download\\";

        public static SecretInfo SecretInfo;

        public static TaskInfo TaskInfo;

        public static void Init(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;

            services.Configure<Connection>(configuration.GetSection("Database:BaseDb"));

            services.Configure<SecretInfo>(configuration.GetSection("SecretInfo"));

            services.Configure<TaskInfo>(configuration.GetSection("TaskInfo"));

            ServiceProvider provider = services.BuildServiceProvider();

            _connection = provider.GetRequiredService<IOptions<Connection>>().Value;

            SecretInfo = provider.GetRequiredService<IOptions<SecretInfo>>().Value;

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

    //public class Connection
    //{
    //    public string DBType { get; set; }
    //    public string DbConnectionString { get; set; }
    //    public string RedisConnectionString { get; set; }
    //    public bool UseRedis { get; set; }
    //    public bool UseSignalR { get; set; }
    //}

    public class CreateMember : TableDefaultColumns
    {
    }
    public class ModifyMember : TableDefaultColumns
    {
    }

    public abstract class TableDefaultColumns
    {
        public string UserIdField { get; set; }
        public string UserNameField { get; set; }
        public string DateField { get; set; }
    }

}

