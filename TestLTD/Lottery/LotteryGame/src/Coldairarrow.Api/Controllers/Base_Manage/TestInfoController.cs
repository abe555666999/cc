using Coldairarrow.Entity.DTO.API;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Linq;
using Coldairarrow.Entity.DTO.Lottery;

namespace Coldairarrow.Api.Controllers.Base_Manage
{

    /// <summary>
    /// Test
    /// </summary>
    [Route("/Base_Manage/[controller]/[action]")]
    [OpenApiTag("Test")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestInfoController : BaseApiController
    {

        /// <summary>
        ///  测试获取数据
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet, Route("GetHtmlStr"),]
        public async Task<IActionResult> GetHtmlStr(string url)
        {

            var headStr = HttpContext.Request.Headers["Authorization"];

            var byteStr = await HttpHelper.GetDataAsync(url);

            var rsult = "";
            var str = await HttpHelper.GetStr(url);
            if (!str.IsNullOrEmpty())
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(str);
                var nodeInfo = doc.GetElementbyId("tb1");
                var list = HtmlHelper.GetLotterNodeData(nodeInfo, 1);
                foreach (var item in list)
                {
                    //{ LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo }

                    var indexNum = list.IndexOf(item);
                    if (indexNum > 0)
                    {
                        var LotNo = item.GetPropertyValue("LotNo");
                        var LotOpenTime = item.GetPropertyValue("LotOpenTime");
                        var LotOpenNo = item.GetPropertyValue("LotOpenNo");
                    }
                }
                rsult = string.Join(',', list);
            }

            return Content(rsult);
        }

