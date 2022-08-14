using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using EFCore.Sharding;
using HtmlAgilityPack;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Data;
using System;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.DTO.Lottery;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Coldairarrow.Util.Configuration;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryFortunateResultInfoBusiness : BaseBusiness<LotteryFortunateResultInfo>, ILotteryFortunateResultInfoBusiness, ITransientDependency
    {
        public LotteryFortunateResultInfoBusiness(IDbAccessor db, IMapper mapper, ILogger<LotteryBaccaratResultInfoBusiness> logger)
            : base(db)
        {
            _mapper = mapper;
            _logger = logger;
        }

        readonly IMapper _mapper;
        readonly ILogger _logger;

        #region 外部接口

        public async Task<PageResult<LotteryFortunateResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryFortunateResultInfo>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryFortunateResultInfo, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryFortunateResultInfo> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryFortunateResultInfo data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryFortunateResultInfo data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        /// <summary>
        /// 澳洲幸运28 28 Open Result
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task<FortunateOpenResultDTO> GetFortunateOpenResultList(long? lotNumber)
        {
            var result = new FortunateOpenResultDTO();

            var fortunateOpenList = new List<FortunateOpenResutInfo>();

            var lotInfo = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Fortunate && p.IsEnable).FirstOrDefaultAsync();
            if (lotInfo == null)
            {
                throw new BusException("没有该彩种信息");
            }

            var where = LinqHelper.True<LotteryFortunateResultInfo>();

            where = where.And(p => p.LotType == (int)LotteryEnum.Fortunate);

            if (lotNumber.HasValue && lotNumber > 0)
            {
                where = where.And(p => p.LotNumber > lotNumber);
            }

            var lotOpenList = await Db.GetIQueryable<LotteryFortunateResultInfo>().Where(where).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
            if (!lotOpenList.Any())
            {
                throw new BusException("没有该彩种的开奖结果信息");
            }
            else
            {
                var newWestExpectInfo = lotOpenList.FirstOrDefault();

                var nextPeriodStartTime = newWestExpectInfo.LotResultTime;
                var lastWestExpectTime = newWestExpectInfo?.LotResultTime;
                var nextDrawTime = "";
                if (!lastWestExpectTime.IsNullOrEmpty())
                {
                    nextDrawTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm:ss")).AddSeconds(Convert.ToInt32(lotInfo.Intervals)).ToString("HH:mm:ss"));
                }
                fortunateOpenList = _mapper.Map<List<LotteryFortunateResultInfo>, List<FortunateOpenResutInfo>>(lotOpenList);

                var nowTime = DateTime.Now;
                result.List = new List<FortunateOpenResutInfo>();
                result.List.AddRange(fortunateOpenList);
                result.NextDrawInfo = new FortunateNextNoOpenTimeInfo()
                {
                    Intervals = Convert.ToInt32(lotInfo.Intervals),
                    CurrentTime = nowTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    CurrentBJTime = nowTime.AddHours(AppSetting.TimeZoneHour).ToString("yyyy-MM-dd HH:mm:ss"),
                    StartTime = nextPeriodStartTime,
                    NextDrawTime = nextDrawTime
                };
            }

            return result;
        }

        /// <summary>
        /// Fortunate 28
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task GetFortunateOpenResultInfo()
        {
            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Fortunate && p.IsEnable).FirstOrDefaultAsync();
            if (entity != null)
            {
                var url = entity.GrabURL;
                if (!url.IsNullOrEmpty())
                {
                    var str = await HttpHelper.GetStr(url);
                    if (!str.IsNullOrEmpty())
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(str);

                        var htmlNodeList = doc.DocumentNode.SelectNodes("//table");
                        var list = HtmlHelper.GetFortunateData(htmlNodeList);
                        var lotResultList = new List<LotteryFortunateResultInfo>();

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                // { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenResultNo }
                                var lotNo = item.GetPropertyValue("LotNo");
                                var lotOpenTime = item.GetPropertyValue("LotOpenTime");
                                var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                                if (lotNo.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                lotResultList.Add(new LotteryFortunateResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = entity.Id,
                                    LotNumber = Convert.ToInt64(lotNo),
                                    LotResultTime = Convert.ToString(lotOpenTime),
                                    LotResultNo = Convert.ToString(lotOpenNo),
                                    CreatTime = DateTime.Now,
                                    BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                                    IsResult = true,
                                    LotType = Convert.ToInt32(entity.LotType)
                                });
                            }

                            if (lotResultList.Any())
                            {
                                await AddLotteryOpenData(entity, lotResultList);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Lot

        /// <summary>
        /// 抓取澳洲幸运28数据
        /// </summary>
        /// <returns></returns>
        public async Task GetLotFortunateOpenResultInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Fortunate && p.IsEnable).FirstOrDefaultAsync();

            if (entity != null)
            {
                var urlList = await Db.GetIQueryable<LotteryURLConfig>().Where(p => p.LotType == entity.LotType && p.LotId == entity.Id && p.IsEnable).ToListAsync();
                if (urlList.Any())
                {
                    await GrabLotOpenResultData(entity, urlList);
                }
            }

            stopwatch.Stop();

            TimeSpan ts = stopwatch.Elapsed;

            var dlapTime = $"小时:{ts.Hours}, 分钟:{ts.Minutes}, 秒:{ts.Seconds}, 毫秒:{ts.Milliseconds / 10}";

            _logger.LogInformation($"获取澳洲幸运28开奖数据用时: {dlapTime}");

        }

        #endregion

        #region 

        /// <summary>
        ///  GrabLotOpenResultData 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlList"></param>
        /// <returns></returns>
        private async Task GrabLotOpenResultData(LotteryInfo entity, List<LotteryURLConfig> urlList)
        {
            await Task.Run(async () =>
            {
                foreach (var item in urlList)
                {
                    var sitName = item.SiteName;
                    var grabURL = item.GrabURL;
                    switch (sitName)
                    {
                        case "Fortunate-caishen28":    //CS 澳洲幸运28
                            await GetCSOpenResult(entity, item);
                            break;
                        case "Fortunate-niubi28":      //NB 澳洲幸运28    
                            await GetNiuBiOpenResult(entity, item);
                            break;
                        case "Fortunate-zizi28":      //NB 澳洲幸运28    
                            await GetZiZiOpenResult(entity, item);
                            break;
                        default:
                            break;
                    }
                }
            });
        }


        /// <summary>
        /// CS GetCSOpenResult
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetCSOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
        {
            if (urlConfig != null)
            {
                var grabURL = urlConfig.GrabURL;
                if (!grabURL.IsNullOrEmpty())
                {
                    var str = await HttpHelper.GetStr(grabURL);
                    if (!str.IsNullOrEmpty())
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(str);

                        var htmlNodeList = doc.DocumentNode.SelectNodes("//table");
                        var list = LotHtmlHelper.GetCSFortunateData(htmlNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity,urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// NB GetNiuBiOpenResult
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetNiuBiOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
        {
            if (urlConfig != null)
            {
                var grabURL = urlConfig.GrabURL;
                if (!grabURL.IsNullOrEmpty())
                {
                    var str = await HttpHelper.GetStr(grabURL);
                    if (!str.IsNullOrEmpty())
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(str);
                        var lotOpenResultNodeList = doc.DocumentNode.SelectNodes("//tbody")[0].SelectNodes("tr");
                        var list = LotHtmlHelper.GetNBFortunateOpenResultData(lotOpenResultNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity,urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// NB GetNiuBiOpenResult
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetZiZiOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
        {
            if (urlConfig != null)
            {
                var grabURL = urlConfig.GrabURL;
                if (!grabURL.IsNullOrEmpty())
                {
                    var str = await HttpHelper.GetStr(grabURL);
                    if (!str.IsNullOrEmpty())
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(str);
                        var nodeInfo = doc.GetElementbyId("item1");
                        var htmlNodeList = nodeInfo.SelectNodes("//div[@id='item1']//table//tr");
                        var list = LotHtmlHelper.GetZiZiFortunateOpenResultData(htmlNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity, urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// AddOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task AddOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list)
        {
            var lotResultList = new List<LotteryFortunateResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    // { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenResultNo }
                    var lotNo = item.GetPropertyValue("LotNo");
                    var lotOpenTime = item.GetPropertyValue("LotOpenTime");
                    var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                    if (lotNo.IsNullOrEmpty())
                    {
                        continue;
                    }

                    lotResultList.Add(new LotteryFortunateResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultTime = Convert.ToString(lotOpenTime),
                        LotResultNo = Convert.ToString(lotOpenNo),
                        CreatTime = DateTime.Now,
                        BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                        IsResult = true,
                        LotType = Convert.ToInt32(entity.LotType),
                        URLConfigId = urlConfig.Id,
                        SourceName = urlConfig.SourceName
                    });
                }

                if (lotResultList.Any())
                {
                    await AddLotteryOpenData(entity, lotResultList);

                    await AddLotteryOpenRecordData(entity,urlConfig,lotResultList);
                }
            }
        }

        #endregion


        #region 私有成员

        /// <summary>
        /// AddLotteryOpenData
        /// </summary>
        /// <param name="lotInfo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [Transactional]
        private async Task AddLotteryOpenData(LotteryInfo lotInfo, List<LotteryFortunateResultInfo> list)
        {
            if (list.Any())
            {
                var needList = new List<LotteryFortunateResultInfo>();
                var addList = new List<LotteryFortunateResultInfo>();
                var updateList = new List<LotteryFortunateResultInfo>();

                var fortunateList = await Db.GetIQueryable<LotteryFortunateResultInfo>().Where(p => p.LotId == lotInfo.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = fortunateList.Select(p => p.LotNumber).ToList();

                //获取最新的开奖的期号
                var currentLastResultInfo = fortunateList.FirstOrDefault();

                var currentMaxNumber = currentLastResultInfo?.LotNumber ?? 0;
                if (currentMaxNumber > 0)
                {
                    if (currentLastResultInfo.LotResultNo.IsNullOrEmpty())  //没有开奖
                    {
                        var isExist = list.Where(p => p.LotNumber == currentMaxNumber).Any();
                        if (isExist)
                        {
                            var entity = list.Where(p => p.LotNumber == currentMaxNumber).FirstOrDefault();
                            entity.LotResultNo = entity.LotResultNo;
                            entity.LotResultTime = entity.LotResultTime;
                            entity.IsResult = true;
                            updateList.Add(entity);

                            needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);
                            }
                        }
                        else
                        {
                            needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);
                            }
                        }
                    }
                    else    //已开奖
                    {
                        needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                        if (needList.Any())
                        {
                            addList.AddRange(needList);
                        }
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryFortunateResultInfo>(updateList);
                    }
                    if (addList.Any())
                    {
                        await Db.InsertAsync<LotteryFortunateResultInfo>(addList);
                    }
                }
                else
                {
                    await Db.InsertAsync<LotteryFortunateResultInfo>(list);
                }
            }
        }

        /// <summary>
        /// AddLotteryOpenData
        /// </summary>
        /// <param name="lotInfo"></param>
        /// <param name="urlConfig"></param>
        /// <param name="lotList"></param>
        /// <returns></returns>
        [Transactional]
        private async Task AddLotteryOpenRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryFortunateResultInfo> lotList)
        {
            if (lotList.Any())
            {
                var list = AutoMapperExtension.MapTo<LotteryFortunateResultInfo, LotteryFortunateResultRecore>(lotList);

                var needList = new List<LotteryFortunateResultRecore>();
                var addList = new List<LotteryFortunateResultRecore>();
                var updateList = new List<LotteryFortunateResultRecore>();

                var fortunateList = await Db.GetIQueryable<LotteryFortunateResultRecore>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId == urlConfig.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = fortunateList.Select(p => p.LotNumber).ToList();

                //获取最新的开奖的期号
                var currentLastResultInfo = fortunateList.FirstOrDefault();

                var currentMaxNumber = currentLastResultInfo?.LotNumber ?? 0;
                if (currentMaxNumber > 0)
                {
                    if (currentLastResultInfo.LotResultNo.IsNullOrEmpty())  //没有开奖
                    {
                        var isExist = list.Where(p => p.LotNumber == currentMaxNumber).Any();
                        if (isExist)
                        {
                            var entity = list.Where(p => p.LotNumber == currentMaxNumber).FirstOrDefault();
                            entity.LotResultNo = entity.LotResultNo;
                            entity.LotResultTime = entity.LotResultTime;
                            entity.IsResult = true;
                            updateList.Add(entity);

                            needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);
                            }
                        }
                        else
                        {
                            needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);
                            }
                        }
                    }
                    else    //已开奖
                    {
                        needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                        if (needList.Any())
                        {
                            addList.AddRange(needList);
                        }
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryFortunateResultRecore>(updateList);
                    }
                    if (addList.Any())
                    {
                        await Db.InsertAsync<LotteryFortunateResultRecore>(addList);
                    }
                }
                else
                {
                    await Db.InsertAsync<LotteryFortunateResultRecore>(list);
                }
            }
        }

        #endregion
    }
}