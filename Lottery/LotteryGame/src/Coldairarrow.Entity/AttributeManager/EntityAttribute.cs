﻿using System;

namespace Coldairarrow.Entity.AttributeManager
{
	public class EntityAttribute : Attribute
	{
		/// <summary>
		/// 真实表名(数据库表名，若没有填写默认实体为表名)
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// 表中文名
		/// </summary>
		public string TableCnName { get; set; }

		/// <summary>
		/// 子表
		/// </summary>
		public Type[] DetailTable { get; set; }

		/// <summary>
		/// 子表中文名
		/// </summary>
		public string DetailTableCnName { get; set; }

		/// <summary>
		/// 数据库
		/// </summary>
		public string DBServer { get; set; }

		public bool CurrentUserPermission { get; set; }

		public Type ApiInput { get; set; }

		public Type ApiOutput { get; set; }
	}
}
