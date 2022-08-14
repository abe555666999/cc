using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Entity.Enum
{
    class EnumHelper
    {
    }

    /// <summary>
    /// (加拿大28:1,牛牛28:2,百家乐28:3,澳洲幸运28:4,香港六合彩:5)
    /// </summary>
    public enum LotteryEnum
    {  
        Canada=1,        //加拿大28(jb28.cc)
        Cows =2,         //牛牛28(jb28.cc)
        BAC =3,          //百家乐28(jb28.cc)
        Fortunate =4,    //澳洲幸运28(caishen28.com)
        XGC = 5,         //香港六合彩
        Many=6,    //多彩种
    }

    public enum LinqExpressionType
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        ThanOrEqual,
        LessThanOrEqual,
        In,
        Contains,
        NotContains
    }

    public enum QueryOrderBy
    {
        Desc = 1,
        Asc
    }

    public enum CPrefix
    {
        Role = 0,
        //UserIDkey
        UID = 1,
        /// <summary>
        /// 头像KEY
        /// </summary>
        HDImg = 2,
        Token = 3,
        CityList

    }

    public enum ResponseType
    {
        ServerError = 1,
        LoginExpiration = 302,
        ParametersLack = 303,
        TokenExpiration,
        PINError,
        NoPermissions,
        NoRolePermissions,
        LoginError,
        AccountLocked,
        LoginSuccess,
        SaveSuccess,
        AuditSuccess,
        OperSuccess,
        RegisterSuccess,
        ModifyPwdSuccess,
        EidtSuccess,
        DelSuccess,
        NoKey,
        NoKeyDel,
        KeyError,
        Other
    }
}
