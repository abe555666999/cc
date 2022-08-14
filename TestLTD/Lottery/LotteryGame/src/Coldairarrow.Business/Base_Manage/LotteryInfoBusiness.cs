using Coldairarrow.Business.Dapper;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryInfoBusiness : BaseBusiness<LotteryInfo>, ILotteryInfoBusiness, ITransientDependency
    {
        public LotteryInfoBusiness(IDbAccessor db, ISqlDapper sqlDapper)
            : base(db)
        {
            _sqlDapper = sqlDapper;
        }

        #region 外部接口
        private readonly ISqlDapper _sqlDapper;

        public async Task<PageResult<LotteryInfo>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryInfo>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryInfo, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryInfo> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryInfo data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryInfo data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task<List<LotteryInfo>> GetLotteryInfoList()
        {
            string sql = "select * from LotteryInfo ";
            List<LotteryInfo> list = _sqlDapper.QueryList<LotteryInfo>(sql, null, CommandType.Text);
            return await Task.FromResult(list);
        }

        #endregion

        #region 私有成员

        #endregion

    }
}