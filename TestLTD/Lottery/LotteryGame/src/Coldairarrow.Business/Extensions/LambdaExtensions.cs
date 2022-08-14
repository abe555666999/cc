using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Coldairarrow.Business;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Util;

namespace Coldairarrow.Business
{
	public static class LambdaExtensions
	{
		/// <summary>
		/// 分页查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int page, int size = 15)
		{
			return queryable.TakeOrderByPage(page, size);
		}

		/// <summary>
		/// 分页查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		public static IQueryable<T> TakeOrderByPage<T>(this IQueryable<T> queryable, int page, int size = 15, Expression<Func<T, Dictionary<object, QueryOrderBy>>> orderBy = null)
		{
			if (page <= 0)
			{
				page = 1;
			}
			return queryable.GetIQueryableOrderBy(orderBy.GetExpressionToDic()).Skip((page - 1) * size).Take(size);
		}

		/// <summary>
		/// 创建lambda表达式：p=&gt;true
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> True<T>()
		{
			return (T p) => true;
		}

		/// <summary>
		/// 创建lambda表达式：p=&gt;false
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> False<T>()
		{
			return (T p) => false;
		}

		public static ParameterExpression GetExpressionParameter(this Type type)
		{
			return Expression.Parameter(type, "p");
		}

		/// <summary>
		/// 创建lambda表达式：p=&gt;p.propertyName
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Expression<Func<T, TKey>> GetExpression<T, TKey>(this string propertyName)
		{
			return propertyName.GetExpression<T, TKey>(typeof(T).GetExpressionParameter());
		}

		/// <summary>
		/// 创建委托有返回值的表达式：p=&gt;p.propertyName
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Func<T, TKey> GetFun<T, TKey>(this string propertyName)
		{
			return propertyName.GetExpression<T, TKey>(typeof(T).GetExpressionParameter()).Compile();
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="propertyName"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static Expression<Func<T, TKey>> GetExpression<T, TKey>(this string propertyName, ParameterExpression parameter)
		{
			if (typeof(TKey).Name == "Object")
			{
				return Expression.Lambda<Func<T, TKey>>(Expression.Convert(Expression.Property(parameter, propertyName), typeof(object)), new ParameterExpression[1] { parameter });
			}
			return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, propertyName), new ParameterExpression[1] { parameter });
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Expression<Func<T, object>> GetExpression<T>(this string propertyName)
		{
			return propertyName.GetExpression<T, object>(typeof(T).GetExpressionParameter());
		}