        /// <summary>
        ///  测试获取数据
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet, Route("GetXYHtmlStr"),]
        public async Task<IActionResult> GetXYHtmlStr(string url)
        {

            var headStr = HttpContext.Request.Headers["Authorization"];

            var str = await HttpHelper.GetStr(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(str);
            var htmlNodeList = doc.DocumentNode.SelectNodes("//table");

            var t = doc.DocumentNode.SelectNodes("//div[@class='home-lottery']");

            var xyDoc = new HtmlDocument();
            xyDoc.LoadHtml(htmlNodeList[0].InnerHtml);

            foreach (HtmlNode item in xyDoc.DocumentNode.SelectNodes("//tbody//tr"))
            {
                var lotNo = item.ChildNodes[0].InnerText;
                var lotNumber = item.ChildNodes[1].InnerText;
                var lotOpenTime = item.ChildNodes[3].InnerText;
                var lotOpenResultNo = item.ChildNodes[5].InnerText;
            }

            //class="table-responsive"
            var nodeList = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']/table[@class='table table-striped b-light']");//双斜杠“//”表示从跟节点开始查找

            foreach (var node in htmlNodeList)
            {
                var rowNode = node.SelectNodes("//tr");
            }

            foreach (HtmlNode table in htmlNodeList)
            {
                foreach (HtmlNode row in table.SelectNodes("//tr"))
                {
                    var rowStr = row.InnerHtml;
                    var rowTextStr = row.InnerText;
                    var lotNo = row.ChildNodes[1].InnerText;
                    var lotOpenTime = row.ChildNodes[3].InnerText;
                    var lotResultNo = row.ChildNodes[5].InnerText;
                }
            }

            var rsult = "";

            return Content(rsult);
        }

        /// <summary>
        ///  测试获取XGC数据
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet, Route("GetXGCHtmlStr"),]
        public async Task<IActionResult> GetXGCHtmlStr(string url)
        {
            var headStr = HttpContext.Request.Headers["Authorization"];

            var list = new List<object>();

            var rsult = "";
            var str = await HttpHelper.GetStr(url);
            if (!str.IsNullOrEmpty())
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(str);
                var nodeInfo = doc.GetElementbyId("xgc");

                #region   nodeInfo.SelectNodes("//label").Count;

                var labelHtmlNodeList = nodeInfo.SelectNodes("//label");

                foreach (var item in labelHtmlNodeList)
                {
                    var indexNum = labelHtmlNodeList.IndexOf(item);

                    var lotArry = item.InnerText.Split(' ');
                    var lotNumber = lotArry[3].Replace("期", "");
                    var lotOpenTime = lotArry[0];

                    var openChilNode = nodeInfo.SelectNodes("//div[@class='code-view']/div[@class='gactm']")[indexNum];
                    #region   开奖号

                    var lotOpenNo = "";
                    var openNoList = new List<string>();

                    var cellInnerHtml = openChilNode.InnerHtml;

                    var oneNo = openChilNode.ChildNodes[0].InnerText;
                    var towNo = openChilNode.ChildNodes[1].InnerText;
                    var threeNo = openChilNode.ChildNodes[2].InnerText;
                    var fourNo = openChilNode.ChildNodes[3].InnerText;
                    var fiveNo = openChilNode.ChildNodes[4].InnerText;
                    var sixNo = openChilNode.ChildNodes[5].InnerText;
                    //var sevenNo = openChilNode.ChildNodes[6].InnerText;
                    var sevenNo = openChilNode.ChildNodes[7].InnerText;

                    openNoList.Add(oneNo);
                    openNoList.Add(towNo);
                    openNoList.Add(threeNo);
                    openNoList.Add(fourNo);
                    openNoList.Add(fiveNo);
                    openNoList.Add(sixNo);
                    openNoList.Add(oneNo);
                    openNoList.Add(sevenNo);

                    lotOpenNo = string.Join('|', openNoList);   //开奖号码

                    #endregion


                    //生肖-五行
                    var wdNode = nodeInfo.SelectNodes("//div[@class='div_wd']")[indexNum];

                    #region
                    var openSXList = new List<string>();
                    var openWXList = new List<string>();

                    var lotSXOpenNo = "";
                    var lotWXOpenNo = "";

                    var sxInnerHtml = wdNode.InnerHtml;

                    var oneSXNo = wdNode.ChildNodes[0].InnerText.Split('/');
                    var towSXNo = wdNode.ChildNodes[1].InnerText.Split('/');
                    var threeSXNo = wdNode.ChildNodes[2].InnerText.Split('/');
                    var fourSXNo = wdNode.ChildNodes[3].InnerText.Split('/');
                    var fiveSXNo = wdNode.ChildNodes[4].InnerText.Split('/');
                    var sixSXNo = wdNode.ChildNodes[5].InnerText.Split('/');
                    var sevenSXNo = wdNode.ChildNodes[7].InnerText.Split('/');

                    openSXList.Add(oneSXNo[0]);
                    openSXList.Add(towSXNo[0]);
                    openSXList.Add(threeSXNo[0]);
                    openSXList.Add(fourSXNo[0]);
                    openSXList.Add(fiveSXNo[0]);
                    openSXList.Add(sixSXNo[0]);
                    openSXList.Add(sevenSXNo[0]);

                    openWXList.Add(oneSXNo[1]);
                    openWXList.Add(towSXNo[1]);
                    openWXList.Add(threeSXNo[1]);
                    openWXList.Add(fourSXNo[1]);
                    openWXList.Add(fiveSXNo[1]);
                    openWXList.Add(sixSXNo[1]);
                    openWXList.Add(sevenSXNo[1]);


                    lotSXOpenNo = string.Join('|', openSXList);   //生肖
                    lotWXOpenNo = string.Join('|', openWXList);   //五行

                    #endregion

                    list.Add(new { LotNo = lotNumber, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo, LotSXNo = lotSXOpenNo, LotWXNo = lotWXOpenNo });
                }


                #endregion

                foreach (HtmlNode row in nodeInfo.SelectNodes("div | label"))
                {
                    var rowInnerHtml = row.InnerHtml;
                    var indexNum = nodeInfo.SelectNodes("label").IndexOf(row);

                    var codestr = row.SelectNodes("//div[@class='code-view']")[indexNum].InnerHtml;

                    var label = row.EndNode.Name;
                    if (label != "label")
                    {
                        continue;
                    }

                    var rowTestStr = row.InnerText;

                    var lotArry = row.InnerText.Split(' ');

                    var lotNumber = lotArry[3].Replace("期", "");
                    var lotOpenTime = lotArry[0];

                    //开奖号
                    var openChilNode = row.SelectNodes("//div[@class='code-view']/div[@class='gactm']")[indexNum];

                    #region   开奖号

                    var lotOpenNo = "";
                    var openNoList = new List<string>();

                    var cellInnerHtml = openChilNode.InnerHtml;

                    var oneNo = openChilNode.ChildNodes[0].InnerText;
                    var towNo = openChilNode.ChildNodes[1].InnerText;
                    var threeNo = openChilNode.ChildNodes[2].InnerText;
                    var fourNo = openChilNode.ChildNodes[3].InnerText;
                    var fiveNo = openChilNode.ChildNodes[4].InnerText;
                    var sixNo = openChilNode.ChildNodes[5].InnerText;
                    //var sevenNo = openChilNode.ChildNodes[6].InnerText;
                    var sevenNo = openChilNode.ChildNodes[7].InnerText;

                    openNoList.Add(oneNo);
                    openNoList.Add(towNo);
                    openNoList.Add(threeNo);
                    openNoList.Add(fourNo);
                    openNoList.Add(fiveNo);
                    openNoList.Add(sixNo);
                    openNoList.Add(oneNo);
                    openNoList.Add(sevenNo);

                    lotOpenNo = string.Join('|', openNoList);   //开奖号码

                    #endregion

                    //生肖
                    var wdNode = row.SelectNodes("//div[@class='div_wd']")[indexNum];

                    #region
                    var openSXList = new List<string>();
                    var openWXList = new List<string>();

                    var lotSXOpenNo = "";
                    var lotWXOpenNo = "";

                    var sxInnerHtml = wdNode.InnerHtml;

                    var oneSXNo = wdNode.ChildNodes[0].InnerText.Split('/');
                    var towSXNo = wdNode.ChildNodes[1].InnerText.Split('/');
                    var threeSXNo = wdNode.ChildNodes[2].InnerText.Split('/');
                    var fourSXNo = wdNode.ChildNodes[3].InnerText.Split('/');
                    var fiveSXNo = wdNode.ChildNodes[4].InnerText.Split('/');
                    var sixSXNo = wdNode.ChildNodes[5].InnerText.Split('/');
                    var sevenSXNo = wdNode.ChildNodes[7].InnerText.Split('/');

                    openSXList.Add(oneSXNo[0]);
                    openSXList.Add(towSXNo[0]);
                    openSXList.Add(threeSXNo[0]);
                    openSXList.Add(fourSXNo[0]);
                    openSXList.Add(fiveSXNo[0]);
                    openSXList.Add(sixSXNo[0]);
                    openSXList.Add(sevenSXNo[0]);

                    openWXList.Add(oneSXNo[1]);
                    openWXList.Add(towSXNo[1]);
                    openWXList.Add(threeSXNo[1]);
                    openWXList.Add(fourSXNo[1]);
                    openWXList.Add(fiveSXNo[1]);
                    openWXList.Add(sixSXNo[1]);
                    openWXList.Add(sevenSXNo[1]);


                    lotSXOpenNo = string.Join('|', openSXList);   //生肖
                    lotWXOpenNo = string.Join('|', openWXList);   //五行

                    #endregion

                    list.Add(new { LotNo = lotNumber, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo, LotSXNo = lotSXOpenNo, LotWXNo = lotWXOpenNo });
                }
            }

            rsult = string.Join(',', list);
            return Content(rsult);
        }

        /// <summary>
        ///  获取加拿大28(dake28.com) 
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet, Route("GetDaKeHtmlStr"),]
        public async Task<IActionResult> GetDaKeHtmlStr(string url)
        {
            var headStr = HttpContext.Request.Headers["Authorization"];

            var str = await HttpHelper.GetStr(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(str);
            var htmlNodeList = doc.DocumentNode.SelectNodes("//tbody");

            var t = doc.DocumentNode.SelectNodes("//div[@class='home-lottery']");

            var lotOpenResultList = doc.DocumentNode.SelectNodes("//tbody//tr");

            foreach (HtmlNode item in lotOpenResultList)
            {
                var indexNum = lotOpenResultList.IndexOf(item);
                if (indexNum > 1)
                {
                    //var lotNo = item.ChildNodes[0].InnerText;
                    var lotNumber = item.ChildNodes[1].InnerText;
                    var lotOpenTime = item.ChildNodes[3].InnerText;
                    var lotOpenResultNo = item.ChildNodes[5].InnerText;
                }
            }
            //class="table-responsive"
            var nodeList = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']/table[@class='table table-striped b-light']");//双斜杠“//”表示从跟节点开始查找

            foreach (var node in htmlNodeList)
            {
                var rowNode = node.SelectNodes("//tr");
            }

            foreach (HtmlNode table in htmlNodeList)
            {
                foreach (HtmlNode row in table.SelectNodes("//tr"))
                {
                    var rowStr = row.InnerHtml;
                    var rowTextStr = row.InnerText;
                    var lotNo = row.ChildNodes[1].InnerText;
                    var lotOpenTime = row.ChildNodes[3].InnerText;
                    var lotResultNo = row.ChildNodes[5].InnerText;
                }
            }
            var rsult = "";

            return Content(rsult);
        }

        /// <summary>
        ///  获取加拿大28(niubi28) 
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet, Route("GetNiuBiHtmlStr"),]
        public async Task<IActionResult> GetNiuBiHtmlStr(string url)
        {

            var headStr = HttpContext.Request.Headers["Authorization"];

            var str = await HttpHelper.GetStr(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(str);
            var htmlNodeList = doc.DocumentNode.SelectNodes("//tbody")[0].SelectNodes("//tr");

            foreach (HtmlNode item in htmlNodeList)
            {
                var indexNum = htmlNodeList.IndexOf(item);
                if (indexNum > 0)
                {
                    var lotNo = item.ChildNodes[1].InnerText;
                    var lotOpenTime = item.ChildNodes[3].InnerText;
                    var lotOpenNo = item.ChildNodes[5].InnerText;
                }
            }

            var t = doc.DocumentNode.SelectNodes("//div[@class='home-lottery']");

            var lotOpenResultList = doc.DocumentNode.SelectNodes("//tbody//tr");

            foreach (HtmlNode item in lotOpenResultList)
            {
                var indexNum = lotOpenResultList.IndexOf(item);
                if (indexNum > 1)
                {
                    //var lotNo = item.ChildNodes[0].InnerText;
                    var lotNumber = item.ChildNodes[1].InnerText;
                    var lotOpenTime = item.ChildNodes[3].InnerText;
                    var lotOpenResultNo = item.ChildNodes[5].InnerText;
                }
            }
            //class="table-responsive"
            var nodeList = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']/table[@class='table table-striped b-light']");//双斜杠“//”表示从跟节点开始查找

            var rsult = "";

            return Content(rsult);
        }


        /// <summary>
        ///  测试获取数据
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("TestStr")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> TestStr(string url)
        {
            var html = await HttpHelper.GetStr(url);

            var reStr = "(?s)<div[^>]*?class=\"home-lottery\"[^>]*?>(.*?)</div>";

            var reg = new Regex(reStr, RegexOptions.IgnoreCase);
            Match _match = reg.Match(html);
            if (_match.Success)
            {
                var getStr = _match.Value;
            }
            var uniStr = "(?s)<uni-view[^>]*?data-v-05ec63b6=\"\"[^>]*?>(.*?)</uni-view>";
            var ureg = new Regex(uniStr, RegexOptions.IgnoreCase);
            Match _umatch = ureg.Match(html);
            if (_umatch.Success)
            {
                var getStr = _umatch.Value;
            }

            MatchCollection mc = Regex.Matches(html, @"<uni-view data-v-05ec63b6=\"".*?>(.*?)(\w+)</uni-view>");
            foreach (Match m in mc)
            {
                //测试结果 错误原因 系统繁忙
                var getData = m.Groups[2].Value;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode oneNodeInfo = doc.GetElementbyId("tb1");

            var resultStr = GetOneNode(oneNodeInfo);

            return Content(resultStr);
        }

        /// <summary>
        ///  测试获取开彩网数据
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet, Route("GetKaiCai"),]
        public async Task<IActionResult> GetKaiCai(string url)
        {
            var headStr = HttpContext.Request.Headers["Authorization"];
            var rsult = "";
            var str = await HttpHelper.GetStr(url);
            if (!str.IsNullOrEmpty())
            {
                var curTime = DateTime.Now.Year.ToString().Substring(0, 2);
                var lotData = str.ToObject<KaiCaiInfo>();
                if (lotData.code == 0)
                {
                    var xgcOpenList = lotData.data;
                    if (xgcOpenList.Any())
                    {
                        //2022050
                        foreach (var item in xgcOpenList)
                        {
                            var openNo = item.issue;
                            var newOpen = $"{curTime}{openNo}";
                        }
                    }
                }
            }

            return Content(rsult);
        }

        /// <summary>
        /// ID
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<ContentResult> GetId()
        {
            var Id = IdHelper.GetId();

            var startTime = DateTime.Now.ToString("yyyy-MM-dd");
            startTime = $"{startTime} 20:00";
            var stime = Convert.ToDateTime(startTime);

            var endTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            startTime = $"{endTime} 19:00";
            var etime = Convert.ToDateTime(startTime);

            var num = 0;
            var endNum = 0;
            var isGo = true;
            while (isGo)
            {
                stime = stime.AddSeconds(210);
                num += 1;
                if (stime >= etime)
                {
                    isGo = false;
                    endNum = num;
                }
            }
            return await Task.FromResult(new ContentResult { Content = $"Id:{Id} -- num:{endNum}", ContentType = "application/json" });
        }

        /// <summary>
        /// GetOpenTime
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<ContentResult> GetOpenTime()
        {
            var startTime = DateTime.Now.ToString("yyyy-MM-dd");
            startTime = $"{startTime} 20:00";
            var stime = Convert.ToDateTime(startTime);

            var endTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            startTime = $"{endTime} 19:00";
            var etime = Convert.ToDateTime(startTime);

            var list = new List<string>();
            var num = 0;
            var endNum = 0;
            var isGo = true;
            while (isGo)
            {
                stime = stime.AddSeconds(210).AddSeconds(60);

                list.Add(stime.ToString("yyyy-MM-dd HH:mm:ss"));
                num += 1;
                if (stime >= etime)
                {
                    isGo = false;
                    endNum = num;
                }
            }

            var str = string.Join(",", list);
            return await Task.FromResult(new ContentResult { Content = $"-- str:{str}", ContentType = "application/json" });

        }

        /// <summary>
        ///  测试获取数据http://www.zizi28.com/#
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetZiZiHtmlStr"),]
        public async Task<IActionResult> GetZiZiHtmlStr(string url)
        {
            var headStr = HttpContext.Request.Headers["Authorization"];

            var byteStr = await HttpHelper.GetDataAsync(url);

            var rsult = "";
            var str = await HttpHelper.GetStr(url);
            if (!str.IsNullOrEmpty())
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(str);
                var nodeInfo = doc.GetElementbyId("item1");
                var htmlNodeList = nodeInfo.SelectNodes("//div[@id='item1']//table//tr");
                foreach (var item in htmlNodeList)
                {
                    var indexNum = htmlNodeList.IndexOf(item);
                    if (indexNum > 0)
                    {
                        var innerHtml = item.InnerHtml;
                        var lotNo = item.ChildNodes[1].InnerHtml;
                        var lotOpenTime = item.ChildNodes[3].InnerHtml;
                        var lotOpenNo = item.ChildNodes[5].InnerHtml;
                    }
                }

                var list = HtmlHelper.GetLotterNodeData(nodeInfo, 1);
                foreach (var item in list)
                {
                    //{ LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo }

                    var indexNum = list.IndexOf(item);
                    if (indexNum > 0)
                    {
                        var LotNo = item.GetPropertyValue("LotNo");
                        var LotOpenTime = item.GetPropertyValue("LotOpenTime");
                        var LotOpenNo = item.GetPropertyValue("LotOpenNo");
                    }
                }
                rsult = string.Join(',', list);
            }

            return Content(rsult);
        }

        /// <summary>
        ///  测试获取数据  http://wb28.vip/
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetWeb28HtmlStr"),]
        public async Task<IActionResult> GetWeb28HtmlStr(string url)
        {
            var headStr = HttpContext.Request.Headers["Authorization"];
            var rsult = "";
            var str = await HttpHelper.GetStr(url);
            if (!str.IsNullOrEmpty())
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(str);
                var nodeInfo = doc.GetElementbyId("item1");
                var htmlNodeList = nodeInfo.SelectNodes("//div[@id='item1']//table//tr");
                foreach (var item in htmlNodeList)
                {
                    var indexNum = htmlNodeList.IndexOf(item);
                    if (indexNum > 0)
                    {
                        var innerHtml = item.InnerHtml;
                        var lotNo = item.ChildNodes[1].InnerHtml;
                        var lotOpenTime = item.ChildNodes[3].InnerHtml;
                        var lotOpenNo = item.ChildNodes[5].InnerHtml;
                    }
                }

                var list = HtmlHelper.GetLotterNodeData(nodeInfo, 1);
                foreach (var item in list)
                {
                    //{ LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo }

                    var indexNum = list.IndexOf(item);
                    if (indexNum > 0)
                    {
                        var LotNo = item.GetPropertyValue("LotNo");
                        var LotOpenTime = item.GetPropertyValue("LotOpenTime");
                        var LotOpenNo = item.GetPropertyValue("LotOpenNo");
                    }
                }
                rsult = string.Join(',', list);
            }

            return Content(rsult);
        }


        /// <summary>
        ///  测试获取数据     https://yuce588.com/#/
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetYuCe28HtmlStr"),]
        public async Task<IActionResult> GetYuCe28HtmlStr(string url)
        {
            var headStr = HttpContext.Request.Headers["Authorization"];

            var rsult = "";
            var list = new List<object>();

            var cowsList = new List<object>();

            var baccaratList = new List<object>();

            var dic = new Dictionary<string, string>();
            dic.Add("referer", url);
            dic.Add("authority", url);
            dic.Add("accept", "application/json");
            dic.Add("accept-encoding", "gzip, deflate");

            var str = HttpHelper.HttpPost(url, dic);
            if (!str.IsNullOrEmpty())
            {
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
                                var lotOpenTime = (Convert.ToString(openTime).ConvertStringToDateTime()).ToString("HH:mm:ss");
                                var lotOpenNo = "";
                                var middleCodeArray = middleCode.Split(',');
                                var one = middleCodeArray[0];
                                var two = middleCodeArray[1];
                                var three = middleCodeArray[2];

                                lotOpenNo = $"{middleCodeArray[0]} + {middleCodeArray[1]} + {middleCodeArray[2]} = {openNum}";

                                list.Add(new { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo });

                                var oneNo = ConvertHelper.GetCowsResultNo(Convert.ToInt32(middleCodeArray[0]));
                                var twoNo = ConvertHelper.GetCowsResultNo(Convert.ToInt32(middleCodeArray[1]));
                                var threeNo = ConvertHelper.GetCowsResultNo(Convert.ToInt32(middleCodeArray[2]));

                                cowsList.Add(new { LotNo = lotNo, OneNo = oneNo, TwoNo = twoNo, ThreeNo = threeNo });

                                var lotPretend = ConvertHelper.GetBaccaratZXD(middleCodeArray);
                                var lotOutCome = ConvertHelper.GetBaccaratResult(middleCodeArray);

                                baccaratList.Add(new { LotNo = lotNo, LotOpenNo = lotOpenNo, LotPretend = lotPretend, LotOutCome = lotOutCome });
                            }
                        }
                    }
                }

                //rsult = string.Join(',', list);

                //cows
                // rsult = string.Join(',', cowsList);

                rsult = string.Join(',', baccaratList);
            }

            return await Task.FromResult(Content(rsult));
        }

        /// <summary>
        /// GetTestStrInfo
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetTestStrInfo"),]
        public async Task<IActionResult> GetTestStrInfo()
        {
            var str = "0+7+5=12 ";
            var array = str.Split('+');  //9 + 7 + 5 = 21
            var sumStr = str.Split('=');
            var newStr = array.Length > 0 ? $"{array[0]} + {array[1]} + {array[2].Split('=')[0]} = {sumStr[1]}" : "";
            return await Task.FromResult(new ContentResult { Content = newStr, ContentType = "application/json", StatusCode = 200 });
        }

        private string GetOneNode(HtmlNode node)
        {
            var result = "";
            if (node != null)
            {
                var tabNodes = node.SelectNodes("//uni-view");
                var tNodes = node.SelectNodes("//*/uni-view[@class='u-table']");
                var trCount = node.SelectNodes("//*/uni-view[@class='u-tr']");

                foreach (HtmlNode row in node.SelectNodes("//*/uni-view[@class='u-table']"))
                {
                    foreach (HtmlNode cell in row.SelectNodes("//*/uni-view[@class='u-tr']"))
                    {
                        var oneChildStr = cell.ChildNodes[0].InnerText;
                        var twoChildStr = cell.ChildNodes[1].InnerText;
                        var threeChildStr = cell.ChildNodes[2].InnerText;
                    }
                }

                var list = new List<object>();

                for (int i = 0; i < node.SelectNodes("//*/uni-view[@class='u-table']").Count; i++)
                {
                    var tbRow = node.SelectNodes("//*/uni-view[@class='u-table']")[i];
                    for (int j = 0; j < tbRow.SelectNodes("//*/uni-view[@class='u-tr']").Count; j++)
                    {
                        var trRow = tbRow.SelectNodes("//*/uni-view[@class='u-tr']")[j];
                        var oneStr = trRow.ChildNodes[0].InnerText;
                        var twoStr = trRow.ChildNodes[1].InnerText;
                        var threeStr = trRow.ChildNodes[2].InnerText;

                        list.Add(new { LotNo = oneStr, LotOpenTime = twoStr, LotResultNo = threeStr });
                    }
                }

                result = string.Join(',', list);
            }

            return result;
        }

        private void GetOneNode_Old(HtmlNode node)
        {
            if (node != null)
            {
                var tabNodes = node.SelectNodes("//uni-view");
                var tNodes = node.SelectNodes("//*/uni-view[@class='u-table']");
                var trCount = node.SelectNodes("//*/uni-view[@class='u-tr']");

                foreach (HtmlNode row in node.SelectNodes("//*/uni-view[@class='u-table']"))
                {
                    foreach (HtmlNode cell in row.SelectNodes("//*/uni-view[@class='u-tr']"))
                    {

                        var getStr = cell.InnerText;

                        if (cell.ChildNodes.Count > 0)
                        {
                            foreach (var item in cell.SelectNodes("//*/uni-view[@class='u-td']"))
                            {
                                var nodeInfo = item;
                            }
                        }
                    }
                }
            }
        }

    }


}
