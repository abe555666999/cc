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
using Coldairarrow.Entity.DTO.API;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryXGCResultInfoBusiness : BaseBusiness<LotteryXGCResultInfo>, ILotteryXGCResultInfoBusiness, ITransientDependency
    {
        public LotteryXGCResultInfoBusiness(IDbAccessor db, IMapper mapper, IServiceProvider serviceProvider, ILogger<LotteryXGCResultInfoBusiness> logger)
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

        public async Task<PageResult<LotteryXGCResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryXGCResultInfo>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryXGCResultInfo, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryXGCResultInfo> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryXGCResultInfo data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryXGCResultInfo data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        /// <summary>
        /// 香港彩开奖数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task<XGCOpenResultDTO> GetXGCOpenResultList(long? lotNumber)
        {
            var result = new XGCOpenResultDTO();

            var cowsOpenList = new List<XGCOpenResutInfo>();

            var lotInfo = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.XGC && p.IsEnable).FirstOrDefaultAsync();
            if (lotInfo == null)
            {
                throw new BusException("没有该彩种信息");
            }

            var where = LinqHelper.True<LotteryXGCResultInfo>();

            where = where.And(p => p.LotType == (int)LotteryEnum.XGC);

            if (lotNumber.HasValue && lotNumber > 0)
            {
                where = where.And(p => p.LotNumber > lotNumber);
            }

            var lotOpenDataList = await Db.GetIQueryable<LotteryXGCResultInfo>().Where(where).OrderByDescending(p => p.LotNumber).Take(101).ToListAsync();

            var lotOpenList = lotOpenDataList.Where(p => p.IsResult == true).ToList();

            var lotNeedOpenInfo = lotOpenDataList.Where(p => p.IsResult == false).FirstOrDefault();

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
                cowsOpenList = _mapper.Map<List<LotteryXGCResultInfo>, List<XGCOpenResutInfo>>(lotOpenList);

                var nowTime = DateTime.Now;
                result.List = new List<XGCOpenResutInfo>();
                result.List.AddRange(cowsOpenList);
                result.NeedOpenInfo = new XGCNeedOpenInfo()
                {
                    NeedLotNumber = lotNeedOpenInfo?.LotNumber ?? 0
                };

                result.NextDrawInfo = new XGCNextNoOpenTimeInfo()
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
        /// 抓取香港彩数据
        /// </summary>
        /// <returns></returns>
        public async Task GetXGCOpenResultInfo()
        {
            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.XGC && p.IsEnable).FirstOrDefaultAsync();
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
                        var nodeInfo = doc.GetElementbyId("xgc");
                        var list = LotHtmlHelper.GetJBXGCData(nodeInfo);
                        var lotResultList = new List<LotteryXGCResultInfo>();

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                var indexNum = list.IndexOf(item);
                                var lotNo = item.GetPropertyValue("LotNo");
                                var lotOpenTime = item.GetPropertyValue("LotOpenTime");
                                var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                                var lotSXNo = item.GetPropertyValue("LotSXNo");
                                var lotWXNo = item.GetPropertyValue("LotWXNo");
                                var lotSpecialNo = item.GetPropertyValue("LotSpecialNo");

                                if (lotNo.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                lotResultList.Add(new LotteryXGCResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = entity.Id,
                                    LotNumber = Convert.ToInt64(lotNo),
                                    LotResultTime = Convert.ToString(lotOpenTime),
                                    LotResultNo = Convert.ToString(lotOpenNo),
                                    LotSXResult = Convert.ToString(lotSXNo),
                                    LotWuXingResult = Convert.ToString(lotWXNo),
                                    LotSpecialCode = Convert.ToString(lotSpecialNo),
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
        /// 抓取香港彩数据
        /// </summary>
        /// <returns></returns>
        public async Task GetLotXGCOpenResultInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.XGC && p.IsEnable).FirstOrDefaultAsync();

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

            _logger.LogInformation($"获取香港彩开奖数据用时: {dlapTime}");

        }

        #endregion

        #region 

        /// <summary>
        ///  GrabCandaLotData 
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
                        case "XGC-jb28":    //Jb 香港彩
                            await GetJBOpenResult(entity, item);
                            break;
                        case "XGC-kc":      //kc 香港彩    
                            await GetKJOpenResult(entity, item);
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// JB Canada28
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
                        var nodeInfo = doc.GetElementbyId("xgc");
                        var list = LotHtmlHelper.GetJBXGCData(nodeInfo);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity,urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// KJ XGC
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetKJOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
        {
            if (urlConfig != null)
            {
                var grabURL = urlConfig.GrabURL;
                if (!grabURL.IsNullOrEmpty())
                {
                    var str = await HttpHelper.GetStr(grabURL);
                    if (!str.IsNullOrEmpty())
                    {
                        var curTime = DateTime.Now.Year.ToString().Substring(0, 2);
                        var lotData = str.ToObject<KaiCaiInfo>();
                        if (lotData.code == 0)
                        {
                            var xgcOpenList = lotData.data;
                            if (xgcOpenList.Any())
                            {
                                var list = await GetKCXGCOpenResultData(xgcOpenList);
                                if (list.Any())
                                {
                                    await AddOpenResultData(entity,urlConfig, list);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// AddCanaOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task AddOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list)
        {
            var lotResultList = new List<LotteryXGCResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    var indexNum = list.IndexOf(item);
                    var lotNo = item.GetPropertyValue("LotNo");
                    var lotOpenTime = item.GetPropertyValue("LotOpenTime");
                    var lotOpenNo = item.GetPropertyValue("LotOpenNo");
                    var lotSXNo = item.GetPropertyValue("LotSXNo");
                    var lotWXNo = item.GetPropertyValue("LotWXNo");
                    var lotSpecialNo = item.GetPropertyValue("LotSpecialNo");

                    if (lotNo.IsNullOrEmpty())
                    {
                        continue;
                    }

                    lotResultList.Add(new LotteryXGCResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultTime = Convert.ToString(lotOpenTime),
                        LotResultNo = Convert.ToString(lotOpenNo),
                        LotSXResult = Convert.ToString(lotSXNo),
                        LotWuXingResult = Convert.ToString(lotWXNo),
                        LotSpecialCode = Convert.ToString(lotSpecialNo),
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

        private async Task<List<object>> GetKCXGCOpenResultData(List<KCInfo> openList)
        {
            var list = new List<object>();
            if (openList.Any())
            {
                var zodiacConfigList = await Db.GetIQueryable<LotteryZodiacConfig>().Where(p => p.IsEnable).ToListAsync();
                var curTime = DateTime.Now.Year.ToString().Substring(0, 2);
                foreach (var item in openList)
                {
                    var kcOpenNo = item.issue;
                    var lotNumber = $"{curTime}{kcOpenNo}";
                    var lotOpenTime = item.drawTime;
                    var lotOpenNo = "";
                    var lotSXOpenNo = "";
                    var lotSpecialNo = "";
                    var lotOpenNoList = item.drawResult.IsNullOrEmpty() ? new List<int>() { } : item.drawResult.Split(',').Select(int.Parse).ToList();
                    if (lotOpenNoList.Any())
                    {
                        lotOpenNo = string.Join('|', lotOpenNoList);
                        var sxList = await GetLotZodiacConfigInfo(zodiacConfigList, lotOpenNoList);
                        lotSXOpenNo = string.Join('|', sxList);

                        lotSpecialNo = Convert.ToString(lotOpenNoList.Last());
                    }

                    list.Add(new { LotNo = lotNumber, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo, LotSXNo = lotSXOpenNo, LotWXNo = "", LotSpecialNo = lotSpecialNo });
                }
            }

            return list;
        }

        private async Task<List<string>> GetLotZodiacConfigInfo(List<LotteryZodiacConfig> zodiacConfigList, List<int> openResultList)
        {
            var result = new List<string>();
            var entity = new LotteryZodiacConfig();
            if (zodiacConfigList.Any() && openResultList.Any())
            {
                var zodList = new List<LotZodiacConfig>();

                foreach (var item in zodiacConfigList)
                {
                    if (!item.OpenNo.IsNullOrEmpty())
                    {
                        var openNoList = item.OpenNo.Split('|').Select(int.Parse).ToList();
                        zodList.Add(new LotZodiacConfig() { Id = item.Id, ZodiacName = item.Name, List = openNoList });
                    }
                }

                foreach (var item in openResultList)
                {
                    var openNo = item;
                    var zodData = zodList.Where(p => p.List.Contains(openNo)).FirstOrDefault();
                    if (zodData != null)
                    {
                        entity = zodiacConfigList.Where(p => p.Id == zodData.Id).FirstOrDefault();
                        if (entity != null)
                        {
                            result.Add(entity?.Name ?? "");
                        }
                    }
                }
            }
            return await Task.FromResult(result);
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
        private async Task AddLotteryOpenData(LotteryInfo lotInfo, List<LotteryXGCResultInfo> list)
        {
            if (list.Any())
            {
                var needList = new List<LotteryXGCResultInfo>();
                var addList = new List<LotteryXGCResultInfo>();
                var updateList = new List<LotteryXGCResultInfo>();

                var xgcList = await Db.GetIQueryable<LotteryXGCResultInfo>().Where(p => p.LotId == lotInfo.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = xgcList.Select(p => p.LotNumber).ToList();

                var currentLastResultInfo = xgcList.FirstOrDefault();

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
                            currentLastResultInfo.LotSXResult = entity.LotSXResult;
                            currentLastResultInfo.LotWuXingResult = entity.LotWuXingResult;
                            currentLastResultInfo.LotSpecialCode = entity.LotSpecialCode;
                            currentLastResultInfo.LotResultTime = entity.LotResultTime;
                            currentLastResultInfo.IsResult = true;
                            updateList.Add(currentLastResultInfo);

                            needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);

                                var maxNo = addList.Max(p => p.LotNumber);
                                addList.Add(new LotteryXGCResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = lotInfo.Id,
                                    LotNumber = maxNo += 1,
                                    CreatTime = DateTime.Now,
                                    BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                                    IsResult = false,
                                    LotType = Convert.ToInt32(lotInfo.LotType)
                                });
                            }
                            else
                            {
                                var maxNo = xgcList.Max(p => p.LotNumber);
                                addList.Add(new LotteryXGCResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = lotInfo.Id,
                                    LotNumber = maxNo += 1,
                                    CreatTime = DateTime.Now,
                                    BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                                    IsResult = false,
                                    LotType = Convert.ToInt32(lotInfo.LotType)
                                });
                            }
                        }
                        else
                        {
                            needList = list.Where(p => p.LotNumber > currentMaxNumber).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);
                            }
                        }
                    }
                    else    //已开奖
                    {
                        var maxNo = 0L;
                        needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                        if (needList.Any())
                        {
                            addList.AddRange(needList);
                            maxNo = addList.Max(p => p.LotNumber);
                        }
                        else
                        {
                            maxNo = xgcList.Max(p => p.LotNumber);
                        }

                        addList.Add(new LotteryXGCResultInfo()
                        {
                            Id = IdHelper.GetId(),
                            LotId = lotInfo.Id,
                            LotNumber = maxNo += 1,
                            CreatTime = DateTime.Now,
                            IsResult = false,
                            LotType = Convert.ToInt32(lotInfo.LotType)
                        });
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryXGCResultInfo>(updateList);
                    }
                    if (addList.Any())
                    {
                        await Db.InsertAsync<LotteryXGCResultInfo>(addList);
                    }
                }
                else
                {
                    var maxNo = list.Max(p => p.LotNumber);
                    list.Add(new LotteryXGCResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = lotInfo.Id,
                        LotNumber = maxNo += 1,
                        CreatTime = DateTime.Now,
                        IsResult = false,
                        LotType = Convert.ToInt32(lotInfo.LotType)
                    });

                    await Db.InsertAsync<LotteryXGCResultInfo>(list);
                }

                System.Threading.Thread.Sleep(500);
                await UpdateLotteryOpenData(lotInfo, list);
            }
        }

        private async Task UpdateLotteryOpenData(LotteryInfo lotInfo, List<LotteryXGCResultInfo> list)
        {
            if (list.Any())
            {
                var lotNumberList = list.Select(p => p.LotNumber).ToList();

                var xgcList = await Db.GetIQueryable<LotteryXGCResultInfo>().Where(p => p.LotId == lotInfo.Id && p.IsResult == false && lotNumberList.Contains(p.LotNumber)).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
                if (xgcList.Any())
                {
                    var updateList = new List<LotteryXGCResultInfo>();
                    foreach (var item in xgcList)
                    {
                        var entity = list.Where(p => p.LotNumber == item.LotNumber).FirstOrDefault();
                        item.LotResultNo = entity.LotResultNo;
                        item.LotSXResult = entity.LotSXResult;
                        item.LotWuXingResult = entity.LotWuXingResult;
                        item.LotSpecialCode = entity.LotSpecialCode;
                        item.LotResultTime = entity.LotResultTime;
                        item.IsResult = true;
                        updateList.Add(item);
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryXGCResultInfo>(updateList);
                    }
                }
            }
        }

        /// <summary>
        /// AddLotteryOpenRecordData
        /// </summary>
        /// <param name="lotInfo"></param>
        /// <param name="urlConfig"></param>
        /// <param name="lotList"></param>
        /// <returns></returns>
        [Transactional]
        private async Task AddLotteryOpenRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryXGCResultInfo> lotList)
        {
            if (lotList.Any())
            {
                var list = AutoMapperExtension.MapTo<LotteryXGCResultInfo, LotteryXGCResultRecore>(lotList);

                var needList = new List<LotteryXGCResultRecore>();
                var addList = new List<LotteryXGCResultRecore>();
                var updateList = new List<LotteryXGCResultRecore>();

                var xgcList = await Db.GetIQueryable<LotteryXGCResultRecore>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId==urlConfig.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = xgcList.Select(p => p.LotNumber).ToList();

                var currentLastResultInfo = xgcList.FirstOrDefault();

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
                            currentLastResultInfo.LotSXResult = entity.LotSXResult;
                            currentLastResultInfo.LotWuXingResult = entity.LotWuXingResult;
                            currentLastResultInfo.LotSpecialCode = entity.LotSpecialCode;
                            currentLastResultInfo.LotResultTime = entity.LotResultTime;
                            currentLastResultInfo.IsResult = true;
                            updateList.Add(currentLastResultInfo);

                            needList = list.Where(p => p.LotNumber > currentMaxNumber && !lotNumberList.Contains(p.LotNumber)).ToList();
                            if (needList.Any())
                            {
                                addList.AddRange(needList);

                                var maxNo = addList.Max(p => p.LotNumber);
                                addList.Add(new LotteryXGCResultRecore()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = lotInfo.Id,
                                    LotNumber = maxNo += 1,
                                    CreatTime = DateTime.Now,
                                    BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                                    IsResult = false,
                                    LotType = Convert.ToInt32(lotInfo.LotType)
                                });
                            }
                            else
                            {
                                var maxNo = xgcList.Max(p => p.LotNumber);
                                addList.Add(new LotteryXGCResultRecore()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = lotInfo.Id,
                                    LotNumber = maxNo += 1,
                                    CreatTime = DateTime.Now,
                                    BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                                    IsResult = false,
                                    LotType = Convert.ToInt32(lotInfo.LotType)
                                });
                            }
                        }
                        else
                        {
                            needList = list.Where(p => p.LotNumber > currentMaxNumber).ToList();
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

                            var maxNo = addList.Max(p => p.LotNumber);
                            addList.Add(new LotteryXGCResultRecore()
                            {
                                Id = IdHelper.GetId(),
                                LotId = lotInfo.Id,
                                LotNumber = maxNo += 1,
                                CreatTime = DateTime.Now,
                                IsResult = false,
                                LotType = Convert.ToInt32(lotInfo.LotType)
                            });
                        }
                        else
                        {
                            var maxNo = xgcList.Max(p => p.LotNumber);
                            addList.Add(new LotteryXGCResultRecore()
                            {
                                Id = IdHelper.GetId(),
                                LotId = lotInfo.Id,
                                LotNumber = maxNo += 1,
                                CreatTime = DateTime.Now,
                                IsResult = false,
                                LotType = Convert.ToInt32(lotInfo.LotType)
                            });
                        }
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryXGCResultRecore>(updateList);
                    }
                    if (addList.Any())
                    {
                        await Db.InsertAsync<LotteryXGCResultRecore>(addList);
                    }
                }
                else
                {
                    var maxNo = list.Max(p => p.LotNumber);
                    list.Add(new LotteryXGCResultRecore()
                    {
                        Id = IdHelper.GetId(),
                        LotId = lotInfo.Id,
                        LotNumber = maxNo += 1,
                        CreatTime = DateTime.Now,
                        IsResult = false,
                        LotType = Convert.ToInt32(lotInfo.LotType)
                    });

                    await Db.InsertAsync<LotteryXGCResultRecore>(list);
                }

                System.Threading.Thread.Sleep(500);
                await UpdateLotteryRecordData(lotInfo, urlConfig,list);
            }
        }


        /// <summary>
        /// UpdateLotteryRecordData
        /// </summary>
        /// <param name="lotInfo"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [Transactional]
        private async Task UpdateLotteryRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryXGCResultRecore> list)
        {
            if (list.Any())
            {
                var lotNumberList = list.Select(p => p.LotNumber).ToList();

                var xgcList = await Db.GetIQueryable<LotteryXGCResultRecore>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId == urlConfig.Id && p.IsResult == false && lotNumberList.Contains(p.LotNumber)).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();
                if (xgcList.Any())
                {
                    var updateList = new List<LotteryXGCResultRecore>();
                    foreach (var item in xgcList)
                    {
                        var entity = list.Where(p => p.LotNumber == item.LotNumber).FirstOrDefault();
                        item.LotResultNo = entity.LotResultNo;
                        item.LotSXResult = entity.LotSXResult;
                        item.LotWuXingResult = entity.LotWuXingResult;
                        item.LotSpecialCode = entity.LotSpecialCode;
                        item.LotResultTime = entity.LotResultTime;
                        item.IsResult = true;
                        updateList.Add(item);
                    }

                    if (updateList.Any())
                    {
                        await Db.UpdateAsync<LotteryXGCResultRecore>(updateList);
                    }
                }
            }
        }

        #endregion
    }
}