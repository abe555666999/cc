using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using AutoMapper;


namespace Coldairarrow.Util
{
	public static class AutoMapperExtension
	{
		/// <summary>
		/// 类型映射
		/// </summary>
		/// <typeparam name="TDestination">映射后的对象</typeparam>
		/// <param name="obj">要映射的对象</param>
		/// <returns></returns>
		public static TDestination MapTo<TDestination>(this object obj) where TDestination : class
		{
			if (obj == null)
			{
				return null;
			}
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<TDestination, object>();
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map<TDestination>(obj);
		}

		/// <summary>
		/// 集合列表类型映射
		/// </summary>
		/// <typeparam name="TDestination">目标对象类型</typeparam>
		/// <param name="source">数据源</param>
		/// <returns></returns>
		public static List<TDestination> MapTo<TDestination>(this IEnumerable source) where TDestination : class
		{
			if (source == null)
			{
				return null;
			}
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap(source.GetType(), typeof(TDestination));
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map<List<TDestination>>(source);
		}

		/// <summary>
		/// 集合列表类型映射
		/// </summary>
		/// <typeparam name="TSource">数据源类型</typeparam>
		/// <typeparam name="TDestination">目标对象类型</typeparam>
		/// <param name="source">数据源</param>
		/// <returns></returns>
		public static List<TDestination> MapTo<TSource, TDestination>(this IEnumerable<TSource> source) where TSource : class where TDestination : class
		{
			if (source == null)
			{
				return new List<TDestination>();
			}
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<TSource, TDestination>();
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map<List<TDestination>>(source);
		}

		/// <summary>
		/// 集合列表类型映射
		/// </summary>
		/// <typeparam name="TSource">数据源类型</typeparam>
		/// <typeparam name="TDestination">目标对象类型</typeparam>
		/// <param name="source">数据源</param>
		/// <param name="configure">自定义配置</param>
		/// <returns></returns>
		public static List<TDestination> MapTo<TSource, TDestination>(this IEnumerable<TSource> source, Action<IMapperConfigurationExpression> configure) where TSource : class where TDestination : class
		{
			if (source == null)
			{
				return new List<TDestination>();
			}
			MapperConfiguration config = new MapperConfiguration(configure);
			IMapper mapper = config.CreateMapper();
			return mapper.Map<List<TDestination>>(source);
		}

		/// <summary>
		/// 类型映射
		/// </summary>
		/// <typeparam name="TSource">数据源类型</typeparam>
		/// <typeparam name="TDestination">目标对象类型</typeparam>
		/// <param name="source">数据源</param>
		/// <param name="destination">目标对象</param>
		/// <returns></returns>
		public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) where TSource : class where TDestination : class
		{
			if (source == null)
			{
				return destination;
			}
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<TSource, TDestination>();
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map(source, destination);
		}

		/// <summary>
		/// 类型映射,默认字段名字一一对应
		/// </summary>
		/// <typeparam name="TDestination">转化之后的model，可以理解为viewmodel</typeparam>
		/// <typeparam name="TSource">要被转化的实体，Entity</typeparam>
		/// <param name="source">可以使用这个扩展方法的类型，任何引用类型</param>
		/// <returns>转化之后的实体</returns>
		public static TDestination MapTo<TSource, TDestination>(this TSource source) where TSource : class where TDestination : class
		{
			if (source == null)
			{
				return null;
			}
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<TSource, TDestination>();
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map<TDestination>(source);
		}

		/// <summary>
		/// DataReader映射
		/// </summary>
		/// <typeparam name="T">目标对象类型</typeparam>
		/// <param name="reader">数据源</param>
		/// <returns></returns>
		public static IEnumerable<T> MapTo<T>(this IDataReader reader)
		{
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<IDataReader, IEnumerable<T>>();
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map<IDataReader, IEnumerable<T>>(reader);
		}

		/// <summary>  
		/// 将 DataTable 转为实体对象  
		/// </summary>  
		/// <typeparam name="T">目标对象类型</typeparam>  
		/// <param name="dt">数据源</param>  
		/// <returns></returns>  
		public static List<T> MapTo<T>(this DataTable dt)
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return null;
			}
			MapperConfiguration config = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<IDataReader, List<T>>();
			});
			IMapper mapper = config.CreateMapper();
			return mapper.Map<IDataReader, List<T>>(dt.CreateDataReader());
		}

		/// <summary>
		/// 将List转换为Datatable
		/// </summary>
		/// <typeparam name="T">目标对象类型</typeparam>
		/// <param name="list">数据源</param>
		/// <returns></returns>
		public static DataTable MapTo<T>(this IEnumerable<T> list)
		{
			if (list == null)
			{
				return null;
			}
			List<PropertyInfo> pList = new List<PropertyInfo>();
			Type type = typeof(T);
			DataTable dt = new DataTable();
			Array.ForEach(type.GetProperties(), delegate (PropertyInfo p)
			{
				pList.Add(p);
				dt.Columns.Add(p.Name, p.PropertyType);
			});
			foreach (T item in list)
			{
				DataRow row = dt.NewRow();
				pList.ForEach(delegate (PropertyInfo p)
				{
					row[p.Name] = p.GetValue(item, null);
				});
				dt.Rows.Add(row);
			}
			return dt;
		}
	}
}
