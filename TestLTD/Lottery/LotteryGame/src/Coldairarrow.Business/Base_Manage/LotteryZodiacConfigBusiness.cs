using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO.Lottery;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryZodiacConfigBusiness : BaseBusiness<LotteryZodiacConfig>, ILotteryZodiacConfigBusiness, ITransientDependency
    {
        public LotteryZodiacConfigBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口

        public async Task<PageResult<LotteryZodiacConfig>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryZodiacConfig>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryZodiacConfig, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryZodiacConfig> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryZodiacConfig data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryZodiacConfig data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task<LotteryZodiacConfig> GetLotZodiacConfigInfo(int openNo)
        {
            var entity = new LotteryZodiacConfig();
            var list = await Db.GetIQueryable<LotteryZodiacConfig>().Where(p => p.IsEnable).ToListAsync();
            if (list.Any() && !openNo.IsNullOrEmpty())
            {
                var zodList = new List<LotZodiacConfig>();

                foreach (var item in list)
                {
                    if (!item.OpenNo.IsNullOrEmpty())
                    {
                        var openNoList = item.OpenNo.Split('|').Select(int.Parse).ToList();
                        zodList.Add(new LotZodiacConfig() { Id = item.Id, ZodiacName = item.Name, List = openNoList });
                    }
                }

                var zodData = zodList.Where(p => p.List.Contains(openNo)).FirstOrDefault();
                if (zodData != null)
                {
                    entity = list.Where(p => p.Id == zodData.Id).FirstOrDefault();
                }
            }
            return entity;
        }

        #endregion

        #region 私有成员

        #endregion
    }
}