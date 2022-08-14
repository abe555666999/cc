using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Coldairarrow.Business;
using Coldairarrow.Entity.AttributeManager;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;

namespace Coldairarrow.Business
{
	public static class EntityProperties
	{
		private static readonly Dictionary<Type, string> entityMapDbColumnType = new Dictionary<Type, string>
	{
		{
			typeof(int),
			"int"
		},
		{
			typeof(int?),
			"int"
		},
		{
			typeof(long),
			"bigint"
		},
		{
			typeof(long?),
			"bigint"
		},
		{
			typeof(decimal),
			"decimal(18, 5)"
		},
		{
			typeof(decimal?),
			"decimal(18, 5)"
		},
		{
			typeof(double),
			"decimal(18, 5)"
		},
		{
			typeof(double?),
			"decimal(18, 5)"
		},
		{
			typeof(float),
			"decimal(18, 5)"
		},
		{
			typeof(float?),
			"decimal(18, 5)"
		},
		{
			typeof(Guid),
			"UniqueIdentifier"
		},
		{
			typeof(Guid?),
			"UniqueIdentifier"
		},
		{
			typeof(byte),
			"tinyint"
		},
		{
			typeof(byte?),
			"tinyint"
		},
		{
			typeof(string),
			"nvarchar"
		}
	};

		private static readonly Dictionary<Type, string> ProperWithDbType = new Dictionary<Type, string>
	{
		{
			typeof(string),
			"nvarchar"
		},
		{
			typeof(DateTime),
			"datetime"
		},
		{
			typeof(long),
			"bigint"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(byte),
			"int"
		},
		{
			typeof(Guid),
			"uniqueidentifier"
		}
	};

		private static string[] _userEditFields { get; set; }

		private static string[] UserEditFields
		{
			get
			{
				if (_userEditFields != null)
				{
					return _userEditFields;
				}
				return _userEditFields;
			}
		}

		public static string GetExpressionPropertyFirst<TEntity>(this Expression<Func<TEntity, object>> properties)
		{
			string[] arr = properties.GetExpressionProperty();
			if (arr.Length != 0)
			{
				return arr[0];
			}
			return "";
		}

		public static string[] GetExpressionProperty<TEntity>(this Expression<Func<TEntity, object>> properties)
		{
			if (properties == null)
			{
				return Array.Empty<string>();
			}
			if (properties.Body is NewExpression)
			{
				return ((NewExpression)properties.Body).Members.Select((MemberInfo x) => x.Name).ToArray();
			}
			if (properties.Body is MemberExpression)
			{
				return new string[1] { ((MemberExpression)properties.Body).Member.Name };
			}
			if (properties.Body is UnaryExpression)
			{
				return new string[1] { ((properties.Body as UnaryExpression).Operand as MemberExpression).Member.Name };
			}
			throw new Exception("未实现的表达式");
		}

		public static string ValidateHashInEntity(this Type typeinfo, Dictionary<string, object> dic)
		{
			return typeinfo.ValidateDicInEntity(dic, removeNotContains: false);
		}

		public static void RemoveNotExistColumns(this Type typeinfo)
		{
		}

		/// <summary>
		/// 获取所有字段的名称 
		/// </summary>
		/// <param name="typeinfo"></param>
		/// <returns></returns>
		public static List<string> GetAtrrNames(this Type typeinfo)
		{
			return (from c in typeinfo.GetProperties()
					select c.Name).ToList();
		}

		public static void IsExistColumns(this Type typeinfo)
		{
		}

		public static Dictionary<string, string> GetColumType(this PropertyInfo[] properties)
		{
			return properties.GetColumType(containsKey: false);
		}

		public static Dictionary<string, string> GetColumType(this PropertyInfo[] properties, bool containsKey)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (PropertyInfo property in properties)
			{
				if (containsKey || !property.IsKey())
				{
					KeyValuePair<string, string> keyVal = property.GetColumnType(lenght: true);
					dictionary.Add(keyVal.Key, keyVal.Value);
				}
			}
			return dictionary;
		}

