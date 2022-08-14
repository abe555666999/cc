using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util.Configuration;
using Coldairarrow.Util.ServerMapPath;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Coldairarrow.Business.EFDbContext
{
	public class LotDBContext : DbContext, IDependency
	{
		/// <summary>
		/// 数据库连接名称 
		/// </summary>
		public string DataBaseName = null;

		/// <summary>
		/// 设置跟踪状态
		/// </summary>
		public bool QueryTracking
		{
			set
			{
				ChangeTracker.QueryTrackingBehavior = ((!value) ? QueryTrackingBehavior.NoTracking : QueryTrackingBehavior.TrackAll);
			}
		}

		public LotDBContext()
		{
		}

		public LotDBContext(string connction)
		{
			DataBaseName = connction;
		}

		public LotDBContext(DbContextOptions<LotDBContext> options)
			: base(options)
		{
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override int SaveChanges()
		{
			try
			{
				return base.SaveChanges();
			}
			catch (Exception ex)
			{
				throw ex.InnerException ?? ex;
			}
		}

		public override DbSet<TEntity> Set<TEntity>()
		{
			return base.Set<TEntity>();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string connectionString = AppSetting.DbConnectionString;
			optionsBuilder.UseSqlServer(connectionString);
			optionsBuilder = optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			try
			{
				IEnumerable<CompilationLibrary> compilationLibrary = DependencyContext.Default.CompileLibraries.Where((CompilationLibrary x) => !x.Serviceable && x.Type != "package" && x.Type == "project");
				foreach (CompilationLibrary _compilation in compilationLibrary)
				{
					(from x in AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(_compilation.Name)).GetTypes()
					 where x.GetTypeInfo().BaseType != null && x.BaseType == typeof(BaseEntity)
					 select x).ToList().ForEach(delegate (Type t)
					 {
						 modelBuilder.Entity(t);
					 });
				}
				base.OnModelCreating(modelBuilder);
			}
			catch (Exception)
			{
			}
		}
	}

}
