using HtmlAgilityPack;
using System.Collections.Generic;

namespace Coldairarrow.Util.Helper
{
    public class HtmlHelper
    {
        /// <summary>
        /// (tb1:加拿大28  tb2:牛牛28  tb3:百家乐28 )  string parentStr,string childStr
        /// </summary>
        /// <param name="node"></param>
        /// <param name="lotType">(1:加拿大28    2:牛牛28,   3：百家乐28)</param>
        /// <returns></returns>
        public static List<object> GetLotterNodeData(HtmlNode node, int lotType)
        {
            var list = new List<object>();

            if (node != null)
            {
                switch (lotType)
                {
                    case 1:
                        list = GetCanadaData(node);    //加拿大28
                        break;
                    case 2:
                        list = GetCowsData(node);      //牛牛28
                        break;
                    case 3:
                        list = GetBaccaratData(node);  //百家乐28
                        break;
                    default:
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// 加拿大28(http://www.dake28.com/jnd28/kj.html)
        /// </summary>
        /// <param name="htmlNodes"></param>
        /// <returns></returns>
        public static List<object> GetCanadaKadeData(HtmlNodeCollection htmlNodes)
        {
            var list = new List<object>();
            foreach (HtmlNode item in htmlNodes)
            {
                var indexNum = htmlNodes.IndexOf(item);
                if (indexNum > 1)
                {
                    var lotNo = item.ChildNodes[1].InnerText;
                    var lotOpenTime = item.ChildNodes[3].InnerText;
                    var lotOpenNo = item.ChildNodes[5].InnerText;

                    list.Add(new { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo });
                }
            }

            return list;
        }

        /// <summary>
        /// 澳洲幸运28 
        /// </summary>
        /// <param name="htmlNodes"></param>
        /// <returns></returns>
        public static List<object> GetFortunateData(HtmlNodeCollection htmlNodes)
        {
            var list = new List<object>();

            var xyDoc = new HtmlDocument();
            xyDoc.LoadHtml(htmlNodes[0].InnerHtml);

            foreach (HtmlNode item in xyDoc.DocumentNode.SelectNodes("//tbody//tr"))
            {
                var lotNo = item.ChildNodes[1].InnerText;
                var lotOpenTime = item.ChildNodes[3].InnerText;
                var lotOpenResultNo = item.ChildNodes[5].InnerText;

                list.Add(new { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenResultNo });
            }

            return list;
        }

        /// <summary>
        /// XGC
        /// </summary>
        /// <param name="nodeInfo"></param>
        /// <returns></returns>
        public static List<object> GetXGCData(HtmlNode nodeInfo)
        {
            var list = new List<object>();

            if (nodeInfo != null)
            {
                var labelHtmlNodeList = nodeInfo.SelectNodes("//label");

                foreach (var item in labelHtmlNodeList)
                {
                    var indexNum = labelHtmlNodeList.IndexOf(item);

                    var lotArry = item.InnerText.Split(' ');
                    var lotNumber = lotArry[3].Replace("期", "");
                    var lotOpenTime = lotArry[0];

                    var openChilNode = nodeInfo.SelectNodes("//div[@class='code-view']/div[@class='gactm']")[indexNum];

                    //   开奖号
                    var lotOpenNo = "";
                    var lotSpecialNo = "";
                    var openNoList = new List<string>();

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
                    openNoList.Add(sevenNo);

                    lotOpenNo = string.Join('|', openNoList);   //开奖号码
                    lotSpecialNo = sevenNo;

                    // 生肖-五行
                    var wdNode = nodeInfo.SelectNodes("//div[@class='div_wd']")[indexNum];
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

                    list.Add(new { LotNo = lotNumber, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo, LotSXNo = lotSXOpenNo, LotWXNo = lotWXOpenNo, LotSpecialNo = lotSpecialNo });
                }
            }

            return list;
        }

        #region

        /// <summary>
        /// 加拿大28(http://jb28.cc/)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List<object> GetCanadaData(HtmlNode node)
        {
            var list = new List<object>();
            #region 加拿大28
            var htmlNodeList = node.SelectNodes("//*/uni-view[@id='tb1']/uni-view[@class='u-table']/uni-view[@class='u-tr']");

            foreach (HtmlNode cell in htmlNodeList)
            {
                var indexNum = htmlNodeList.IndexOf(cell);
                if (indexNum > 0)
                {
                    var lotNo = cell.ChildNodes[0].InnerText;
                    var lotOpenTime = cell.ChildNodes[1].InnerText;
                    var lotOpenNo = cell.ChildNodes[2].InnerText;

                    list.Add(new { LotNo = lotNo, LotOpenTime = lotOpenTime, LotOpenNo = lotOpenNo });
                }
            }
            #endregion

            return list;
        }

        /// <summary>
        /// 牛牛28
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List<object> GetCowsData(HtmlNode node)
        {
            var list = new List<object>();
            #region 牛牛28
            var htmlNodeList = node.SelectNodes("//*/uni-view[@id='tb2']/uni-view[@class='u-table']/uni-view[@class='u-tr']");
            foreach (HtmlNode cell in htmlNodeList)
            {
                var indexNum = htmlNodeList.IndexOf(cell);
                if (indexNum > 0)
                {
                    var getStr = cell.InnerHtml;
                    var textStr = cell.InnerText;

                    var lotNo = cell.ChildNodes[0].InnerText;
                    var oneNo = cell.ChildNodes[1].InnerText;
                    var twoNo = cell.ChildNodes[2].InnerText;
                    var threeNo = cell.ChildNodes[3].InnerText;

                    list.Add(new { LotNo = lotNo, OneNo = oneNo, TwoNo = twoNo, ThreeNo = threeNo });
                }
            }
            #endregion
            return list;
        }

        /// <summary>
        /// 百家乐28
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List<object> GetBaccaratData(HtmlNode node)
        {
            var list = new List<object>();
            #region 百家乐28
            var htmlNodeList = node.SelectNodes("//*/uni-view[@id='tb3']/table[@class='bjltb']/tr");
            foreach (HtmlNode cell in htmlNodeList)
            {
                var indexNum = htmlNodeList.IndexOf(cell);
                if (indexNum > 0)
                {
                    var getStr = cell.InnerHtml;
                    var textStr = cell.InnerText;

                    var lotNo = cell.ChildNodes[0].InnerText;
                    var lotOpenNo = cell.ChildNodes[1].InnerText.Replace("&nbsp;", "").Replace("&nbsp", "");
                    var lotPretend = cell.ChildNodes[2].InnerText;
                    var lotOutCome = cell.ChildNodes[3].InnerText;

                    list.Add(new { LotNo = lotNo, LotOpenNo = lotOpenNo, LotPretend = lotPretend, LotOutCome = lotOutCome });
                }
            }
            #endregion
            return list;
        }

        #endregion
    }
}
