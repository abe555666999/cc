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
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Coldairarrow.Util.Configuration;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryBaccaratResultInfoBusiness : BaseBusiness<LotteryBaccaratResultInfo>, ILotteryBaccaratResultInfoBusiness, ITransientDependency
    {
        public LotteryBaccaratResultInfoBusiness(IDbAccessor db, IMapper mapper, IServiceProvider serviceProvider, ILogger<LotteryBaccaratResultInfoBusiness> logger)
            : base(db)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;

        #region 外部接口

        public async Task<PageResult<LotteryBaccaratResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryBaccaratResultInfo>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryBaccaratResultInfo, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryBaccaratResultInfo> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryBaccaratResultInfo data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryBaccaratResultInfo data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        #endregion

        /// <summary>
        /// 获取百家乐28开奖结果
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task<BaccaratOpenResultDTO> GetBaccaratOpenResultList(long? lotNumber)
        {
            var result = new BaccaratOpenResultDTO();

            var cowsOpenList = new List<BaccaratOpenResutInfo>();

            var lotInfo = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.BAC).FirstOrDefaultAsync();
            if (lotInfo == null)
            {
                throw new BusException("没有该彩种信息");
            }

            var where = LinqHelper.True<LotteryBaccaratResultInfo>();

            where = where.And(p => p.LotType == (int)LotteryEnum.BAC);

            if (lotNumber.HasValue && lotNumber > 0)
            {
                where = where.And(p => p.LotNumber > lotNumber);
            }

            var lotOpenList = await Db.GetIQueryable<LotteryBaccaratResultInfo>().Where(where).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

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
                    nextDrawTime = await _serviceProvider.GetService<ILotteryResultInfoBusiness>().GetNextDrawTime(lastWestExpectTime, lotInfo);
                }

                cowsOpenList = _mapper.Map<List<LotteryBaccaratResultInfo>, List<BaccaratOpenResutInfo>>(lotOpenList);

                var nowTime = DateTime.Now;
                result.List = new List<BaccaratOpenResutInfo>();
                result.List.AddRange(cowsOpenList);
                result.NextDrawInfo = new BaccaratNextNoOpenTimeInfo()
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
        /// 抓取百家乐28开奖数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task GetBaccaratOpenResultInfo()
        {
            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.BAC && p.IsEnable).FirstOrDefaultAsync();
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
                        var nodeInfo = doc.GetElementbyId("tb3");
                        var list = HtmlHelper.GetLotterNodeData(nodeInfo, (int)entity.LotType);
                        var lotResultList = new List<LotteryBaccaratResultInfo>();

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                // { LotNo = lotNo, LotOpenNo = lotOpenNo, LotPretend = lotPretend, LotOutCome = lotOutCome }

                                var lotNo = item.GetPropertyValue("LotNo");
                                var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                                var lotPretend = item.GetPropertyValue("LotPretend");
                                var lotOutCome = item.GetPropertyValue("LotOutCome");

                                lotResultList.Add(new LotteryBaccaratResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = entity.Id,
                                    LotNumber = Convert.ToInt64(lotNo),
                                    LotResultNo = Convert.ToString(lotOpenNo),
                                    Pretend = Convert.ToString(lotPretend),
                                    OutCome = Convert.ToString(lotOutCome),
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

        #region Lot

        /// <summary>
        /// 抓取百家乐28数据
        /// </summary>
        /// <returns></returns>
        public async Task GetLotBaccaratOpenResultInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.BAC && p.IsEnable).FirstOrDefaultAsync();

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

            _logger.LogInformation($"获取百家乐28开奖数据用时: {dlapTime}");

        }

        /// <summary>
        /// AddBaccaratOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task AddBaccaratOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list)
        {
            var lotResultList = new List<LotteryBaccaratResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    // { LotNo = lotNo, LotOpenNo = lotOpenNo, LotPretend = lotPretend, LotOutCome = lotOutCome }
                    var lotNo = item.GetPropertyValue("LotNo");
                    var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                    var lotPretend = item.GetPropertyValue("LotPretend");
                    var lotOutCome = item.GetPropertyValue("LotOutCome");

                    lotResultList.Add(new LotteryBaccaratResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultNo = Convert.ToString(lotOpenNo),
                        Pretend = Convert.ToString(lotPretend),
                        OutCome = Convert.ToString(lotOutCome),
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

                    await AddLotteryOpenRecordData(entity, urlConfig, lotResultList);
                }
            }
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
                        case "Baccarat-jb28":    //Jb 百家乐28
                            await GetJBOpenResult(entity, item);
                            break;
                        case "Baccarat-niubi28":  //NB 百家乐28    
                            await GetNiuBiOpenResult(entity, item);
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// JB GetJBOpenResult
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetJBOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
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
                        var nodeInfo = doc.GetElementbyId("tb3");
                        var list = LotHtmlHelper.GetJBBaccaratData(nodeInfo);
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

                        var lotOpenResultNodeList = doc.DocumentNode.SelectNodes("//tbody")[1].SelectNodes("tr");

                        var list = LotHtmlHelper.GetNBBacOpenResultData(lotOpenResultNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity,urlConfig, list);
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
            var lotResultList = new List<LotteryBaccaratResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    // { LotNo = lotNo, LotOpenNo = lotOpenNo, LotPretend = lotPretend, LotOutCome = lotOutCome }
                    var lotNo = item.GetPropertyValue("LotNo");
                    var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                    var lotPretend = item.GetPropertyValue("LotPretend");
                    var lotOutCome = item.GetPropertyValue("LotOutCome");

                    lotResultList.Add(new LotteryBaccaratResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultNo = Convert.ToString(lotOpenNo),
                        Pretend = Convert.ToString(lotPretend),
                        OutCome = Convert.ToString(lotOutCome),
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

                    await AddLotteryOpenRecordData(entity, urlConfig, lotResultList);
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
        private async Task AddLotteryOpenData(LotteryInfo lotInfo, List<LotteryBaccaratResultInfo> list)
        {
            if (list.Any())
            {
                var needList = new List<LotteryBaccaratResultInfo>();
                var addList = new List<LotteryBaccaratResultInfo>();
                var updateList = new List<LotteryBaccaratResultInfo>();

                var bacList = await Db.GetIQueryable<LotteryBaccaratResultInfo>().Where(p => p.LotId == lotInfo.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = bacList.Select(p => p.LotNumber).ToList();

                var currentLastResultInfo = bacList.FirstOrDefault();

                //获取最新的开奖的期号
                var currentMaxNumber = currentLastResultInfo?.LotNumber ?? 0;
                if (currentMaxNumber > 0)
                {
                    if (currentLastResultInfo.LotResultNo.IsNullOrEmpty())  //没有开奖
                    {
                        var isExist = list.Where(p => p.LotNumber == currentMaxNumber).Any();
                        if (isExist)
                        {
                            var entity = list.Where(p => p.LotNumber == currentMaxNumber).FirstOrDefault();
                            currentLastResultInfo.LotResultNo = entity.LotResultNo;
                            currentLastResultInfo.Pretend = entity.Pretend;
                            currentLastResultInfo.OutCome = entity.OutCome;
                            currentLastResultInfo.LotResultTime = entity.LotResultTime;
                            currentLastResultInfo.IsResult = true;
                            updateList.Add(currentLastResultInfo);

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
                        await Db.UpdateAsync<LotteryBaccaratResultInfo>(updateList);
                    }
                    if (addList.Any())
                    {
                        // await Db.InsertAsync<LotteryBaccaratResultInfo>(addList);

                        var twoList = await Db.GetIQueryable<LotteryBaccaratResultInfo>().OrderByDescending(p => p.LotNumber).Take(10).ToListAsync();

                        var newLotNumberNO = twoList.FirstOrDefault()?.LotNumber ?? 0;
                        var needAddList = addList.Where(p => p.LotNumber > newLotNumberNO).ToList();
                        if (needAddList.Any())
                        {
                            await Db.InsertAsync<LotteryBaccaratResultInfo>(needAddList);
                        }
                    }
                }
                else
                {
                    var twoList = await Db.GetIQueryable<LotteryBaccaratResultInfo>().OrderByDescending(p => p.LotNumber).Take(10).ToListAsync();

                    var newLotNumberNO = twoList.FirstOrDefault()?.LotNumber ?? 0;
                    var needAddList = list.Where(p => p.LotNumber > newLotNumberNO).ToList();
                    if (needAddList.Any())
                    {
                        await Db.InsertAsync<LotteryBaccaratResultInfo>(needAddList);
                    }
                   
                }

                System.Threading.Thread.Sleep(500);
                await UpdateLotteryOpenData(lotInfo, list);
            }
        }

        private async Task UpdateLotteryOpenData(LotteryInfo lotInfo, List<LotteryBaccaratResultInfo> list)
        {
            if (list.Any())
            {
                var lotNumberList = list.Select(p => p.LotNumber).ToList();

                var xgcList = await Db.GetIQueryable<LotteryBaccaratResultInfo>().Where(p => p.LotId == lotInfo.Id && p.IsResult == false && lotNumberList.Contains(p.LotNumber)).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
                if (xgcList.Any())
                {
                    var updateList = new List<LotteryBaccaratResultInfo>();
                    foreach (var item in xgcList)
                    {
                        var entity = list.Where(p => p.LotNumber == item.LotNumber).FirstOrDefault();
                        entity.LotResultNo = entity.LotResultNo;
                        entity.Pretend = entity.Pretend;
                        entity.OutCome = entity.OutCome;
                        entity.LotResultTime = entity.LotResultTime;
                        entity.IsResult = true;
                        updateList.Add(entity);
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryBaccaratResultInfo>(updateList);
                    }
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
        private async Task AddLotteryOpenRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryBaccaratResultInfo> lotList)
        {
            if (lotList.Any())
            {
                var list = AutoMapperExtension.MapTo<LotteryBaccaratResultInfo, LotteryBaccaratResultRecord>(lotList);

                var needList = new List<LotteryBaccaratResultRecord>();
                var addList = new List<LotteryBaccaratResultRecord>();
                var updateList = new List<LotteryBaccaratResultRecord>();

                var bacList = await Db.GetIQueryable<LotteryBaccaratResultRecord>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId == urlConfig.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = bacList.Select(p => p.LotNumber).ToList();

                var currentLastResultInfo = bacList.FirstOrDefault();

                //获取最新的开奖的期号
                var currentMaxNumber = currentLastResultInfo?.LotNumber ?? 0;
                if (currentMaxNumber > 0)
                {
                    if (currentLastResultInfo.LotResultNo.IsNullOrEmpty())  //没有开奖
                    {
                        var isExist = list.Where(p => p.LotNumber == currentMaxNumber).Any();
                        if (isExist)
                        {
                            var entity = list.Where(p => p.LotNumber == currentMaxNumber).FirstOrDefault();
                            currentLastResultInfo.LotResultNo = entity.LotResultNo;
                            currentLastResultInfo.Pretend = entity.Pretend;
                            currentLastResultInfo.OutCome = entity.OutCome;
                            currentLastResultInfo.LotResultTime = entity.LotResultTime;
                            currentLastResultInfo.IsResult = true;
                            updateList.Add(currentLastResultInfo);

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
                        await Db.UpdateAsync<LotteryBaccaratResultRecord>(updateList);
                    }
                    if (addList.Any())
                    {
                        await Db.InsertAsync<LotteryBaccaratResultRecord>(addList);
                    }
                }
                else
                {
                    await Db.InsertAsync<LotteryBaccaratResultRecord>(list);
                }

                System.Threading.Thread.Sleep(500);
                await UpdateLotteryRecordData(lotInfo,urlConfig, list);
            }
        }

        private async Task UpdateLotteryRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryBaccaratResultRecord> list)
        {
            if (list.Any())
            {
                var lotNumberList = list.Select(p => p.LotNumber).ToList();

                var recordList = await Db.GetIQueryable<LotteryBaccaratResultRecord>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId==urlConfig.Id && p.IsResult == false && lotNumberList.Contains(p.LotNumber)).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
                if (recordList.Any())
                {
                    var updateList = new List<LotteryBaccaratResultRecord>();
                    foreach (var item in recordList)
                    {
                        var entity = list.Where(p => p.LotNumber == item.LotNumber).FirstOrDefault();
                        entity.LotResultNo = entity.LotResultNo;
                        entity.Pretend = entity.Pretend;
                        entity.OutCome = entity.OutCome;
                        entity.LotResultTime = entity.LotResultTime;
                        entity.IsResult = true;
                        updateList.Add(entity);
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryBaccaratResultRecord>(updateList);
                    }
                }
            }
        }
        #endregion
    }
}