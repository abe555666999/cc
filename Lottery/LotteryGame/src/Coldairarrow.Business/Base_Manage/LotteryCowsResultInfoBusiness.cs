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
    public class LotteryCowsResultInfoBusiness : BaseBusiness<LotteryCowsResultInfo>, ILotteryCowsResultInfoBusiness, ITransientDependency
    {
        public LotteryCowsResultInfoBusiness(IDbAccessor db, IMapper mapper, IServiceProvider serviceProvider, ILogger<LotteryCowsResultInfoBusiness> logger)
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

        public async Task<PageResult<LotteryCowsResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryCowsResultInfo>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryCowsResultInfo, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryCowsResultInfo> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryCowsResultInfo data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryCowsResultInfo data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        /// <summary>
        /// Cows 28 Open Result
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task<CowsOpenResultDTO> GetCowsOpenResultList(long? lotNumber)
        {
            var result = new CowsOpenResultDTO();

            var cowsOpenList = new List<CowsOpenResutInfo>();

            var lotInfo = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Cows && p.IsEnable).FirstOrDefaultAsync();
            if (lotInfo == null)
            {
                throw new BusException("没有该彩种信息");
            }

            var where = LinqHelper.True<LotteryCowsResultInfo>();

            where = where.And(p => p.LotType == (int)LotteryEnum.Cows);

            if (lotNumber.HasValue && lotNumber > 0)
            {
                where = where.And(p => p.LotNumber > lotNumber);
            }

            var lotOpenList = await Db.GetIQueryable<LotteryCowsResultInfo>().Where(where).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

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
                cowsOpenList = _mapper.Map<List<LotteryCowsResultInfo>, List<CowsOpenResutInfo>>(lotOpenList);

                var nowTime = DateTime.Now;
                result.List = new List<CowsOpenResutInfo>();
                result.List.AddRange(cowsOpenList);
                result.NextDrawInfo = new CowsNextNoOpenTimeInfo()
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
        /// Cows 28
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task GetCowsOpenResultInfo()
        {
            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Cows && p.IsEnable).FirstOrDefaultAsync();
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
                        var nodeInfo = doc.GetElementbyId("tb2");
                        var list = HtmlHelper.GetLotterNodeData(nodeInfo, (int)entity.LotType);
                        var lotResultList = new List<LotteryCowsResultInfo>();

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                // LotNo = lotNo, OneNo = oneNo, TwoNo = twoNo, ThreeNo = threeNo }
                                var lotNo = item.GetPropertyValue("LotNo");
                                var oneNo = item.GetPropertyValue("OneNo");
                                var twoNo = item.GetPropertyValue("TwoNo");
                                var threeNo = item.GetPropertyValue("ThreeNo");

                                if (lotNo.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                lotResultList.Add(new LotteryCowsResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = entity.Id,
                                    LotNumber = Convert.ToInt64(lotNo),
                                    LotResultOneNo = Convert.ToString(oneNo),
                                    LotResultTwoNo = Convert.ToString(twoNo),
                                    LotResultThreeNo = Convert.ToString(threeNo),
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
        /// 抓取牛牛28数据
        /// </summary>
        /// <returns></returns>
        public async Task GetLotCowsOpenResultInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Cows && p.IsEnable).FirstOrDefaultAsync();

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

            _logger.LogInformation($"获取加拿大开奖数据用时: {dlapTime}");

        }

        /// <summary>
        /// AddCowsOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task AddCowsOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list)
        {
            var lotResultList = new List<LotteryCowsResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    // LotNo = lotNo, OneNo = oneNo, TwoNo = twoNo, ThreeNo = threeNo }
                    var lotNo = item.GetPropertyValue("LotNo");
                    var oneNo = item.GetPropertyValue("OneNo");
                    var twoNo = item.GetPropertyValue("TwoNo");
                    var threeNo = item.GetPropertyValue("ThreeNo");

                    if (lotNo.IsNullOrEmpty())
                    {
                        continue;
                    }

                    lotResultList.Add(new LotteryCowsResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultOneNo = Convert.ToString(oneNo),
                        LotResultTwoNo = Convert.ToString(twoNo),
                        LotResultThreeNo = Convert.ToString(threeNo),
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
                        case "Cows-jb28":    //Jb 牛牛28
                            await GetJBOpenResult(entity, item);
                            break;
                        case "Cows-niubi28":  //NB 牛牛28    
                            await GetNiuBiOpenResult(entity, item);
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// JB Cows28
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
                        var nodeInfo = doc.GetElementbyId("tb2");
                        var list = LotHtmlHelper.GetJBCowsData(nodeInfo);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity, urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// NB Cows28
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
                        var list = LotHtmlHelper.GetNBCowsOpenResultData(lotOpenResultNodeList);
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
            var lotResultList = new List<LotteryCowsResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    // LotNo = lotNo, OneNo = oneNo, TwoNo = twoNo, ThreeNo = threeNo }
                    var lotNo = item.GetPropertyValue("LotNo");
                    var oneNo = item.GetPropertyValue("OneNo");
                    var twoNo = item.GetPropertyValue("TwoNo");
                    var threeNo = item.GetPropertyValue("ThreeNo");

                    if (lotNo.IsNullOrEmpty())
                    {
                        continue;
                    }

                    lotResultList.Add(new LotteryCowsResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultOneNo = Convert.ToString(oneNo),
                        LotResultTwoNo = Convert.ToString(twoNo),
                        LotResultThreeNo = Convert.ToString(threeNo),
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


        /// <summary>
        /// AddLotteryOpenData
        /// </summary>
        /// <param name="lotInfo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [Transactional]
        private async Task AddLotteryOpenData(LotteryInfo lotInfo, List<LotteryCowsResultInfo> list)
        {
            if (list.Any())
            {
                var needList = new List<LotteryCowsResultInfo>();
                var addList = new List<LotteryCowsResultInfo>();
                var updateList = new List<LotteryCowsResultInfo>();

                var cowsList = await Db.GetIQueryable<LotteryCowsResultInfo>().Where(p => p.LotId == lotInfo.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = cowsList.Select(p => p.LotNumber).ToList();

                var currentLastResultInfo = cowsList.FirstOrDefault();

                //获取最新的开奖的期号
                var currentMaxNumber = currentLastResultInfo?.LotNumber ?? 0;

                if (currentMaxNumber > 0)
                {
                    if (currentLastResultInfo.LotResultOneNo.IsNullOrEmpty() || currentLastResultInfo.LotResultTwoNo.IsNullOrEmpty() || currentLastResultInfo.LotResultThreeNo.IsNullOrEmpty())  //没有开奖
                    {
                        var isExist = list.Where(p => p.LotNumber == currentMaxNumber).Any();
                        if (isExist)
                        {
                            var entity = list.Where(p => p.LotNumber == currentMaxNumber).FirstOrDefault();
                            currentLastResultInfo.LotResultOneNo = entity.LotResultOneNo;
                            currentLastResultInfo.LotResultTwoNo = entity.LotResultTwoNo;
                            currentLastResultInfo.LotResultThreeNo = entity.LotResultThreeNo;
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
                        await Db.UpdateAsync<LotteryCowsResultInfo>(updateList);
                    }
                    if (addList.Any())
                    {
                        var twoList = await Db.GetIQueryable<LotteryCowsResultInfo>().OrderByDescending(p => p.LotNumber).Take(10).ToListAsync();
                        var newLotNumberNO = twoList.FirstOrDefault()?.LotNumber ?? 0;
                        var needAddList = addList.Where(p => p.LotNumber > newLotNumberNO).ToList();
                        if (needAddList.Any())
                        {
                            await Db.InsertAsync<LotteryCowsResultInfo>(needAddList);
                        }
                    }
                }
                else
                {
                    var twoList = await Db.GetIQueryable<LotteryCowsResultInfo>().OrderByDescending(p => p.LotNumber).Take(10).ToListAsync();

                    var newLotNumberNO = twoList.FirstOrDefault()?.LotNumber ?? 0;
                    var needAddList = list.Where(p => p.LotNumber > newLotNumberNO).ToList();
                    if (needAddList.Any())
                    {
                        await Db.InsertAsync<LotteryCowsResultInfo>(needAddList);
                    }
                }

                System.Threading.Thread.Sleep(500);
                await UpdateLotteryOpenData(lotInfo, list);
            }
        }

        private async Task UpdateLotteryOpenData(LotteryInfo lotInfo, List<LotteryCowsResultInfo> list)
        {
            if (list.Any())
            {
                var lotNumberList = list.Select(p => p.LotNumber).ToList();

                var xgcList = await Db.GetIQueryable<LotteryCowsResultInfo>().Where(p => p.LotId == lotInfo.Id && p.IsResult == false && lotNumberList.Contains(p.LotNumber)).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
                if (xgcList.Any())
                {
                    var updateList = new List<LotteryCowsResultInfo>();
                    foreach (var item in xgcList)
                    {
                        var entity = list.Where(p => p.LotNumber == item.LotNumber).FirstOrDefault();
                        entity.LotResultOneNo = entity.LotResultOneNo;
                        entity.LotResultTwoNo = entity.LotResultTwoNo;
                        entity.LotResultThreeNo = entity.LotResultThreeNo;
                        entity.LotResultTime = entity.LotResultTime;
                        entity.IsResult = true;
                        updateList.Add(entity);
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryCowsResultInfo>(updateList);
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
        private async Task AddLotteryOpenRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryCowsResultInfo> lotList)
        {
            if (lotList.Any())
            {
                var list = AutoMapperExtension.MapTo<LotteryCowsResultInfo, LotteryCowsResultRecore>(lotList);
                if (list.Any())
                {
                    var needList = new List<LotteryCowsResultRecore>();
                    var addList = new List<LotteryCowsResultRecore>();
                    var updateList = new List<LotteryCowsResultRecore>();

                    var cowsList = await Db.GetIQueryable<LotteryCowsResultRecore>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId == urlConfig.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                    var lotNumberList = cowsList.Select(p => p.LotNumber).ToList();

                    var currentLastResultInfo = cowsList.FirstOrDefault();

                    //获取最新的开奖的期号
                    var currentMaxNumber = currentLastResultInfo?.LotNumber ?? 0;

                    if (currentMaxNumber > 0)
                    {
                        if (currentLastResultInfo.LotResultOneNo.IsNullOrEmpty() || currentLastResultInfo.LotResultTwoNo.IsNullOrEmpty() || currentLastResultInfo.LotResultThreeNo.IsNullOrEmpty())  //没有开奖
                        {
                            var isExist = list.Where(p => p.LotNumber == currentMaxNumber).Any();
                            if (isExist)
                            {
                                var entity = list.Where(p => p.LotNumber == currentMaxNumber).FirstOrDefault();
                                currentLastResultInfo.LotResultOneNo = entity.LotResultOneNo;
                                currentLastResultInfo.LotResultTwoNo = entity.LotResultTwoNo;
                                currentLastResultInfo.LotResultThreeNo = entity.LotResultThreeNo;
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
                            await Db.UpdateAsync<LotteryCowsResultRecore>(updateList);
                        }
                        if (addList.Any())
                        {
                            await Db.InsertAsync<LotteryCowsResultRecore>(addList);
                        }
                    }
                    else
                    {
                        await Db.InsertAsync<LotteryCowsResultRecore>(list);
                    }
                }

                System.Threading.Thread.Sleep(500);
                await UpdateLotteryRecordData(lotInfo, urlConfig, list);
            }
        }

        private async Task UpdateLotteryRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryCowsResultRecore> list)
        {
            if (list.Any())
            {
                var lotNumberList = list.Select(p => p.LotNumber).ToList();

                var xgcList = await Db.GetIQueryable<LotteryCowsResultRecore>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId == urlConfig.Id && p.IsResult == false && lotNumberList.Contains(p.LotNumber)).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
                if (xgcList.Any())
                {
                    var updateList = new List<LotteryCowsResultRecore>();
                    foreach (var item in xgcList)
                    {
                        var entity = list.Where(p => p.LotNumber == item.LotNumber).FirstOrDefault();
                        entity.LotResultOneNo = entity.LotResultOneNo;
                        entity.LotResultTwoNo = entity.LotResultTwoNo;
                        entity.LotResultThreeNo = entity.LotResultThreeNo;
                        entity.LotResultTime = entity.LotResultTime;
                        entity.IsResult = true;
                        updateList.Add(entity);
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryCowsResultRecore>(updateList);
                    }
                }
            }
        }


        #endregion
    }
}