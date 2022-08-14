using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Coldairarrow.Business.Dapper;
using Coldairarrow.Business.EFDbContext;
using Coldairarrow.Business;
using Coldairarrow.Util.Configuration;

namespace Coldairarrow.Business.Dapper
{
	public class DBServerProvider
	{
		private static Dictionary<string, string> ConnectionPool;

		private static readonly string DefaultConnName;

		public static LotDBContext DbContext => GetEFDbContext();

		public static ISqlDapper SqlDapper => new SqlDapper(DefaultConnName);

		static DBServerProvider()
		{
			ConnectionPool = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			DefaultConnName = "defalut";
			SetConnection(DefaultConnName, AppSetting.DbConnectionString);
		}

		public static void SetConnection(string key, string val)
		{
			if (ConnectionPool.ContainsKey(key))
			{
				ConnectionPool[key] = val;
			}
			else
			{
				ConnectionPool.Add(key, val);
			}
		}

		/// <summary>
		/// 设置默认数据库连接
		/// </summary>
		/// <param name="val"></param>
		public static void SetDefaultConnection(string val)
		{
			SetConnection(DefaultConnName, val);
		}

		public static string GetConnectionString(string key)
		{
			if (key == null)
			{
				key = DefaultConnName;
			}
			if (ConnectionPool.ContainsKey(key))
			{
				return ConnectionPool[key];
			}
			return key;
		}

		/// <summary>
		/// 获取默认数据库连接
		/// </summary>
		/// <returns></returns>
		public static string GetConnectionString()
		{
			return GetConnectionString(DefaultConnName);
		}

		public static IDbConnection GetDbConnection(string connString = null)
		{
			if (connString == null)
			{
				connString = ConnectionPool[DefaultConnName];
			}
			return new SqlConnection(connString);
		}

		public static LotDBContext GetEFDbContext()
		{
			return GetEFDbContext(null);
		}

		public static LotDBContext GetEFDbContext(string dbName)
		{
			LotDBContext beefContext = HttpContext.Current.RequestServices.GetService(typeof(LotDBContext)) as LotDBContext;
			if (dbName != null)
			{
				if (!ConnectionPool.ContainsKey(dbName))
				{
					throw new Exception("数据库连接名称错误");
				}
				beefContext.Database.GetDbConnection().ConnectionString = ConnectionPool[dbName];
			}
			return beefContext;
		}

		public static void SetDbContextConnection(LotDBContext beefContext, string dbName)
		{
			if (!ConnectionPool.ContainsKey(dbName))
			{
				throw new Exception("数据库连接名称错误");
			}
			beefContext.Database.GetDbConnection().ConnectionString = ConnectionPool[dbName];
		}

		/// <summary>
		/// 获取实体的数据库连接
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="defaultDbContext"></param>
		/// <returns></returns>
		public static void GetDbContextConnection<TEntity>(LotDBContext defaultDbContext)
		{
		}

		public static ISqlDapper GetSqlDapper(string dbName = null)
		{
			return new SqlDapper(dbName ?? DefaultConnName);
		}

		public static ISqlDapper GetSqlDapper<TEntity>()
		{
			string dbName = typeof(TEntity).GetTypeCustomValue((DBConnectionAttribute x) => x.DBName) ?? DefaultConnName;
			return GetSqlDapper(dbName);
		}
	}

}
