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
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Coldairarrow.Util.Configuration;

namespace Coldairarrow.Business.Base_Manage
{
    public class LotteryResultInfoBusiness : BaseBusiness<LotteryResultInfo>, ILotteryResultInfoBusiness, ITransientDependency
    {
        public LotteryResultInfoBusiness(IDbAccessor db, IMapper mapper, IServiceProvider serviceProvider, ILogger<LotteryResultInfoBusiness> logger)
            : base(db)
        {
            _mapper = mapper;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        readonly IMapper _mapper;
        readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        #region 外部接口

        public async Task<PageResult<LotteryResultInfo>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<LotteryResultInfo>();
            var search = input.Search;

            input.SortField = input.SortField.IsNullOrEmpty() ? "Id" : input.SortField;
            input.SortType = input.SortType.IsNullOrEmpty() ? "DESC" : input.SortType;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<LotteryResultInfo, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<LotteryResultInfo> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(LotteryResultInfo data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(LotteryResultInfo data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        /// <summary>
        /// 获取加拿大28开奖结果信息
        /// </summary>
        /// <param name="lotNumber">期号</param>
        /// <returns></returns>
        /// <exception cref="BusException"></exception>
        public async Task<CanadaOpenResultDTO> GetCanadaOpenResultList(long? lotNumber)
        {
            var result = new CanadaOpenResultDTO();

            var canadaOpenList = new List<CanadaOpenResutInfo>();

            var lotInfo = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Canada && p.IsEnable).FirstOrDefaultAsync();
            if (lotInfo == null)
            {
                throw new BusException("没有该彩种信息");
            }

            var where = LinqHelper.True<LotteryResultInfo>();

            where = where.And(p => p.LotType == (int)LotteryEnum.Canada);

            if (lotNumber.HasValue && lotNumber > 0)
            {
                where = where.And(p => p.LotNumber > lotNumber);
            }

            var lotOpenList = await Db.GetIQueryable<LotteryResultInfo>().Where(where).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

            if (!lotOpenList.Any())
            {
                throw new BusException("没有该彩种的开奖结果信息");
            }
            else
            {
                var newWestExpectInfo = lotOpenList.FirstOrDefault();

                var lastLotNumber = newWestExpectInfo.LotNumber;
                var nextPeriodStartTime = newWestExpectInfo.LotResultTime;
                var lastWestExpectTime = newWestExpectInfo?.LotResultTime;
                

                var nextDrawTime = "";

                var curTaday = DateTime.Today.DayOfWeek.ToString();


                if (!lastWestExpectTime.IsNullOrEmpty())
                {
                    var manyOpenList= await Db.GetIQueryable<LotteryCanadaResultRecord>().Where(p=>p.SourceName == "yuce").OrderByDescending(p => p.LotNumber).Take(50).ToListAsync();
                    if (manyOpenList.Any())
                    {
                        var difTimeHour = AppSetting.DifTimeHour;
                        var manLostOpenInfo = manyOpenList.Where(p => p.LotNumber == lastLotNumber).FirstOrDefault();
                        if (manLostOpenInfo != null)
                        {
                            lastWestExpectTime = manLostOpenInfo.LotResultTime;

                            //lastWestExpectTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm:ss")).AddHours(difTimeHour).ToString("HH:mm:ss"));

                            lastWestExpectTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm:ss")).ToString("HH:mm:ss"));
                        }
                        else
                        {
                            var manOpenInfo = manyOpenList.FirstOrDefault();
                            var manLastLotNumber = manOpenInfo.LotNumber;
                            var difNum = lastLotNumber - manLastLotNumber;

                            if (difNum <= 10)
                            {
                                //lastWestExpectTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm:ss")).AddSeconds(Convert.ToInt32(lotInfo.Intervals * difNum)).AddHours(difTimeHour).ToString("HH:mm:ss"));

                                lastWestExpectTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm:ss")).AddSeconds(Convert.ToInt32(lotInfo.Intervals * difNum)).ToString("HH:mm:ss"));
                            }
                        }
                    }

                    nextDrawTime = await GetNextDrawTime(lastWestExpectTime, lotInfo); //Next Open Time
                }

                canadaOpenList = _mapper.Map<List<LotteryResultInfo>, List<CanadaOpenResutInfo>>(lotOpenList);

                var nowTime = DateTime.Now;
                result.List = new List<CanadaOpenResutInfo>();
                result.List.AddRange(canadaOpenList);
                result.NextDrawInfo = new CanadaNextNoOpenTimeInfo()
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
        ///抓取加拿大28( http://jb28.cc/ )
        /// </summary>
        /// <returns></returns>
        public async Task GetCanadaOpenResultInfo()
        {
            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Canada && p.IsEnable).FirstOrDefaultAsync();

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
                        var nodeInfo = doc.GetElementbyId("tb1");
                        var list = HtmlHelper.GetLotterNodeData(nodeInfo, (int)entity.LotType);
                        var lotResultList = new List<LotteryResultInfo>();

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                //LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo
                                var lotNo = item.GetPropertyValue("LotNo");
                                var lotOpenTime = item.GetPropertyValue("LotOpenTime");
                                var lotOpenNo = item.GetPropertyValue("LotOpenNo");

                                if (lotNo.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                lotResultList.Add(new LotteryResultInfo()
                                {
                                    Id = IdHelper.GetId(),
                                    LotId = entity.Id,
                                    LotNumber = Convert.ToInt64(lotNo),
                                    LotResultTime = Convert.ToString(lotOpenTime),
                                    LotResultNo = Convert.ToString(lotOpenNo),
                                    CreatTime = DateTime.Now,
                                    BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                                    IsResult = true,
                                    LotType = entity.LotType
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


        /// <summary>
        /// 抓取加拿大28数据
        /// </summary>
        /// <returns></returns>
        public async Task GetLotCanadaOpenResultInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Canada && p.IsEnable).FirstOrDefaultAsync();

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
        /// 加拿大28、牛牛28 、百家乐28
        /// </summary>
        /// <returns></returns>
        public async Task GetLotManyOpenResultInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var entity = await Db.GetIQueryable<LotteryInfo>().Where(p => p.LotType == (int)LotteryEnum.Canada && p.IsEnable).FirstOrDefaultAsync();

            if (entity != null)
            {
                var urlList = await Db.GetIQueryable<LotteryURLConfig>().Where(p => p.SiteName == "Many-yuce" && p.IsEnable).ToListAsync();
                if (urlList.Any())
                {
                    await GrabManyLotOpenResultData(entity, urlList);
                }
            }

            stopwatch.Stop();

            TimeSpan ts = stopwatch.Elapsed;

            var dlapTime = $"小时:{ts.Hours}, 分钟:{ts.Minutes}, 秒:{ts.Seconds}, 毫秒:{ts.Milliseconds / 10}";

            _logger.LogInformation($"获取多彩种开奖数据用时: {dlapTime}");
        }

        public async Task GrabManyLotOpenResultData(LotteryInfo entity, List<LotteryURLConfig> urlList)
        {
            await Task.Run(async () =>
            {
                foreach (var item in urlList)
                {
                    var sitName = item.SiteName;
                    var grabURL = item.GrabURL;
                    switch (sitName)
                    {
                        case "Many-yuce":    // 加拿大28、牛牛28、百家乐28
                            await GetManyOpenResult(entity, item);
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// GetManyOpenResult
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetManyOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
        {
            try
            {
                if (urlConfig != null)
                {
                    var grabURL = urlConfig.GrabURL;
                    if (!grabURL.IsNullOrEmpty())
                    {
                        var dic = new Dictionary<string, string>();
                        dic.Add("referer", grabURL);
                        dic.Add("authority", grabURL);
                        dic.Add("accept", "application/json");
                        dic.Add("accept-encoding", "gzip, deflate");

                        var str = HttpHelper.HttpPost(grabURL, dic);
                        if (!str.IsNullOrEmpty())
                        {
                            await AnalyticalManyLotData(entity, urlConfig, str);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"获取多彩种数据错误(GetManyOpenResult): ErrorInfo:{ex}");
            }

        }

        /// <summary>
        /// AnalyticalManyLotData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task AnalyticalManyLotData(LotteryInfo entity, LotteryURLConfig urlConfig, string str)
        {
            try
            {
                var canadaList = new List<object>();

                var cowsList = new List<object>();

                var baccaratList = new List<object>();

                if (!str.IsNullOrEmpty())
                {
                    #region

                    var info = str.ToObject<YCOpenResultInfo>();
                    if (info != null)
                    {
                        var data = info.data;
                        if (data.Any())
                        {
                            foreach (var item in data)
                            {
                                var isOpen = item.isOpen;
                                if (isOpen)
                                {
                                    var section = item.section;
                                    var openTime = item.openTime;

                                    var getTime = Convert.ToString(openTime).ConvertStringToDateTime();

                                    var middleCode = item.middleCode;
                                    var openNum = item.openNum;
                                    var openTime_s = item.openTime_s;

                                    var lotNo = item.section;
                                    //var lotOpenTime = (Convert.ToString(openTime).ConvertStringToDateTime()).ToString("HH:mm:ss");

                                    var lotOpenTime = (Convert.ToString(openTime).ConvertStringToDateTime().AddHours(AppSetting.DifTimeHour)).ToString("HH:mm:ss");

                                    var lotCanaOpenNo = "";
                                    var middleCodeArray = middleCode.Split(',');
                                    var one = middleCodeArray[0];
                                    var two = middleCodeArray[1];
                                    var three = middleCodeArray[2];

                                    lotCanaOpenNo = $"{middleCodeArray[0]} + {middleCodeArray[1]} + {middleCodeArray[2]} = {openNum}";

                                    canadaList.Add(new { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotCanaOpenNo });

                                    var oneNo = ConvertHelper.GetCowsResultNo(Convert.ToInt32(middleCodeArray[0]));
                                    var twoNo = ConvertHelper.GetCowsResultNo(Convert.ToInt32(middleCodeArray[1]));
                                    var threeNo = ConvertHelper.GetCowsResultNo(Convert.ToInt32(middleCodeArray[2]));

                                    cowsList.Add(new { LotNo = lotNo, OneNo = oneNo, TwoNo = twoNo, ThreeNo = threeNo });

                                    var baccaratLotOpenNo = $"{one}{two}{three}";
                                    var lotPretend = ConvertHelper.GetBaccaratZXD(middleCodeArray);
                                    var lotOutCome = ConvertHelper.GetBaccaratResult(middleCodeArray);

                                    baccaratList.Add(new { LotNo = lotNo, LotOpenNo = baccaratLotOpenNo, LotPretend = lotPretend, LotOutCome = lotOutCome });
                                }
                            }
                        }
                    }

                    #endregion

                }

                if (canadaList.Any())
                {
                    entity.LotType = (int)LotteryEnum.Canada;
                    await AddOpenResultData(entity, urlConfig, canadaList);
                }
                if (cowsList.Any())   //AddCowsOpenResultData
                {
                    entity.LotType = (int)LotteryEnum.Cows;
                    await _serviceProvider.GetRequiredService<ILotteryCowsResultInfoBusiness>().AddCowsOpenResultData(entity, urlConfig, cowsList);
                }
                if (baccaratList.Any())
                {
                    entity.LotType = (int)LotteryEnum.BAC;
                    await _serviceProvider.GetRequiredService<ILotteryBaccaratResultInfoBusiness>().AddBaccaratOpenResultData(entity, urlConfig, baccaratList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"多彩种数据处理错误(AnalyticalManyLotData): ErrorInfo:{ex}");
            }
        }


        /// <summary>
        /// 获取下期时间
        /// </summary>
        /// <param name="lastWestExpectTime"></param>
        /// <param name="lotInfo"></param>
        /// <returns></returns>
        public async Task<string> GetNextDrawTime(string lastWestExpectTime, LotteryInfo lotInfo)
        {
            var nextDrawTime = "";

            var curTaday = DateTime.Today.DayOfWeek.ToString();

            if (Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm")) >= Convert.ToDateTime(Convert.ToDateTime(lotInfo.EndTime).ToString("HH:mm")) && Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm")) <= Convert.ToDateTime(Convert.ToDateTime(lotInfo.StartTime).ToString("HH:mm")))
            {
                if (curTaday.ToUpper() == "MONDAY")
                {
                    //每三分半钟开一期，每天维护时间为：晚上19:00点到20:00点，周一延迟一个小时
                    //nextDrawTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lotInfo.StartTime).ToString("HH:mm:ss")).AddHours(1).AddSeconds(Convert.ToInt32(lotInfo.Intervals)).ToString("HH:mm:ss"));

                    nextDrawTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lotInfo.StartTime).ToString("HH:mm:ss")).AddHours(AppSetting.MondayDelayHour).ToString("HH:mm:ss"));
                }
                else
                {
                    //nextDrawTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lotInfo.StartTime).ToString("HH:mm:ss")).AddSeconds(Convert.ToInt32(lotInfo.Intervals)).ToString("HH:mm:ss"));

                    nextDrawTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lotInfo.StartTime).ToString("HH:mm:ss")).AddHours(AppSetting.OtherDelayHour).ToString("HH:mm:ss"));
                }
            }
            else
            {
                nextDrawTime = Convert.ToString(Convert.ToDateTime(Convert.ToDateTime(lastWestExpectTime).ToString("HH:mm:ss")).AddSeconds(Convert.ToInt32(lotInfo.Intervals)).ToString("HH:mm:ss"));
            }

            return await Task.FromResult(nextDrawTime);
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
                        case "Many-yuce":    // 加拿大28、牛牛28、百家乐28
                            await GetManyOpenResult(entity, item);
                            break;
                        case "Canada-jb28":    //Jb 加拿大28
                            await GetCanadaJBOpenResult(entity, item);
                            break;
                        case "Canada-dake28":  //DK 加拿大28    
                            await GetDKOpenResult(entity, item);
                            break;
                        case "Canada-niubi28":  //NB 加拿大28    
                            await GetNiuBiOpenResult(entity, item);
                            break;
                        case "Canada-zizi28":  //ZiZi 加拿大28    
                            await GetZiZiOpenResult(entity, item);
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
        private async Task GetCanadaJBOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
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
                        var nodeInfo = doc.GetElementbyId("tb1");
                        var list = LotHtmlHelper.GetJBCanadaData(nodeInfo);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity, urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// DK Canada28
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <returns></returns>
        private async Task GetDKOpenResult(LotteryInfo entity, LotteryURLConfig urlConfig)
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
                        var lotOpenResultNodeList = doc.DocumentNode.SelectNodes("//tbody//tr");
                        var list = LotHtmlHelper.GetCanadaKadeData(lotOpenResultNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity, urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// NB Canada28
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
                        var list = LotHtmlHelper.GetNBCanadaOpenResultData(lotOpenResultNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity, urlConfig, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// NB Canada28
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
                        var list = LotHtmlHelper.GetZiZiCanadaOpenResultData(htmlNodeList);
                        if (list.Any())
                        {
                            await AddOpenResultData(entity, urlConfig, list);
                        }
                    }
                }
            }
        }

        #endregion

        #region 私有成员

        /// <summary>
        /// AddCanaOpenResultData
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="urlConfig"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task AddOpenResultData(LotteryInfo entity, LotteryURLConfig urlConfig, List<object> list)
        {
            var lotResultList = new List<LotteryResultInfo>();

            if (list.Any())
            {
                foreach (var item in list)
                {
                    //LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo
                    var lotNo = item.GetPropertyValue("LotNo");
                    var lotOpenTime = item.GetPropertyValue("LotOpenTime");
                    var lotOpenNo = item.GetPropertyValue("LotOpenNo");

                    if (lotNo.IsNullOrEmpty())
                    {
                        continue;
                    }

                    lotResultList.Add(new LotteryResultInfo()
                    {
                        Id = IdHelper.GetId(),
                        LotId = entity.Id,
                        LotNumber = Convert.ToInt64(lotNo),
                        LotResultTime = Convert.ToString(Convert.ToDateTime(lotOpenTime.ToString()).ToString("HH:mm:ss")),
                        LotResultNo = Convert.ToString(lotOpenNo),
                        CreatTime = DateTime.Now,
                        BJCreatTime = DateTime.Now.AddHours(AppSetting.TimeZoneHour),
                        IsResult = true,
                        LotType = entity.LotType,
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
        private async Task AddLotteryOpenData(LotteryInfo lotInfo, List<LotteryResultInfo> list)
        {
            if (list.Any())
            {
                var needList = new List<LotteryResultInfo>();
                var addList = new List<LotteryResultInfo>();
                var updateList = new List<LotteryResultInfo>();

                var canadaList = await Db.GetIQueryable<LotteryResultInfo>().Where(p => p.LotId == lotInfo.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                var lotNumberList = canadaList.Select(p => p.LotNumber).ToList();

                var currentLastResultInfo = canadaList.FirstOrDefault();

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
                        await Db.UpdateAsync<LotteryResultInfo>(updateList);
                    }
                    if (addList.Any())
                    {
                        //  await Db.InsertAsync<LotteryResultInfo>(addList);

                        var canadaTwoList = await Db.GetIQueryable<LotteryResultInfo>().OrderByDescending(p => p.LotNumber).Take(10).ToListAsync();

                        var newLotNumberNO = canadaTwoList.FirstOrDefault()?.LotNumber ?? 0;
                        var needAddList = addList.Where(p => p.LotNumber > newLotNumberNO).ToList();
                        if (needAddList.Any())
                        {
                            await Db.InsertAsync<LotteryResultInfo>(needAddList);
                        }
                    }
                }
                else
                {
                    var canadaTwoList = await Db.GetIQueryable<LotteryResultInfo>().OrderByDescending(p => p.LotNumber).Take(10).ToListAsync();

                    var newLotNumberNO = canadaTwoList.FirstOrDefault()?.LotNumber??0;
                    var needAddList = list.Where(p => p.LotNumber>newLotNumberNO).ToList();
                    if (needAddList.Any())
                    {
                        await Db.InsertAsync<LotteryResultInfo>(needAddList);
                    }         
                }
            }
        }

        // <summary>
        /// AddLotteryOpenData
        /// </summary>
        /// <param name="lotInfo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [Transactional]
        private async Task AddLotteryOpenRecordData(LotteryInfo lotInfo, LotteryURLConfig urlConfig, List<LotteryResultInfo> lotList)
        {
            if (lotList.Any())
            {
                var list = AutoMapperExtension.MapTo<LotteryResultInfo, LotteryCanadaResultRecord>(lotList);
                if (list.Any())
                {
                    var needList = new List<LotteryCanadaResultRecord>();
                    var addList = new List<LotteryCanadaResultRecord>();
                    var updateList = new List<LotteryCanadaResultRecord>();

                    var canadaList = await Db.GetIQueryable<LotteryCanadaResultRecord>().Where(p => p.LotId == lotInfo.Id && p.URLConfigId == urlConfig.Id).OrderByDescending(p => p.LotNumber).Take(100).ToListAsync();

                    var lotNumberList = canadaList.Select(p => p.LotNumber).ToList();

                    var currentLastResultInfo = canadaList.FirstOrDefault();

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
                            await Db.UpdateAsync<LotteryCanadaResultRecord>(updateList);
                        }
                        if (addList.Any())
                        {
                            await Db.InsertAsync<LotteryCanadaResultRecord>(addList);
                        }
                    }
                    else
                    {
                        await Db.InsertAsync<LotteryCanadaResultRecord>(list);
                    }
                }
            }
        }

        #endregion


    }
}