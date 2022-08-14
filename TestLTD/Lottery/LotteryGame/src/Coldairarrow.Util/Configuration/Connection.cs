using EFCore.Sharding;

namespace Coldairarrow.Util.Configuration
{
	public class Connection
	{
		public string ConnectionString { get; set; }
		public DatabaseType DatabaseType { get; set; }
	}
}
