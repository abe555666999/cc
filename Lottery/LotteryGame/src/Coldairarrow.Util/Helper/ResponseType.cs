using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Util.Helper
{
	public enum ResponseType
	{
		ServerError = 1,
		LoginExpiration = 302,
		ParametersLack = 303,
		TokenExpiration = 304,
		PINError = 305,
		NoPermissions = 306,
		NoRolePermissions = 307,
		LoginError = 308,
		AccountLocked = 309,
		LoginSuccess = 310,
		SaveSuccess = 311,
		AuditSuccess = 312,
		OperSuccess = 313,
		RegisterSuccess = 314,
		ModifyPwdSuccess = 315,
		EidtSuccess = 316,
		DelSuccess = 317,
		NoKey = 318,
		NoKeyDel = 319,
		KeyError = 320,
		Other = 321
	}

}
