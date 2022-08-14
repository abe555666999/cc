using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Business
{
	/// <summary>
	/// 泛型扩展
	/// </summary>
	public static class GenericExtension
	{
		public static bool Equal<T>(this T x, T y)
		{
			return ((IComparable)(object)x).CompareTo(y) == 0;
		}

		/// <summary>
		/// 将实体指定的字段写入字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static Dictionary<string, object> ToDictionary<T>(this T t, Expression<Func<T, object>> expression) where T : class
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			string[] fields = expression.GetExpressionToArray();
			PropertyInfo[] properties = ((expression == null) ? t.GetType().GetProperties() : (from x in t.GetType().GetProperties()
																							   where fields.Contains(x.Name)
																							   select x).ToArray());
			PropertyInfo[] array = properties;
			foreach (PropertyInfo property in array)
			{
				object value = property.GetValue(t, null);
				dic.Add(property.Name, (value != null) ? value.ToString() : "");
			}
			return dic;
		}

		public static Dictionary<string, string> ToDictionary<TInterface, T>(this TInterface t, Dictionary<string, string> dic = null) where T : class, TInterface
		{
			if (dic == null)
			{
				dic = new Dictionary<string, string>();
			}
			PropertyInfo[] properties = typeof(T).GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo property in array)
			{
				object value = property.GetValue(t, null);
				if (value != null)
				{
					dic.Add(property.Name, (value != null) ? value.ToString() : "");
				}
			}
			return dic;
		}

		public static DataTable ToDataTable<T>(this IEnumerable<T> source, Expression<Func<T, object>> columns = null, bool contianKey = true)
		{
			DataTable dtReturn = new DataTable();
			if (source == null)
			{
				return dtReturn;
			}
			PropertyInfo[] oProps = (from x in typeof(T).GetProperties()
									 where x.PropertyType.Name != "List`1"
									 select x).ToArray();
			if (columns != null)
			{
				string[] columnArray = columns.GetExpressionToArray();
				oProps = oProps.Where((PropertyInfo x) => columnArray.Contains(x.Name)).ToArray();
			}
			PropertyInfo keyType = oProps.GetKeyProperty();
			if (!contianKey && keyType != null && (keyType.PropertyType == typeof(int) || keyType.PropertyType == typeof(long)))
			{
				oProps = oProps.Where((PropertyInfo x) => x.Name != keyType.Name).ToArray();
			}
			PropertyInfo[] array = oProps;
			foreach (PropertyInfo pi in array)
			{
				Type colType = pi.PropertyType;
				if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					colType = colType.GetGenericArguments()[0];
				}
				dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
			}
			foreach (T rec in source)
			{
				DataRow dr = dtReturn.NewRow();
				PropertyInfo[] array2 = oProps;
				foreach (PropertyInfo pi2 in array2)
				{
					dr[pi2.Name] = ((pi2.GetValue(rec, null) == null) ? DBNull.Value : pi2.GetValue(rec, null));
				}
				dtReturn.Rows.Add(dr);
			}
			return dtReturn;
		}
	}
}