		/// <summary>
		/// 返回属性的字段及数据库类型
		/// </summary>
		/// <param name="property"></param>
		/// <param name="lenght">是否包括后字段具体长度:nvarchar(100)</param>
		/// <returns></returns>
		public static KeyValuePair<string, string> GetColumnType(this PropertyInfo property, bool lenght = false)
		{
			object objAtrr = property.GetTypeCustomAttributes(typeof(ColumnAttribute), out var asType);
			string colType;
			if (asType)
			{
				colType = ((ColumnAttribute)objAtrr).TypeName!.ToLower();
				if (!string.IsNullOrEmpty(colType))
				{
					if (!lenght)
					{
						return new KeyValuePair<string, string>(property.Name, colType);
					}
					if (colType == "decimal" || colType == "double" || colType == "float")
					{
						objAtrr = property.GetTypeCustomAttributes(typeof(DisplayFormatAttribute), out asType);
						colType = colType + "(" + (asType ? ((DisplayFormatAttribute)objAtrr).DataFormatString : "18,5") + ")";
					}
					if (property.PropertyType.ToString() == "System.String")
					{
						colType = colType.Split("(")[0];
						objAtrr = property.GetTypeCustomAttributes(typeof(MaxLengthAttribute), out asType);
						if (asType)
						{
							int length = ((MaxLengthAttribute)objAtrr).Length;
							colType = colType + "(" + ((length < 1 || length > (colType.StartsWith("n") ? 8000 : 4000)) ? "max" : length.ToString()) + ")";
						}
						else
						{
							colType += "(max)";
						}
					}
					return new KeyValuePair<string, string>(property.Name, colType);
				}
			}
			colType = ((!entityMapDbColumnType.TryGetValue(property.PropertyType, out var value)) ? "nvarchar" : value);
			if (lenght && colType == "nvarchar")
			{
				colType = "nvarchar(max)";
			}
			return new KeyValuePair<string, string>(property.Name, colType);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="array">将数组转换成sql语句</param>
		/// <param name="fieldType">指定FieldType数据库字段类型</param>
		/// <returns></returns>
		public static string GetArraySql(this object[] array, FieldType fieldType)
		{
			if (array == null || array.Length == 0)
			{
				return string.Empty;
			}
			string columnType = string.Empty;
			List<ArrayEntity> arrrayEntityList = array.Select((object x) => new ArrayEntity
			{
				column1 = x.ToString()
			}).ToList();
			return arrrayEntityList.GetEntitySql(containsKey: false, null, null, null, fieldType);
		}

		/// <param name="array"></param>
		/// <param name="fieldType">指定生成的数组值的类型</param>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static string GetArraySql(this object[] array, FieldType fieldType, string sql)
		{
			if (array == null || array.Length == 0)
			{
				return string.Empty;
			}
			string columnType = string.Empty;
			List<ArrayEntity> arrrayEntityList = array.Select((object x) => new ArrayEntity
			{
				column1 = x.ToString()
			}).ToList();
			return arrrayEntityList.GetEntitySql(containsKey: false, sql, null, null, fieldType);
		}

		public static string GetArraySql<T>(this object[] array, string sql)
		{
			return array.GetArraySql(typeof(T).GetFieldType(), sql);
		}

		/// <summary>
		/// 根据实体获取key的类型，用于update或del操作
		/// </summary>
		/// <returns></returns>
		public static FieldType GetFieldType(this Type typeEntity)
		{
			return (from x in typeEntity.GetProperties()
					where x.Name == typeEntity.GetKeyName()
					select x).ToList()[0].GetColumnType().Value switch
			{
				"int" => FieldType.Int,
				"bigint" => FieldType.BigInt,
				"varchar" => FieldType.VarChar,
				"uniqueidentifier" => FieldType.UniqueIdentifier,
				_ => FieldType.NvarChar,
			};
		}

		public static string GetEntitySql<T>(this IEnumerable<T> entityList, bool containsKey = false, string sql = null, Expression<Func<T, object>> ignoreFileds = null, Expression<Func<T, object>> fixedColumns = null, FieldType? fieldType = null)
		{
			if (!entityList.Any())
			{
				return "";
			}
			PropertyInfo[] propertyInfo = typeof(T).GetProperties().ToArray();
			if (propertyInfo.Length == 0)
			{
				propertyInfo = entityList.ToArray()[0].GetType().GetGenericProperties().ToArray();
			}
			propertyInfo = propertyInfo.GetGenericProperties().ToArray();
			string[] arr = null;
			if (fixedColumns != null)
			{
				arr = fixedColumns.GetExpressionToArray();
				PropertyInfo keyProperty = typeof(T).GetKeyProperty();
				propertyInfo = propertyInfo.Where((PropertyInfo x) => (containsKey && x.Name == keyProperty.Name) || arr.Contains(x.Name)).ToArray();
			}
			if (ignoreFileds != null)
			{
				arr = ignoreFileds.GetExpressionToArray();
				propertyInfo = propertyInfo.Where((PropertyInfo x) => !arr.Contains(x.Name)).ToArray();
			}
			Dictionary<string, string> dictProperties = propertyInfo.GetColumType(containsKey);
			if (fieldType.HasValue)
			{
				string realType = fieldType.ToString();
				if (fieldType.Value == FieldType.VarChar || fieldType.Value == FieldType.NvarChar)
				{
					realType += "(max)";
				}
				dictProperties = new Dictionary<string, string> {
			{
				dictProperties.Select((KeyValuePair<string, string> x) => x.Key).ToList()[0],
				realType
			} };
			}
			if (dictProperties.Keys.Count * entityList.Count() > 150000)
			{
				throw new Exception("写入数据太多,请分开写入。");
			}
			string cols = string.Join(",", dictProperties.Select((KeyValuePair<string, string> c) => "[" + c.Key + "] " + c.Value));
			StringBuilder declareTable = new StringBuilder();
			string tempTablbe = "#" + EntityToSqlTempName.TempInsert;
			declareTable.Append("CREATE TABLE " + tempTablbe + " (" + cols + ")");
			declareTable.Append("\r\n");
			int parCount = dictProperties.Count * entityList.Count();
			int takeCount = 0;
			int maxParsCount = 2050;
			if (parCount > maxParsCount)
			{
				takeCount = maxParsCount / dictProperties.Count;
			}
			int count = 0;
			StringBuilder stringLeft = new StringBuilder();
			StringBuilder stringCenter = new StringBuilder();
			StringBuilder stringRight = new StringBuilder();
			int index = 0;
			foreach (T entity in entityList)
			{
				if (index == 0 || index >= 1000 || takeCount - index == 0)
				{
					if (stringLeft.Length > 0)
					{
						declareTable.AppendLine(stringLeft.Remove(stringLeft.Length - 2, 2).Append("',").ToString() + stringCenter.Remove(stringCenter.Length - 1, 1).Append("',").ToString() + stringRight.Remove(stringRight.Length - 1, 1).ToString());
						stringLeft.Clear();
						stringCenter.Clear();
						stringRight.Clear();
					}
					stringLeft.AppendLine("exec sp_executesql N'SET NOCOUNT ON;");
					stringCenter.Append("N'");
					index = 0;
					count = 0;
				}
				stringLeft.Append((index == 0) ? ("; INSERT INTO  " + tempTablbe + "  values (") : " ");
				index++;
				PropertyInfo[] array = propertyInfo;
				foreach (PropertyInfo property in array)
				{
					if (containsKey || !property.IsKey())
					{
						string par = "@v" + count;
						stringLeft.Append(par + ",");
						stringCenter.Append(par + " " + dictProperties[property.Name] + ",");
						object val = property.GetValue(entity);
						if (val == null)
						{
							stringRight.Append(par + "=NUll,");
						}
						else
						{
							stringRight.Append(par + "='" + val.ToString()!.Replace("'", "''''") + "',");
						}
						count++;
					}
				}
				stringLeft.Remove(stringLeft.Length - 1, 1);
				stringLeft.Append("),(");
			}
			if (stringLeft.Length > 0)
			{
				declareTable.AppendLine(stringLeft.Remove(stringLeft.Length - 2, 2).Append("',").ToString() + stringCenter.Remove(stringCenter.Length - 1, 1).Append("',").ToString() + stringRight.Remove(stringRight.Length - 1, 1).ToString());
				stringLeft.Clear();
				stringCenter.Clear();
				stringRight.Clear();
			}
			if (!string.IsNullOrEmpty(sql))
			{
				sql = sql.Replace(EntityToSqlTempName.TempInsert.ToString(), tempTablbe);
				declareTable.AppendLine(sql);
			}
			else
			{
				declareTable.AppendLine(" SELECT " + string.Join(",", fixedColumns?.GetExpressionToArray() ?? new string[1] { "*" }) + " FROM " + tempTablbe);
			}
			if (tempTablbe.Substring(0, 1) == "#")
			{
				declareTable.AppendLine("; drop table " + tempTablbe);
			}
			return declareTable.ToString();
		}

		/// <summary>
		///             此方法适用于数据量少,只有几列数据，不超过1W行，或几十列数据不超过1000行的情况下使用
		/// 大批量的数据考虑其他方式
		/// 將datatable生成sql語句，替換datatable作為參數傳入存儲過程
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static string GetDataTableSql(this DataTable table)
		{
			Dictionary<string, string> dictCloumn = new Dictionary<string, string>();
			for (int i = 0; i < table.Columns.Count; i++)
			{
				dictCloumn.Add(table.Columns[i].ColumnName, "  nvarchar(max)");
			}
			int parCount = dictCloumn.Count * table.Rows.Count;
			int takeCount = 0;
			int maxParsCount = 2050;
			if (parCount > maxParsCount)
			{
				takeCount = maxParsCount / dictCloumn.Count;
			}
			if (dictCloumn.Keys.Count * table.Rows.Count > 150000)
			{
				throw new Exception("写入数据太多,请分开写入。");
			}
			string cols = string.Join(",", dictCloumn.Select((KeyValuePair<string, string> c) => "[" + c.Key + "] " + c.Value));
			StringBuilder declareTable = new StringBuilder();
			string tempTablbe = "#Temp_Insert0";
			declareTable.Append("CREATE TABLE " + tempTablbe + " (" + cols + ")");
			declareTable.Append("\r\n");
			int count = 0;
			StringBuilder stringLeft = new StringBuilder();
			StringBuilder stringCenter = new StringBuilder();
			StringBuilder stringRight = new StringBuilder();
			int index = 0;
			foreach (DataRow row in table.Rows)
			{
				if (index == 0 || index >= 1000 || takeCount - index == 0)
				{
					if (stringLeft.Length > 0)
					{
						declareTable.AppendLine(stringLeft.Remove(stringLeft.Length - 2, 2).Append("',").ToString() + stringCenter.Remove(stringCenter.Length - 1, 1).Append("',").ToString() + stringRight.Remove(stringRight.Length - 1, 1).ToString());
						stringLeft.Clear();
						stringCenter.Clear();
						stringRight.Clear();
					}
					stringLeft.AppendLine("exec sp_executesql N'SET NOCOUNT ON;");
					stringCenter.Append("N'");
					index = 0;
					count = 0;
				}
				stringLeft.Append((index == 0) ? ("; INSERT INTO  " + tempTablbe + "  values (") : " ");
				index++;
				foreach (KeyValuePair<string, string> keyValue in dictCloumn)
				{
					string par = "@v" + count;
					stringLeft.Append(par + ",");
					stringCenter.Append(par + " " + keyValue.Value + ",");
					object val = row[keyValue.Key];
					if (val == null)
					{
						stringRight.Append(par + "=NUll,");
					}
					else
					{
						stringRight.Append(par + "='" + val.ToString()!.Replace("'", "''''") + "',");
					}
					count++;
				}
				stringLeft.Remove(stringLeft.Length - 1, 1);
				stringLeft.Append("),(");
			}
			if (stringLeft.Length > 0)
			{
				declareTable.AppendLine(stringLeft.Remove(stringLeft.Length - 2, 2).Append("',").ToString() + stringCenter.Remove(stringCenter.Length - 1, 1).Append("',").ToString() + stringRight.Remove(stringRight.Length - 1, 1).ToString());
				stringLeft.Clear();
				stringCenter.Clear();
				stringRight.Clear();
			}
			declareTable.AppendLine(" SELECT * FROM " + tempTablbe);
			if (tempTablbe.Substring(0, 1) == "#")
			{
				declareTable.AppendLine("; drop table " + tempTablbe);
			}
			return declareTable.ToString();
		}

		public static string GetKeyName(this Type typeinfo)
		{
			return typeinfo.GetProperties().GetKeyName();
		}

		public static string GetKeyType(this Type typeinfo)
		{
			string keyType = typeinfo.GetProperties().GetKeyName(keyType: true);
			if (keyType == "varchar")
			{
				return "varchar(max)";
			}
			if (keyType != "nvarchar")
			{
				return keyType;
			}
			return "nvarchar(max)";
		}

		public static string GetKeyName(this PropertyInfo[] properties)
		{
			return properties.GetKeyName(keyType: false);
		}

		/// <summary>
		/// 获取key列名
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="keyType">true获取key对应类型,false返回对象Key的名称</param>
		/// <returns></returns>
		public static string GetKeyName(this PropertyInfo[] properties, bool keyType)
		{
			string keyName = string.Empty;
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.IsKey())
				{
					if (!keyType)
					{
						return propertyInfo.Name;
					}
					object[] attributes = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), inherit: false);
					if (attributes.Length != 0)
					{
						return ((ColumnAttribute)attributes[0]).TypeName!.ToLower();
					}
					return new PropertyInfo[1] { propertyInfo }.GetColumType(containsKey: true)[propertyInfo.Name];
				}
			}
			return keyName;
		}

		/// <summary>
		/// 获取主键字段
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static PropertyInfo GetKeyProperty(this Type entity)
		{
			return entity.GetProperties().GetKeyProperty();
		}

		public static PropertyInfo GetKeyProperty(this PropertyInfo[] properties)
		{
			return properties.Where((PropertyInfo c) => c.IsKey()).FirstOrDefault();
		}

		public static bool IsKey(this PropertyInfo propertyInfo)
		{
			object[] keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), inherit: false);
			if (keyAttributes.Length != 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 获取实体所有可以编辑的列
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string[] GetEditField(this Type type)
		{
			Type editType = typeof(EditableAttribute);
			PropertyInfo[] propertyInfo = type.GetProperties();
			string keyName = propertyInfo.GetKeyName();
			return (from x in propertyInfo
					where x.Name != keyName && (UserEditFields.Contains(x.Name.ToLower()) || x.ContainsCustomAttributes(editType))
					select x into s
					select s.Name).ToArray();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool ContainsCustomAttributes(this PropertyInfo propertyInfo, Type type)
		{
			propertyInfo.GetTypeCustomAttributes(type, out var contains);
			return contains;
		}

		public static List<PropertyInfo> ContainsCustomAttributes(this Type obj, Type containType)
		{
			List<PropertyInfo> proList = new List<PropertyInfo>();
			PropertyInfo[] properties = obj.GetProperties();
			foreach (PropertyInfo pro in properties)
			{
				if (pro.GetTypeCustomAttributes(containType) != null)
				{
					proList.Add(pro);
				}
			}
			return proList;
		}

		/// <summary>
		/// 获取PropertyInfo指定属性
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="type"></param>
		/// <param name="asType"></param>
		/// <returns></returns>
		public static object GetTypeCustomAttributes(this PropertyInfo propertyInfo, Type type, out bool asType)
		{
			object[] attributes = propertyInfo.GetCustomAttributes(type, inherit: false);
			if (attributes.Length == 0)
			{
				asType = false;
				return Array.Empty<string>();
			}
			asType = true;
			return attributes[0];
		}

		/// <summary>
		/// 验证集合的属性
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entityList"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static WebResponseContent ValidationEntityList<T>(this List<T> entityList, Expression<Func<T, object>> expression = null)
		{
			WebResponseContent responseData = new WebResponseContent();
			foreach (T entity in entityList)
			{
				responseData = entity.ValidationEntity(expression);
				if (!responseData.Status)
				{
					return responseData;
				}
			}
			responseData.Status = true;
			return responseData;
		}

		/// <summary>
		/// 指定需要验证的字段
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="expression">对指定属性进行验证x=&gt;{x.Name,x.Size}</param>
		/// <param name="validateProperties"></param>
		/// <returns></returns>
		public static WebResponseContent ValidationEntity<T>(this T entity, Expression<Func<T, object>> expression = null, Expression<Func<T, object>> validateProperties = null)
		{
			return entity.ValidationEntity(expression?.GetExpressionProperty(), validateProperties?.GetExpressionProperty());
		}

		/// <summary>
		/// specificProperties=null并且validateProperties=null，对所有属性验证，只验证其是否合法，不验证是否为空(除属性标识指定了不能为空外)
		/// specificProperties!=null，对指定属性校验，并且都必须有值
		/// null并且validateProperties!=null，对指定属性校验，不判断是否有值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="specificProperties">验证指定的属性，并且非空判断</param>
		/// <param name="validateProperties">验证指定属性，只对字段合法性判断，不验证是否为空</param>
		/// <returns></returns>
		public static WebResponseContent ValidationEntity<T>(this T entity, string[] specificProperties, string[] validateProperties = null)
		{
			WebResponseContent responseData = new WebResponseContent();
			if (entity == null)
			{
				return responseData.Error("对象不能为null");
			}
			PropertyInfo[] propertyArray = typeof(T).GetProperties();
			if (propertyArray.Length == 0)
			{
				propertyArray = entity.GetType().GetProperties();
			}
			List<PropertyInfo> compareProper = new List<PropertyInfo>();
			if (specificProperties != null && specificProperties.Length != 0)
			{
				compareProper.AddRange(propertyArray.Where((PropertyInfo x) => specificProperties.Contains(x.Name)));
			}
			if (validateProperties != null && validateProperties.Length != 0)
			{
				compareProper.AddRange(propertyArray.Where((PropertyInfo x) => validateProperties.Contains(x.Name)));
			}
			if (compareProper.Count > 0)
			{
				propertyArray = compareProper.ToArray();
			}
			PropertyInfo[] array = propertyArray;
			foreach (PropertyInfo propertyInfo in array)
			{
				object value = propertyInfo.GetValue(entity);
				if ((propertyInfo.Name == "Enable" || propertyInfo.Name == "AuditStatus") && value == null)
				{
					propertyInfo.SetValue(entity, 0);
					continue;
				}
				(bool, string, object) reslut = propertyInfo.ValidationProperty(value, (specificProperties != null && specificProperties.Contains(propertyInfo.Name)) ? true : false);
				if (!reslut.Item1)
				{
					return responseData.Error(reslut.Item2);
				}
			}
			return responseData.OK();
		}

		/// <summary>
		/// 获取数据库类型，不带长度，如varchar(100),只返回的varchar
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <returns></returns>
		public static string GetSqlDbType(this PropertyInfo propertyInfo)
		{
			string dbType = propertyInfo.GetTypeCustomValue((ColumnAttribute x) => new { x.TypeName });
			if (string.IsNullOrEmpty(dbType))
			{
				return dbType;
			}
			dbType = dbType.ToLower();
			if (dbType.Contains("nvarchar"))
			{
				dbType = "nvarchar";
			}
			else if (dbType.Contains("varchar"))
			{
				dbType = "varchar";
			}
			else if (dbType.Contains("nchar"))
			{
				dbType = "nchar";
			}
			else if (dbType.Contains("char"))
			{
				dbType = "char";
			}
			return dbType;
		}

		/// <summary>
		/// 验证数据库字段类型与值是否正确，
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static IEnumerable<(bool, string, object)> ValidationValueForDbType(this PropertyInfo propertyInfo, params object[] values)
		{
			string dbTypeName = propertyInfo.GetTypeCustomValue((ColumnAttribute c) => c.TypeName);
			foreach (object value in values)
			{
				yield return dbTypeName.ValidationVal(value, propertyInfo);
			}
		}

		public static bool ValidationRquiredValueForDbType(this PropertyInfo propertyInfo, object value, out string message)
		{
			if (value == null || value?.ToString()?.Trim() == "")
			{
				message = propertyInfo.GetDisplayName() + "不能为空";
				return false;
			}
			(bool, string, object) result = propertyInfo.GetProperWithDbType().ValidationVal(value, propertyInfo);
			message = result.Item2;
			return result.Item1;
		}

		public static string GetProperWithDbType(this PropertyInfo propertyInfo)
		{
			if (ProperWithDbType.TryGetValue(propertyInfo.PropertyType, out var value))
			{
				return value;
			}
			return "nvarchar";
		}

		/// <summary>
		/// 验证数据库字段类型与值是否正确，
		/// </summary>
		/// <param name="dbType">数据库字段类型(如varchar,nvarchar,decimal,不要带后面长度如:varchar(50))</param>
		/// <param name="value">值</param>
		/// <param name="propertyInfo">要验证的类的属性，若不为null，则会判断字符串的长度是否正确</param>
		/// <returns>(bool, string, object)bool成否校验成功,string校验失败信息,object,当前校验的值</returns>
		public static (bool, string, object) ValidationVal(this string dbType, object value, PropertyInfo propertyInfo = null)
		{
			if (string.IsNullOrEmpty(dbType))
			{
				dbType = ((propertyInfo != null) ? propertyInfo.GetProperWithDbType() : "nvarchar");
			}
			dbType = dbType.ToLower();
			string val = value?.ToString();
			string reslutMsg = string.Empty;
			if (dbType == "int" || dbType == "bigint")
			{
				if (!value.IsInt())
				{
					reslutMsg = "只能为有效整数";
				}
			}
			else
			{
				int num;
				switch (dbType)
				{
					default:
						num = ((dbType == "smalldate") ? 1 : 0);
						break;
					case "datetime":
					case "date":
					case "smalldatetime":
						num = 1;
						break;
				}
				if (num != 0)
				{
					if (!value.IsDate())
					{
						reslutMsg = "必须为日期格式";
					}
				}
				else if (dbType == "float" || dbType == "decimal" || dbType == "double")
				{
					if (!val.IsNumber())
					{
						reslutMsg = "不是有效数字";
					}
				}
				else if (dbType == "uniqueidentifier")
				{
					if (!val.IsGuid())
					{
						reslutMsg = propertyInfo.Name + "Guid不正确";
					}
				}
				else
				{
					int num2;
					if (propertyInfo != null)
					{
						switch (dbType)
						{
							default:
								num2 = ((dbType == "text") ? 1 : 0);
								break;
							case "varchar":
							case "nvarchar":
							case "nchar":
							case "char":
								num2 = 1;
								break;
						}
					}
					else
					{
						num2 = 0;
					}
					if (num2 != 0)
					{
						if (val.Length > 20000)
						{
							reslutMsg = "字符长度最多【20000】";
						}
						else
						{
							int length = propertyInfo.GetTypeCustomValue((MaxLengthAttribute x) => new { x.Length }).GetInt();
							if (length == 0)
							{
								return (true, null, null);
							}
							if (length < 8000 && ((dbType.Substring(0, 1) != "n" && Encoding.UTF8.GetBytes(val.ToCharArray()).Length > length) || val.Length > length))
							{
								reslutMsg = $"最多只能【{length}】个字符。";
							}
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(reslutMsg) && propertyInfo != null)
			{
				reslutMsg = propertyInfo.GetDisplayName() + reslutMsg;
			}
			return (reslutMsg == "", reslutMsg, value);
		}

		public static string GetDisplayName(this PropertyInfo property)
		{
			string displayName = property.GetTypeCustomValue((DisplayAttribute x) => new { x.Name });
			if (string.IsNullOrEmpty(displayName))
			{
				return property.Name;
			}
			return displayName;
		}

		/// <summary>
		/// 验证每个属性的值是否正确
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="objectVal">属性的值</param>
		/// <param name="required">是否指定当前属性必须有值</param>
		/// <returns></returns>
		public static (bool, string, object) ValidationProperty(this PropertyInfo propertyInfo, object objectVal, bool required)
		{
			if (propertyInfo.IsKey())
			{
				return (true, null, objectVal);
			}
			string val = ((objectVal == null) ? "" : objectVal.ToString()!.Trim());
			string requiredMsg = string.Empty;
			if (!required)
			{
				Dictionary<string, string> reuireVal = propertyInfo.GetTypeCustomValues((RequiredAttribute x) => new { x.AllowEmptyStrings, x.ErrorMessage });
				if (reuireVal != null && !Convert.ToBoolean(reuireVal["AllowEmptyStrings"]))
				{
					required = true;
					requiredMsg = reuireVal["ErrorMessage"];
				}
			}
			if (!required && string.IsNullOrEmpty(val))
			{
				return (true, null, objectVal);
			}
			if (required && val == string.Empty)
			{
				if (requiredMsg != "")
				{
					return (false, requiredMsg, objectVal);
				}
				string propertyName = propertyInfo.GetTypeCustomValue((DisplayAttribute x) => new { x.Name });
				return (false, requiredMsg + (string.IsNullOrEmpty(propertyName) ? propertyInfo.Name : propertyName) + "不能为空", objectVal);
			}
			return propertyInfo.GetSqlDbType()?.ValidationVal(val, propertyInfo) ?? (true, null, objectVal);
		}

		/// <summary>
		/// 获取属性的指定属性
		/// </summary>
		/// <param name="member"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetTypeCustomAttributes(this MemberInfo member, Type type)
		{
			object[] obj = member.GetCustomAttributes(type, inherit: false);
			if (obj.Length == 0)
			{
				return null;
			}
			return obj[0];
		}

		/// <summary>
		/// 获取类的指定属性
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetTypeCustomAttributes(this Type entity, Type type)
		{
			object[] obj = entity.GetCustomAttributes(type, inherit: false);
			if (obj.Length == 0)
			{
				return null;
			}
			return obj[0];
		}

		/// <summary>
		/// 获取类的多个指定属性的值
		/// </summary>
		/// <param name="member">当前类</param>
		/// <param name="expression">指定属性的值 格式  new { x.字段1, x.字段2 }</param>
		/// <returns>返回的是字段+value</returns>
		public static Dictionary<string, string> GetTypeCustomValues<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
		{
			object attr = member.GetTypeCustomAttributes(typeof(TEntity));
			if (attr == null)
			{
				return null;
			}
			string[] propertyName = expression.GetExpressionProperty();
			Dictionary<string, string> propertyKeyValues = new Dictionary<string, string>();
			PropertyInfo[] properties = attr.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (propertyName.Contains(property.Name))
				{
					propertyKeyValues[property.Name] = (property.GetValue(attr) ?? string.Empty)!.ToString();
				}
			}
			return propertyKeyValues;
		}

		/// <summary>
		/// 获取类的单个指定属性的值(只会返回第一个属性的值)
		/// </summary>
		/// <param name="member">当前类</param>
		/// <param name="expression">指定属性的值 格式  new { x.字段1, x.字段2 }</param>
		/// <returns></returns>
		public static string GetTypeCustomValue<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
		{
			Dictionary<string, string> propertyKeyValues = member.GetTypeCustomValues(expression);
			if (propertyKeyValues == null || propertyKeyValues.Count == 0)
			{
				return null;
			}
			return propertyKeyValues.First().Value ?? "";
		}

		/// <summary>
		/// 判断hash的列是否为对应的实体，并且值是否有效
		/// </summary>
		/// <param name="typeinfo"></param>
		/// <param name="dic"></param>
		/// <param name="removeNotContains">移除不存在字段</param>
		/// <param name="ignoreFields"></param>
		/// <returns></returns>
		public static string ValidateDicInEntity(this Type typeinfo, Dictionary<string, object> dic, bool removeNotContains, string[] ignoreFields = null)
		{
			return typeinfo.ValidateDicInEntity(dic, removeNotContains, removerKey: true, ignoreFields);
		}

		public static string ValidateDicInEntity(this Type type, List<Dictionary<string, object>> dicList, bool removeNotContains, bool removerKey, string[] ignoreFields = null)
		{
			PropertyInfo[] propertyInfo = type.GetProperties();
			string reslutMsg = string.Empty;
			foreach (Dictionary<string, object> dic in dicList)
			{
				reslutMsg = type.ValidateDicInEntity(dic, propertyInfo, removeNotContains, removerKey, ignoreFields);
				if (!string.IsNullOrEmpty(reslutMsg))
				{
					return reslutMsg;
				}
			}
			return reslutMsg;
		}

		public static string ValidateDicInEntity(this Type type, Dictionary<string, object> dic, bool removeNotContains, bool removerKey, string[] ignoreFields = null)
		{
			return type.ValidateDicInEntity(dic, null, removeNotContains, removerKey, ignoreFields);
		}

		/// <summary>
		/// 判断hash的列是否为对应的实体，并且值是否有效
		/// </summary>
		/// <param name="typeinfo"></param>
		/// <param name="dic"></param>
		/// <param name="removeNotContains">移除不存在字段</param>
		/// <param name="removerKey">移除主键</param>
		/// <param name="ignoreFields"></param>
		/// <param name="propertyInfo"></param>
		/// <returns></returns>
		private static string ValidateDicInEntity(this Type typeinfo, Dictionary<string, object> dic, PropertyInfo[] propertyInfo, bool removeNotContains, bool removerKey, string[] ignoreFields = null)
		{
			if (dic == null || dic.Count == 0)
			{
				return "参数无效";
			}
			if (propertyInfo == null)
			{
				propertyInfo = (from x in typeinfo.GetProperties()
								where x.PropertyType.Name != "List`1"
								select x).ToArray();
			}
			(from x in dic
			 where !propertyInfo.Any((PropertyInfo p) => p.Name == x.Key)
			 select x into s
			 select s.Key).ToList().ForEach(delegate (string f)
			 {
				 dic.Remove(f);
			 });
			string keyName = typeinfo.GetKeyName();
			if (removerKey)
			{
				dic.Remove(keyName);
			}
			PropertyInfo[] array = propertyInfo;
			foreach (PropertyInfo property in array)
			{
				if (property.Name == keyName || (ignoreFields?.Contains(property.Name) ?? false))
				{
					continue;
				}
				if (!dic.ContainsKey(property.Name))
				{
					if (property.GetCustomAttributes(typeof(RequiredAttribute)).Any() && property.PropertyType != typeof(int) && property.PropertyType != typeof(long) && property.PropertyType != typeof(byte) && property.PropertyType != typeof(decimal))
					{
						return property.GetTypeCustomValue((DisplayAttribute x) => x.Name) + "为必须提交项";
					}
					continue;
				}
				if (!property.ContainsCustomAttributes(typeof(EditableAttribute)))
				{
					if (property.GetCustomAttributes(typeof(RequiredAttribute)).Any())
					{
						return property.GetTypeCustomValue((DisplayAttribute x) => x.Name) + "没有配置好Model为编辑列";
					}
					dic.Remove(property.Name);
					continue;
				}
				(bool, string, object) result = property.ValidationProperty(dic[property.Name], required: false);
				if (!result.Item1)
				{
					return result.Item2;
				}
				if (dic[property.Name] != null && dic[property.Name].ToString() == string.Empty)
				{
					dic[property.Name] = null;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 获取表带有EntityAttribute属性的真实表名
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetEntityTableName(this Type type)
		{
			Attribute attribute = type.GetCustomAttribute(typeof(EntityAttribute));
			if (attribute != null && attribute is EntityAttribute)
			{
				return (attribute as EntityAttribute).TableName ?? type.Name;
			}
			return type.Name;
		}

		/// <summary>
		/// 获取表带有EntityAttribute属性的表中文名
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetEntityTableCnName(this Type type)
		{
			Attribute attribute = type.GetCustomAttribute(typeof(EntityAttribute));
			if (attribute != null && attribute is EntityAttribute)
			{
				return (attribute as EntityAttribute).TableCnName;
			}
			return string.Empty;
		}

		private static object MapToInstance(this Type reslutType, object sourceEntity, PropertyInfo[] sourcePro, PropertyInfo[] reslutPro, string[] sourceFilterField, string[] reslutFilterField, string mapType = null)
		{
			if (mapType == null)
			{
				mapType = GetMapType(reslutType);
			}
			if (sourcePro == null)
			{
				sourcePro = sourceEntity.GetType().GetProperties();
			}
			if (reslutPro == null)
			{
				reslutPro = reslutType.GetProperties();
			}
			object newObj = Activator.CreateInstance(reslutType);
			if (mapType == "Dictionary")
			{
				if (sourceFilterField != null && sourceFilterField.Length != 0)
				{
					sourcePro = sourcePro.Where((PropertyInfo x) => sourceFilterField.Contains(x.Name)).ToArray();
				}
				PropertyInfo[] array = sourcePro;
				foreach (PropertyInfo property2 in array)
				{
					(newObj as IDictionary).Add(property2.Name, property2.GetValue(sourceEntity));
				}
				return newObj;
			}
			if (reslutFilterField != null && reslutFilterField.Length != 0)
			{
				reslutPro.Where((PropertyInfo x) => reslutFilterField.Contains(x.Name));
			}
			PropertyInfo[] array2 = reslutPro;
			foreach (PropertyInfo property in array2)
			{
				PropertyInfo info = sourcePro.Where((PropertyInfo x) => x.Name == property.Name).FirstOrDefault();
				if (info != null && info.PropertyType == property.PropertyType)
				{
					property.SetValue(newObj, info.GetValue(sourceEntity));
				}
			}
			return newObj;
		}

		private static string GetMapType(Type type)
		{
			return (typeof(Dictionary<,>) == type) ? "Dictionary" : "entity";
		}

		/// <summary>
		/// 将数据源映射到新的数据中
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="source"></param>
		/// <param name="resultExpression"></param>
		/// <param name="sourceExpression"></param>
		/// <returns></returns>
		public static TResult MapToObject<TSource, TResult>(this TSource source, Expression<Func<TResult, object>> resultExpression, Expression<Func<TSource, object>> sourceExpression = null) where TResult : class
		{
			if (source == null)
			{
				return null;
			}
			string[] sourceFilterField = ((sourceExpression == null) ? (from x in typeof(TSource).GetProperties()
																		select x.Name).ToArray() : sourceExpression.GetExpressionProperty());
			string[] reslutFilterField = resultExpression?.GetExpressionProperty();
			if (!(source is IList))
			{
				return typeof(TResult).MapToInstance(source, null, null, sourceFilterField, reslutFilterField) as TResult;
			}
			Type sourceType = null;
			Type resultType = null;
			IList sourceList = source as IList;
			sourceType = sourceList[0]!.GetType();
			resultType = typeof(TResult).GenericTypeArguments[0];
			IList reslutList = Activator.CreateInstance(typeof(TResult)) as IList;
			PropertyInfo[] sourcePro = sourceType.GetProperties();
			PropertyInfo[] resultPro = resultType.GetProperties();
			string mapType = GetMapType(resultType);
			for (int i = 0; i < sourceList.Count; i++)
			{
				object reslutobj = resultType.MapToInstance(sourceList[i], sourcePro, resultPro, sourceFilterField, reslutFilterField, mapType);
				reslutList.Add(reslutobj);
			}
			return reslutList as TResult;
		}

		/// <summary>
		/// 将一个实体的赋到另一个实体上,应用场景：
		/// 两个实体，a a1= new a();b b1= new b();  a1.P=b1.P; a1.Name=b1.Name;
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="source"></param>
		/// <param name="result"></param>
		/// <param name="expression">指定对需要的字段赋值,格式x=&gt;new {x.Name,x.P},返回的结果只会对Name与P赋值</param>
		public static void MapValueToEntity<TSource, TResult>(this TSource source, TResult result, Expression<Func<TResult, object>> expression = null) where TResult : class
		{
			if (source == null)
			{
				return;
			}
			string[] fields = expression?.GetExpressionToArray();
			PropertyInfo[] reslutPro = ((fields == null) ? result.GetType().GetProperties() : (from x in result.GetType().GetProperties()
																							   where fields.Contains(x.Name)
																							   select x).ToArray());
			PropertyInfo[] sourcePro = source.GetType().GetProperties();
			PropertyInfo[] array = reslutPro;
			foreach (PropertyInfo property in array)
			{
				PropertyInfo info = sourcePro.Where((PropertyInfo x) => x.Name == property.Name).FirstOrDefault();
				if (info != null && info.PropertyType == property.PropertyType)
				{
					property.SetValue(result, info.GetValue(source));
				}
			}
		}
	}
}
