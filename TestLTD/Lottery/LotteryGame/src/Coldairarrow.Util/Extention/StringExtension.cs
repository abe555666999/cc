using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Coldairarrow.Util
{
	public static class StringExtension
	{
		public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		private static DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);

		private static long longTime = 621355968000000000L;

		private static int samllTime = 10000000;

		private static char[] randomConstant = new char[62]
		{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
		'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
		'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
		'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
		'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
		'Y', 'Z'
		};

		public static string ReplacePath(this string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return "";
			}
			if (_windows)
			{
				return path.Replace("/", "\\");
			}
			return path.Replace("\\", "/");
		}

		/// <summary>
		/// 获取时间戳 
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static long GetTimeStamp(this DateTime dateTime)
		{
			return (dateTime.ToUniversalTime().Ticks - longTime) / samllTime;
		}

		/// <summary>
		/// 时间戳转换成日期
		/// </summary>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		public static DateTime GetTimeSpmpToDate(this object timeStamp)
		{
			if (timeStamp == null)
			{
				return dateStart;
			}
			return new DateTime(longTime + Convert.ToInt64(timeStamp) * samllTime, DateTimeKind.Utc).ToLocalTime();
		}

		public static bool IsUrl(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			string Url = "(http://)?([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?";
			return Regex.IsMatch(str, Url);
		}

		/// <summary>
		/// 判断是不是正确的手机号码
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsPhoneNo(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			if (input.Length != 11)
			{
				return false;
			}
			if (new Regex("^1[3578][01379]\\d{8}$").IsMatch(input) || new Regex("^1[34578][01256]\\d{8}").IsMatch(input) || new Regex("^(1[012345678]\\d{8}|1[345678][0123456789]\\d{8})$").IsMatch(input))
			{
				return true;
			}
			return false;
		}

		public static bool GetGuid(this string guid, out Guid outId)
		{
			return Guid.TryParse(guid, out outId);
		}

		public static bool IsGuid(this string guid)
		{
			Guid outId;
			return GetGuid(guid, out outId);
		}

		public static bool IsInt(this object obj)
		{
			if (obj == null)
			{
				return false;
			}
			int result;
			return int.TryParse(obj.ToString(), out result);
		}

		public static bool IsDate(this object str)
		{
			DateTime dateTime;
			return IsDate(str, out dateTime);
		}

		public static bool IsDate(this object str, out DateTime dateTime)
		{
			dateTime = DateTime.Now;
			if (str == null || str.ToString() == "")
			{
				return false;
			}
			return DateTime.TryParse(str.ToString(), out dateTime);
		}

		/// <summary>
		/// 根据传入格式判断是否为小数
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNumber(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			return Regex.IsMatch(str, "^[+-]?\\d*[.]?\\d*$");
		}

		/// <summary>
		/// 判断一个字符串是否为合法数字(指定整数位数和小数位数)
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="precision">整数位数</param>
		/// <param name="scale">小数位数</param>
		/// <returns></returns>
		public static bool IsNumber(this string str, int precision, int scale)
		{
			if (precision == 0 && scale == 0)
			{
				return false;
			}
			string pattern = "(^\\d{1," + precision + "}";
			if (scale > 0)
			{
				pattern = pattern + "\\.\\d{0," + scale + "}$)|" + pattern;
			}
			pattern += "$)";
			return Regex.IsMatch(str, pattern);
		}

		public static int GetInt(this object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			int.TryParse(obj.ToString(), out var _number);
			return _number;
		}

		/// <summary>
		/// 获取 object 中的枚举值
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static long GetLong(this object obj)
		{
			if (obj == null)
			{
				return 0L;
			}
			try
			{
				return Convert.ToInt64(Convert.ToDouble(obj));
			}
			catch
			{
				return 0L;
			}
		}

		/// <summary>
		/// 获取 object 中的 float
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static float GetFloat(this object obj)
		{
			if (DBNull.Value.Equals(obj) || obj == null)
			{
				return 0f;
			}
			try
			{
				return float.Parse(obj.ToString());
			}
			catch
			{
				return 0f;
			}
		}

		public static double GetDouble(this object obj)
		{
			if (DBNull.Value.Equals(obj) || obj == null)
			{
				return 0.0;
			}
			try
			{
				return Convert.ToDouble(obj);
			}
			catch
			{
				return 0.0;
			}
		}

		/// <summary>
		/// 获取 object 中的 decimal
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static decimal GetDecimal(this object obj)
		{
			if (DBNull.Value.Equals(obj) || obj == null)
			{
				return 0m;
			}
			try
			{
				return Convert.ToDecimal(obj);
			}
			catch
			{
				return 0m;
			}
		}

		/// <summary>
		/// 获取 object 中的 decimal
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static dynamic GetDynamic(this object obj)
		{
			if (DBNull.Value.Equals(obj) || obj == null)
			{
				return null;
			}
			try
			{
				string str = obj.ToString();
				if (IsNumber(str, 25, 15))
				{
					return Convert.ToDecimal(obj);
				}
				return str;
			}
			catch
			{
				return string.Empty;
			}
		}

		public static DateTime? GetDateTime(this object obj)
		{
			if (DBNull.Value.Equals(obj) || obj == null)
			{
				return null;
			}
			if (!DateTime.TryParse(obj.ToString(), out var dateTime))
			{
				return null;
			}
			return dateTime;
		}

		public static object ParseTo(this string str, string type)
		{
			return type switch
			{
				"System.Boolean" => ToBoolean(str),
				"System.SByte" => ToSByte(str),
				"System.Byte" => ToByte(str),
				"System.UInt16" => ToUInt16(str),
				"System.Int16" => ToInt16(str),
				"System.uInt32" => ToUInt32(str),
				"System.Int32" => ToInt32(str),
				"System.UInt64" => ToUInt64(str),
				"System.Int64" => ToInt64(str),
				"System.Single" => ToSingle(str),
				"System.Double" => ToDouble(str),
				"System.Decimal" => ToDecimal(str),
				"System.DateTime" => ToDateTimeInfo(str),
				"System.Guid" => ToGuid(str),
				_ => throw new NotSupportedException($"The string of \"{str}\" can not be parsed to {type}"),
			};
		}

		public static sbyte? ToSByte(this string value)
		{
			if (sbyte.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static byte? ToByte(this string value)
		{
			if (byte.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static ushort? ToUInt16(this string value)
		{
			if (ushort.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static short? ToInt16(this string value)
		{
			if (short.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static uint? ToUInt32(this string value)
		{
			if (uint.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static ulong? ToUInt64(this string value)
		{
			if (ulong.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static long? ToInt64(this string value)
		{
			if (long.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static float? ToSingle(this string value)
		{
			if (float.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static double? ToDouble(this string value)
		{
			if (double.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static decimal? ToDecimal(this string value)
		{
			if (decimal.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static bool? ToBoolean(this string value)
		{
			if (bool.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static Guid? ToGuid(this string str)
		{
			if (Guid.TryParse(str, out var value))
			{
				return value;
			}
			return null;
		}

		public static DateTime? ToDateTimeInfo(this string value)
		{
			if (DateTime.TryParse(value, out var value2))
			{
				return value2;
			}
			return null;
		}

		public static int? ToInt32(this string input)
		{
			try
			{
				if (string.IsNullOrEmpty(input))
				{
					return null;
				}
				if (int.TryParse(input, out var value))
				{
					return value;
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		///     替换空格字符
		/// </summary>
		/// <param name="input"></param>
		/// <param name="replacement">替换为该字符</param>
		/// <returns>替换后的字符串</returns>
		public static string ReplaceWhitespace(this string input, string replacement = "")
		{
			return string.IsNullOrEmpty(input) ? null : Regex.Replace(input, "\\s", replacement, RegexOptions.Compiled);
		}

		/// <summary>
		/// 生成指定长度的随机数
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GenerateRandomNumber(this int length)
		{
			StringBuilder newRandom = new StringBuilder(62);
			Random rd = new Random();
			for (int i = 0; i < length; i++)
			{
				newRandom.Append(randomConstant[rd.Next(62)]);
			}
			return newRandom.ToString();
		}
	}
	}

