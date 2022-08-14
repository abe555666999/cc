using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryBaccaratResultRecordBusiness : BaseBusiness<LotteryBaccaratResultRecord>, ILotteryBaccaratResultRecordBusiness, ITransientDependency
    {
        public LotteryBaccaratResultRecordBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口

        public async Task<PageResult<LotteryBaccaratResultRecord>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryBaccaratResultRecord>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryBaccaratResultRecord, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryBaccaratResultRecord> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryBaccaratResultRecord data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryBaccaratResultRecord data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        #endregion

        #region 私有成员

        #endregion
    }
}