		public static Expression<Func<T, object>> GetExpression<T>(this string propertyName, ParameterExpression parameter)
		{
			return Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(parameter, propertyName), typeof(object)), new ParameterExpression[1] { parameter });
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName">字段名</param>
		/// <param name="propertyValue">表达式的值</param>
		/// <param name="expressionType">创建表达式的类型,如:p=&gt;p.propertyName != propertyValue 
		/// p=&gt;p.propertyName.Contains(propertyValue)</param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> CreateExpression<T>(this string propertyName, object propertyValue, LinqExpressionType expressionType)
		{
			return propertyName.CreateExpression<T>(propertyValue, null, expressionType);
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName">字段名</param>
		/// <param name="propertyValue">表达式的值</param>
		/// <param name="expressionType">创建表达式的类型,如:p=&gt;p.propertyName != propertyValue 
		/// p=&gt;p.propertyName.Contains(propertyValue)</param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private static Expression<Func<T, bool>> CreateExpression<T>(this string propertyName, object propertyValue, ParameterExpression parameter, LinqExpressionType expressionType)
		{
			Type proType = typeof(T).GetProperty(propertyName)!.PropertyType;
			if (parameter == null)
			{
				parameter = Expression.Parameter(typeof(T), "p");
			}
			MemberExpression memberProperty = Expression.PropertyOrField(parameter, propertyName);
			if (expressionType == LinqExpressionType.In)
			{
				IList list = propertyValue as IList;
				if (list == null || list.Count == 0)
				{
					throw new Exception("属性值类型不正确");
				}
				bool isStringValue = true;
				List<object> objList = new List<object>();
				if (proType.ToString() != "System.String")
				{
					isStringValue = false;
					foreach (object value2 in list)
					{
						objList.Add(value2.ToString().ChangeType(proType));
					}
					list = objList;
				}
				if (isStringValue)
				{
					MethodInfo method2 = typeof(IList).GetMethod("Contains");
					ConstantExpression constantCollection = Expression.Constant(list);
					MethodCallExpression methodCall = Expression.Call(constantCollection, method2, memberProperty);
					return Expression.Lambda<Func<T, bool>>(methodCall, new ParameterExpression[1] { parameter });
				}
				BinaryExpression body = null;
				foreach (object value in list)
				{
					ConstantExpression constantExpression = Expression.Constant(value);
					UnaryExpression unaryExpression = Expression.Convert(memberProperty, constantExpression.Type);
					body = ((body == null) ? Expression.Equal(unaryExpression, constantExpression) : Expression.OrElse(body, Expression.Equal(unaryExpression, constantExpression)));
				}
				return Expression.Lambda<Func<T, bool>>(body, new ParameterExpression[1] { parameter });
			}
			ConstantExpression constant = ((proType.ToString() == "System.String") ? Expression.Constant(propertyValue) : Expression.Constant(propertyValue.ToString().ChangeType(proType)));
			UnaryExpression member = Expression.Convert(memberProperty, constant.Type);
			Expression<Func<T, bool>> expression;
			switch (expressionType)
			{
				case LinqExpressionType.Equal:
					expression = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, constant), new ParameterExpression[1] { parameter });
					break;
				case LinqExpressionType.NotEqual:
					expression = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, constant), new ParameterExpression[1] { parameter });
					break;
				case LinqExpressionType.GreaterThan:
					expression = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, constant), new ParameterExpression[1] { parameter });
					break;
				case LinqExpressionType.LessThan:
					expression = Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, constant), new ParameterExpression[1] { parameter });
					break;
				case LinqExpressionType.ThanOrEqual:
					expression = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, constant), new ParameterExpression[1] { parameter });
					break;
				case LinqExpressionType.LessThanOrEqual:
					expression = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, constant), new ParameterExpression[1] { parameter });
					break;
				case LinqExpressionType.Contains:
				case LinqExpressionType.NotContains:
					{
						MethodInfo method = typeof(string).GetMethod("Contains", new Type[1] { typeof(string) });
						constant = Expression.Constant(propertyValue, typeof(string));
						expression = ((expressionType != LinqExpressionType.Contains) ? Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Call(member, method, constant)), new ParameterExpression[1] { parameter }) : Expression.Lambda<Func<T, bool>>(Expression.Call(member, method, constant), new ParameterExpression[1] { parameter }));
						break;
					}
				default:
					expression = False<T>();
					break;
			}
			return expression;
		}

		/// <summary>
		///  表达式转换成KeyValList(主要用于多字段排序，并且多个字段的排序规则不一样)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<string, QueryOrderBy>> GetExpressionToPair<T>(this Expression<Func<T, Dictionary<object, QueryOrderBy>>> expression)
		{
			foreach (ElementInit exp in ((ListInitExpression)expression.Body).Initializers)
			{
				yield return new KeyValuePair<string, QueryOrderBy>((exp.Arguments[0] is MemberExpression) ? (exp.Arguments[0] as MemberExpression).Member.Name.ToString() : ((exp.Arguments[0] as UnaryExpression).Operand as MemberExpression).Member.Name, (QueryOrderBy)(exp.Arguments[1] as ConstantExpression).Value);
			}
		}

		/// <summary>
		/// 表达式转换成KeyValList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static Dictionary<string, QueryOrderBy> GetExpressionToDic<T>(this Expression<Func<T, Dictionary<object, QueryOrderBy>>> expression)
		{
			if (expression == null)
			{
				return new Dictionary<string, QueryOrderBy>();
			}
			return expression.GetExpressionToPair().Reverse().ToList()
				.ToDictionary((KeyValuePair<string, QueryOrderBy> x) => x.Key, (KeyValuePair<string, QueryOrderBy> x) => x.Value);
		}

		/// <summary>
		/// 解析多字段排序
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="orderBySelector">string=排序的字段,bool=true降序/false升序</param>
		/// <returns></returns>
		public static IQueryable<TEntity> GetIQueryableOrderBy<TEntity>(this IQueryable<TEntity> queryable, Dictionary<string, QueryOrderBy> orderBySelector)
		{
			string[] orderByKeys = orderBySelector.Select((KeyValuePair<string, QueryOrderBy> x) => x.Key).ToArray();
			if (orderByKeys == null || orderByKeys.Length == 0)
			{
				return queryable;
			}
			IOrderedQueryable<TEntity> queryableOrderBy = null;
			string orderByKey = orderByKeys[^1];
			queryableOrderBy = ((orderBySelector[orderByKey] != QueryOrderBy.Desc) ? queryable.OrderBy(orderByKey.GetExpression<TEntity>()) : (queryableOrderBy = queryable.OrderByDescending(orderByKey.GetExpression<TEntity>())));
			for (int i = orderByKeys.Length - 2; i >= 0; i--)
			{
				queryableOrderBy = ((orderBySelector[orderByKeys[i]] == QueryOrderBy.Desc) ? queryableOrderBy.ThenByDescending(orderByKeys[i].GetExpression<TEntity>()) : queryableOrderBy.ThenBy(orderByKeys[i].GetExpression<TEntity>()));
			}
			return queryableOrderBy;
		}

		/// <summary>
		/// 获取对象表达式指定属性的值
		/// 如获取:Out_Scheduling对象的ID或基他字段
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="expression">格式 Expression new {x.v1,x.v2} or x=&gt;x.v1 解析里面的值返回为数组</param>
		/// <returns></returns>
		public static string[] GetExpressionToArray<TEntity>(this Expression<Func<TEntity, object>> expression)
		{
			string[] propertyNames = null;
			MemberExpression expression2 = expression.Body as MemberExpression;
			if (expression2 != null)
			{
				return new string[1] { expression2.Member.Name };
			}
			return expression.GetExpressionProperty().Distinct().ToArray();
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listExpress"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> And<T>(List<ExpressionParameters> listExpress)
		{
			return listExpress.Compose<T>(Expression.And);
		}

		/// <summary>
		/// 同上面and用法相同
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listExpress"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> Or<T>(this List<ExpressionParameters> listExpress)
		{
			return listExpress.Compose<T>(Expression.Or);
		}

		private static Expression<Func<T, bool>> Compose<T>(this List<ExpressionParameters> listExpress, Func<Expression, Expression, Expression> merge)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
			Expression<Func<T, bool>> expression = null;
			foreach (ExpressionParameters exp in listExpress)
			{
				expression = ((expression != null) ? expression.Compose(exp.Field.GetExpression<T, bool>(parameter), merge) : exp.Field.GetExpression<T, bool>(parameter));
			}
			return expression;
		}

		/// <summary>
		/// https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
		/// 表达式合并(合并生产的sql语句有性能问题)
		/// 合并两个where条件，如：多个查询条件时，判断条件成立才where
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.And);
		}

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.Or);
		}

		public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
		{
			Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters.Select((ParameterExpression f, int i) => new
			{
				f = f,
				s = second.Parameters[i]
			}).ToDictionary(p => p.s, p => p.f);
			Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
			return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
		}

		public static IQueryable<Result> GetQueryableSelect<Source, Result>(this IQueryable<Source> queryable)
		{
			Expression<Func<Source, Result>> expression = CreateMemberInitExpression<Source, Result>();
			return queryable.Select(expression);
		}

		/// <summary>
		/// 动态创建表达式Expression
		/// </summary>
		/// <typeparam name="Source"></typeparam>
		/// <typeparam name="Result"></typeparam>
		/// <param name="resultType"></param>
		/// <returns></returns>
		public static Expression<Func<Source, Result>> CreateMemberInitExpression<Source, Result>(Type resultType = null)
		{
			if ((object)resultType == null)
			{
				resultType = typeof(Result);
			}
			ParameterExpression left = Expression.Parameter(typeof(Source), "p");
			NewExpression newExpression = Expression.New(resultType);
			PropertyInfo[] propertyInfos = resultType.GetProperties();
			List<MemberBinding> memberBindings = new List<MemberBinding>();
			PropertyInfo[] array = propertyInfos;
			foreach (PropertyInfo propertyInfo in array)
			{
				MemberExpression member = Expression.Property(left, propertyInfo.Name);
				MemberBinding speciesMemberBinding = Expression.Bind(resultType.GetMember(propertyInfo.Name)[0], member);
				memberBindings.Add(speciesMemberBinding);
			}
			MemberInitExpression memberInitExpression = Expression.MemberInit(newExpression, memberBindings);
			return Expression.Lambda<Func<Source, Result>>(memberInitExpression, new ParameterExpression[1] { left });
		}

		public static Expression<Func<Source, object>> CreateMemberInitExpression<Source>(Type resultType)
		{
			return CreateMemberInitExpression<Source, object>(resultType);
		}

		/// <summary>
		/// 属性判断待完
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<PropertyInfo> GetGenericProperties(this Type type)
		{
			return type.GetProperties().GetGenericProperties();
		}

		/// <summary>
		/// 属性判断待完
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static IEnumerable<PropertyInfo> GetGenericProperties(this IEnumerable<PropertyInfo> properties)
		{
			return properties.Where((PropertyInfo x) => (!x.PropertyType.IsGenericType && x.PropertyType.GetInterface("IList") == null) || x.PropertyType.GetInterface("IEnumerable", ignoreCase: false) == null);
		}
	}

}
