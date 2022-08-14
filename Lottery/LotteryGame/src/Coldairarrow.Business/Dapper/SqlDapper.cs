using Coldairarrow.Business.Utilities;
using Coldairarrow.Util;
using Coldairarrow.Util.Primitives;
using Dapper;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Dapper
{
	public class SqlDapper : ISqlDapper
	{
		private string _connectionString;

		private IDbTransaction dbTransaction = null;

		private IDbConnection _connection { get; set; }

		public IDbConnection Connection
		{
			get
			{
				if (_connection == null || _connection.State == ConnectionState.Closed)
				{
					_connection = DBServerProvider.GetDbConnection(_connectionString);
				}
				return _connection;
			}
		}

		private bool _transaction { get; set; }

		public SqlDapper()
		{
			_connectionString = DBServerProvider.GetConnectionString();
		}

		/// <summary>
		///      string mySql = "Data Source=132.232.2.109;Database=mysql;User 
		///      ID=root;Password=mysql;pooling=true;CharSet=utf8;port=3306;sslmode=none";
		///  this.conn = new MySql.Data.MySqlClient.MySqlConnection(mySql);
		/// </summary>
		/// <param name="connKeyName"></param>
		public SqlDapper(string connKeyName)
		{
			_connectionString = DBServerProvider.GetConnectionString(connKeyName);
		}

		/// <summary>
		/// Dapper事务处理
		/// </summary>
		/// <param name="action"></param>
		/// <param name="error"></param>
		public void BeginTransaction(Func<ISqlDapper, bool> action, Action<Exception> error)
		{
			_transaction = true;
			try
			{
				Connection.Open();
				dbTransaction = Connection.BeginTransaction();
				if (action(this))
				{
					dbTransaction?.Commit();
				}
				else
				{
					dbTransaction?.Rollback();
				}
			}
			catch (Exception ex)
			{
				dbTransaction?.Rollback();
				error(ex);
			}
			finally
			{
				Connection?.Dispose();
				dbTransaction?.Dispose();
				_transaction = false;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="param"></param>
		/// <param name="commandType"></param>
		/// <param name="beginTransaction"></param>
		/// <returns></returns>
		public List<T> QueryList<T>(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false) where T : class
		{
			return Execute((IDbConnection conn, IDbTransaction dbTransaction) => conn.Query<T>(cmd, param, dbTransaction, buffered: true, null, commandType ?? CommandType.Text).ToList(), beginTransaction);
		}

		public T QueryFirst<T>(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false) where T : class
		{
			List<T> list = QueryList<T>(cmd, param, commandType ?? CommandType.Text, beginTransaction).ToList();
			return (list.Count == 0) ? null : list[0];
		}

		public object ExecuteScalar(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false)
		{
			return Execute((IDbConnection conn, IDbTransaction dbTransaction) => conn.ExecuteScalar(cmd, param, dbTransaction, null, commandType ?? CommandType.Text), beginTransaction);
		}

		public int ExcuteNonQuery(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false)
		{
			return Execute((IDbConnection conn, IDbTransaction dbTransaction) => conn.Execute(cmd, param, dbTransaction, null, commandType ?? CommandType.Text), beginTransaction);
		}

		public IDataReader ExecuteReader(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false)
		{
			return Execute((IDbConnection conn, IDbTransaction dbTransaction) => conn.ExecuteReader(cmd, param, dbTransaction, null, commandType ?? CommandType.Text), beginTransaction, disposeConn: false);
		}

		public SqlMapper.GridReader QueryMultiple(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false)
		{
			return Execute((IDbConnection conn, IDbTransaction dbTransaction) => conn.QueryMultiple(cmd, param, dbTransaction, null, commandType ?? CommandType.Text), beginTransaction, disposeConn: false);
		}

		/// <summary>
		/// 获取output值
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="param"></param>
		/// <param name="commandType"></param>
		/// <param name="beginTransaction"></param>
		/// <returns></returns>
		public (List<T1>, List<T2>) QueryMultiple<T1, T2>(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false)
		{
			using SqlMapper.GridReader reader = QueryMultiple(cmd, param, commandType, beginTransaction);
			return (reader.Read<T1>().ToList(), reader.Read<T2>().ToList());
		}

		public (List<T1>, List<T2>, List<T3>) QueryMultiple<T1, T2, T3>(string cmd, object param, CommandType? commandType = null, bool beginTransaction = false)
		{
			using SqlMapper.GridReader reader = QueryMultiple(cmd, param, commandType, beginTransaction);
			return (reader.Read<T1>().ToList(), reader.Read<T2>().ToList(), reader.Read<T3>().ToList());
		}

		/// <summary>
		/// 获取分页数据(包括总数量)
		/// </summary>
		/// <typeparam name="T">泛型</typeparam>
		/// <param name="pageInput">分页参数</param>
		/// <returns></returns>
		public PageResult<T> GetPageList<T>(PageSelInput pageInput)
		{
			if (pageInput.where.IsNullOrEmpty())
			{
				pageInput.where = " 1=1";
			}
			int count;
			IEnumerable<T> list = GetPageResult<T>(pageInput.files, pageInput.tableName, pageInput.where, pageInput.orderby, pageInput.pageIndex, pageInput.pageSize, out count, CommandType.Text, pageInput.beginTransaction);
			return new PageResult<T>
			{
				Data = list.ToList(),
				Total = count
			};
		}

		/// <summary>
		/// dapper通用分页方法
		/// </summary>
		/// <typeparam name="T">泛型集合实体类</typeparam>
		/// <param name="files">列</param>
		/// <param name="tableName">表</param>
		/// <param name="where">条件</param>
		/// <param name="orderby">排序</param>
		/// <param name="pageIndex">当前页</param>
		/// <param name="pageSize">当前页显示条数</param>
		/// <param name="total">结果集总数</param>
		/// <param name="commandType"></param>
		/// <param name="beginTransaction"></param>
		/// <returns></returns>
		private IEnumerable<T> GetPageResult<T>(string files, string tableName, string where, string orderby, int pageIndex, int pageSize, out int total, CommandType? commandType = null, bool beginTransaction = false)
		{
			int skip = 1;
			if (pageIndex > 0)
			{
				skip = (pageIndex - 1) * pageSize + 1;
			}
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("SELECT COUNT(1) FROM {0} where {1};", tableName, where);
			sb.AppendFormat("SELECT  {0}\r\n\r\n                                FROM(SELECT ROW_NUMBER() OVER(ORDER BY {3}) AS RowNum,{0}\r\n\r\n                                          FROM  {1}\r\n\r\n                                          WHERE {2}\r\n\r\n                                        ) AS result\r\n\r\n                                WHERE  RowNum >= {4}   AND RowNum <= {5}\r\n\r\n                                ORDER BY {3}", files, tableName, where, orderby, skip, pageIndex * pageSize);
			using SqlMapper.GridReader reader = QueryMultiple(sb.ToString(), null, commandType, beginTransaction);
			total = reader.ReadFirst<int>();
			return reader.Read<T>();
		}

		private T Execute<T>(Func<IDbConnection, IDbTransaction, T> func, bool beginTransaction = false, bool disposeConn = true)
		{
			if (beginTransaction && !_transaction)
			{
				Connection.Open();
				dbTransaction = Connection.BeginTransaction();
			}
			try
			{
				T reslutT = func(Connection, dbTransaction);
				if (!_transaction && dbTransaction != null)
				{
					dbTransaction.Commit();
				}
				return reslutT;
			}
			catch (Exception)
			{
				if (!_transaction && dbTransaction != null)
				{
					dbTransaction.Rollback();
				}
				throw;
			}
			finally
			{
				if (!_transaction)
				{
					if (disposeConn)
					{
						Connection.Dispose();
					}
					dbTransaction?.Dispose();
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="addFileds">指定插入的字段</param>
		/// <param name="beginTransaction">是否开启事务</param>
		/// <returns></returns>
		public int Add<T>(T entity, Expression<Func<T, object>> addFileds = null, bool beginTransaction = false)
		{
			return AddRange(new T[1] { entity }, addFileds, beginTransaction);
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entities"></param>
		/// <param name="addFileds">指定插入的字段</param>
		/// <param name="beginTransaction">是否开启事务</param>
		/// <returns></returns>
		public int AddRange<T>(IEnumerable<T> entities, Expression<Func<T, object>> addFileds = null, bool beginTransaction = true)
		{
			Type entityType = typeof(T);
			PropertyInfo key = entityType.GetKeyProperty();
			if (key == null)
			{
				throw new Exception("实体必须包括主键才能批量更新");
			}
			string[] columns;
			if (addFileds != null)
			{
				columns = addFileds.GetExpressionToArray();
			}
			else
			{
				IEnumerable<PropertyInfo> properties = entityType.GetGenericProperties();
				if (key.PropertyType != typeof(Guid))
				{
					properties = properties.Where((PropertyInfo x) => x.Name != key.Name).ToArray();
				}
				columns = properties.Select((PropertyInfo x) => x.Name).ToArray();
			}
			string sql = null;
			sql = "insert into " + entityType.GetEntityTableName() + "(" + string.Join(",", columns) + ")" + $"select *  from  {EntityToSqlTempName.TempInsert};";
			sql = entities.GetEntitySql(key.PropertyType == typeof(Guid), sql, null, addFileds);
			return Execute((IDbConnection conn, IDbTransaction dbTransaction) => conn.Execute(sql, null, dbTransaction), beginTransaction);
		}

		/// <summary>
		/// sqlserver使用的临时表参数化批量更新，mysql批量更新待发开
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">实体必须带主键</param>
		/// <param name="updateFileds">指定更新的字段x=new {x.a,x.b}</param>
		/// <param name="beginTransaction">是否开启事务</param>
		/// <returns></returns>
		public int Update<T>(T entity, Expression<Func<T, object>> updateFileds = null, bool beginTransaction = true)
		{
			return UpdateRange(new T[1] { entity }, updateFileds, beginTransaction);
		}

		/// <summary>
		///             (根据主键批量更新实体) sqlserver使用的临时表参数化批量更新，mysql待优化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entities">实体必须带主键</param>
		/// <param name="updateFileds">批定更新字段</param>
		/// <param name="beginTransaction"></param>
		/// <returns></returns>
		public int UpdateRange<T>(IEnumerable<T> entities, Expression<Func<T, object>> updateFileds = null, bool beginTransaction = true)
		{
			Type entityType = typeof(T);
			PropertyInfo key = entityType.GetKeyProperty();
			if (key == null)
			{
				throw new Exception("实体必须包括主键才能批量更新");
			}
			IEnumerable<PropertyInfo> properties = from x in entityType.GetGenericProperties()
												   where x.Name != key.Name
												   select x;
			if (updateFileds != null)
			{
				properties = properties.Where((PropertyInfo x) => updateFileds.GetExpressionToArray().Contains(x.Name));
			}
			string fileds = string.Join(",", properties.Select((PropertyInfo x) => " a." + x.Name + "=b." + x.Name).ToArray());
			string sql = "update  a  set " + fileds + " from  " + entityType.GetEntityTableName() + " as a inner join " + EntityToSqlTempName.TempInsert.ToString() + " as b on a." + key.Name + "=b." + key.Name;
			sql = entities.ToList().GetEntitySql(containsKey: true, sql, null, updateFileds);
			return ExcuteNonQuery(sql, null, CommandType.Text, beginTransaction: true);
		}

		public int DelWithKey<T>(bool beginTransaction = false, params object[] keys)
		{
			Type entityType = typeof(T);
			PropertyInfo keyProperty = entityType.GetKeyProperty();
			if (keyProperty == null || keys == null || keys.Length == 0)
			{
				return 0;
			}
			IEnumerable<(bool, string, object)> validation = keyProperty.ValidationValueForDbType(keys);
			if (validation.Any(((bool, string, object) x) => !x.Item1))
			{
				throw new Exception($"主键类型【{(from x in validation
											 where !x.Item1
											 select x into s
											 select s.Item3).FirstOrDefault()}】不正确");
			}
			string tKey = entityType.GetKeyProperty().Name;
			FieldType fieldType = entityType.GetFieldType();
			string joinKeys = ((fieldType == FieldType.Int || fieldType == FieldType.BigInt) ? string.Join(",", keys) : ("'" + string.Join("','", keys) + "'"));
			string sql = "DELETE FROM " + entityType.GetEntityTableName() + " where " + tKey + " in (" + joinKeys + ");";
			return ExcuteNonQuery(sql, null);
		}

		/// <summary>
		/// 使用key批量删除
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="keys"></param>
		/// <returns></returns>
		public int DelWithKey<T>(params object[] keys)
		{
			return DelWithKey<T>(beginTransaction: false, keys);
		}

		/// <summary>
		/// 通过Bulk批量插入
		/// </summary>
		/// <param name="table"></param>
		/// <param name="tableName"></param>
		/// <param name="sqlBulkCopyOptions"></param>
		/// <param name="dbKeyName"></param>
		/// <returns></returns>
		private int MSSqlBulkInsert(DataTable table, string tableName, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.UseInternalTransaction, string dbKeyName = null)
		{
			if (!string.IsNullOrEmpty(dbKeyName))
			{
				Connection.ConnectionString = DBServerProvider.GetConnectionString(dbKeyName);
			}
			using SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(Connection.ConnectionString, sqlBulkCopyOptions)
			{
				DestinationTableName = tableName,
				BatchSize = table.Rows.Count
			};
			for (int i = 0; i < table.Columns.Count; i++)
			{
				sqlBulkCopy.ColumnMappings.Add(table.Columns[i].ColumnName, table.Columns[i].ColumnName);
			}
			sqlBulkCopy.WriteToServer(table);
			return table.Rows.Count;
		}

		public int BulkInsert<T>(List<T> entities, string tableName = null, Expression<Func<T, object>> columns = null, SqlBulkCopyOptions? sqlBulkCopyOptions = null)
		{
			DataTable table = entities.ToDataTable(columns, contianKey: false);
			return BulkInsert(table, tableName ?? typeof(T).GetEntityTableName(), sqlBulkCopyOptions);
		}

		public int BulkInsert(DataTable table, string tableName, SqlBulkCopyOptions? sqlBulkCopyOptions = null, string fileName = null, string tmpPath = null)
		{
			if (!string.IsNullOrEmpty(tmpPath))
			{
				tmpPath = tmpPath.ReplacePath();
			}
			return MSSqlBulkInsert(table, tableName, sqlBulkCopyOptions ?? SqlBulkCopyOptions.KeepIdentity);
		}

		/// <summary>
		///             大批量数据插入,返回成功插入行数
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="table">数据表</param>
		/// <param name="fileName"></param>
		/// <param name="tmpPath"></param>
		/// <returns>返回成功插入行数</returns>
		private int MySqlBulkInsert(DataTable table, string tableName, string fileName = null, string tmpPath = null)
		{
			if (table.Rows.Count == 0)
			{
				return 0;
			}
			if (tmpPath == null)
			{
				tmpPath = FileHelper.GetCurrentDownLoadPath();
			}
			if (fileName == null)
			{
				fileName = $"{DateTime.Now:yyyyMMddHHmmss}.csv";
			}
			int insertCount = 0;
			string csv = DataTableToCsv(table);
			FileHelper.WriteFile(tmpPath, fileName, csv);
			string path = tmpPath + fileName;
			try
			{
				if (Connection.State == ConnectionState.Closed)
				{
					Connection.Open();
				}
				using IDbTransaction tran = Connection.BeginTransaction();
				MySqlBulkLoader bulk = new MySqlBulkLoader(Connection as MySqlConnection)
				{
					FieldTerminator = ",",
					FieldQuotationCharacter = '"',
					EscapeCharacter = '"',
					LineTerminator = "\r\n",
					FileName = path.ReplacePath(),
					NumberOfLinesToSkip = 0,
					TableName = tableName,
					CharacterSet = "UTF8"
				};
				bulk.Columns.AddRange((from DataColumn colum in table.Columns
									   select colum.ColumnName).ToList());
				insertCount = bulk.Load();
				tran.Commit();
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				Connection?.Dispose();
				Connection?.Close();
			}
			return insertCount;
		}

		/// <summary>
		///             将DataTable转换为标准的CSV
		/// </summary>
		/// <param name="table">数据表</param>
		/// <returns>返回标准的CSV</returns>
		private static string DataTableToCsv(DataTable table)
		{
			StringBuilder sb = new StringBuilder();
			Type typeString = typeof(string);
			Type typeDate = typeof(DateTime);
			foreach (DataRow row in table.Rows)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					DataColumn colum = table.Columns[i];
					if (i != 0)
					{
						sb.Append(',');
					}
					if (colum.DataType == typeString && row[colum].ToString()!.Contains(","))
					{
						sb.Append("\"" + row[colum].ToString()!.Replace("\"", "\"\"") + "\"");
					}
					else if (colum.DataType == typeDate)
					{
						DateTime dt;
						bool b = DateTime.TryParse(row[colum].ToString(), out dt);
						sb.Append(b ? dt.ToString("yyyy-MM-dd HH:mm:ss") : "");
					}
					else
					{
						sb.Append(row[colum].ToString());
					}
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		/// <summary>
		/// 批量写入
		/// </summary>
		/// <param name="table"></param>
		/// <param name="tableName"></param>
		private void PGSqlBulkInsert(DataTable table, string tableName)
		{
			List<string> columns = new List<string>();
			for (int j = 0; j < table.Columns.Count; j++)
			{
				columns.Add("\"" + table.Columns[j].ColumnName + "\"");
			}
			string copySql = "copy \"public\".\"" + tableName + "\"(" + string.Join(',', columns) + ") FROM STDIN (FORMAT BINARY)";
			using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
			conn.Open();
			using NpgsqlBinaryImporter writer = conn.BeginBinaryImport(copySql);
			foreach (DataRow row in table.Rows)
			{
				writer.StartRow();
				for (int i = 0; i < table.Columns.Count; i++)
				{
					writer.Write(row[i]);
				}
			}
			writer.Complete();
		}

		/// <summary>
		/// 将List转换为DataTable
		/// </summary>
		/// <param name="list">请求数据</param>
		/// <returns></returns>
		public static DataTable ListToDataTable<T>(List<T> list)
		{
			DataTable dt = new DataTable("tableName");
			PropertyInfo[] properties = list.FirstOrDefault()!.GetType().GetProperties();
			foreach (PropertyInfo item in properties)
			{
				dt.Columns.Add(item.Name);
			}
			foreach (T item2 in list)
			{
				DataRow value = dt.NewRow();
				foreach (DataColumn dtColumn in dt.Columns)
				{
					int i = dt.Columns.IndexOf(dtColumn);
					if (value.GetType().IsPrimitive)
					{
						value[i] = item2.GetType().GetProperty(dtColumn.ColumnName)!.GetValue(item2);
					}
					else
					{
						value[i] = JsonConvert.SerializeObject(item2.GetType().GetProperty(dtColumn.ColumnName)!.GetValue(item2));
					}
				}
				dt.Rows.Add(value);
			}
			return dt;
		}
	}
}
