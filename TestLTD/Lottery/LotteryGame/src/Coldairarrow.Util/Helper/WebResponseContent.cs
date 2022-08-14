using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Util.Helper
{
	public class WebResponseContent
	{
		public bool Status { get; set; }

		public string Code { get; set; }

		public string Message { get; set; }

		public object Data { get; set; }

		public static WebResponseContent Instance => new WebResponseContent();

		public WebResponseContent()
		{
		}

		public WebResponseContent(bool status)
		{
			Status = status;
		}

		public WebResponseContent OK()
		{
			Status = true;
			return this;
		}

		public WebResponseContent OK(string message = null, object data = null)
		{
			Status = true;
			Message = message;
			Data = data;
			return this;
		}

		public WebResponseContent OK(ResponseType responseType)
		{
			return Set(responseType, true);
		}

		public WebResponseContent Error(string message = null)
		{
			Status = false;
			Message = message;
			return this;
		}

		public WebResponseContent Error(ResponseType responseType)
		{
			return Set(responseType, false);
		}

		public WebResponseContent Set(ResponseType responseType)
		{
			return Set(responseType, (bool?)null);
		}

		public WebResponseContent Set(ResponseType responseType, bool? status)
		{
			return Set(responseType, null, status);
		}

		public WebResponseContent Set(ResponseType responseType, string msg)
		{
			return Set(responseType, msg, null);
		}

		public WebResponseContent Set(ResponseType responseType, string msg, bool? status)
		{
			if (status.HasValue)
			{
				Status = status.Value;
			}
			int num = (int)responseType;
			Code = num.ToString();
			if (!string.IsNullOrEmpty(msg))
			{
				Message = msg;
				return this;
			}
			return this;
		}
	}
}
