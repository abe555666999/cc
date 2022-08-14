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
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Telegram.Bot.Types.Enums;
using System.ServiceModel.Channels;
using Telegram.Bot.Types;

namespace Coldairarrow.Api.Controllers.Tele
{
    /// <summary>
    /// Test
    /// </summary>
    [Route("/TeleManager/[controller]/[action]")]
    [OpenApiTag("Tele")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class TestTledController : BaseApiController
    {

        readonly TelegramBotClient _bolClient;

        readonly TelegramBotClient _twoClient;

        
        private object _lock = new object();
        public TestTledController()
        {
            if (_bolClient == null)
            {
                lock (_lock)
                {
                    if (_bolClient == null)
                    {
                        _bolClient = new TelegramBotClient("5364544448:AAH-Fpmz6ltrnBUsHzNDUKbVuKk__tK9Bik");
                    }

                    if (_twoClient == null)
                    {
                        _twoClient = new TelegramBotClient("5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk");
                    }
                }
            }
        }


        /// <summary>
        ///  测试获取数据
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetTestStr"),]
        public async Task<IActionResult> GetTestStr(string url)
        {

            var Client = new TelegramBotClient("5364544448:AAH-Fpmz6ltrnBUsHzNDUKbVuKk__tK9Bik");

            Client.GetUpdatesAsync().Wait();

            var resStr = "";
            var data = await Client.GetUpdatesAsync();
            if (data.Any())
            {
                resStr = string.Join(',', data.ToList());
            }


            var isSendInfo = await Client.SendTextMessageAsync("-657345271", "hello world");

            //  var byteStr = await HttpHelper.GetDataAsync(url);

            var rsult = "";
            //var str = await HttpHelper.GetStr(url);

            return Content(rsult);
        }


        /// <summary>
        ///  测试发送消息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetTestStr"),]
        public async Task<IActionResult> TestSendMsg(string content)
        {
            var rsult = "";
            var data = await _bolClient.GetUpdatesAsync();

            //获取信息
            var myInfo = await _bolClient.GetMeAsync();

            var copeMessage = await _bolClient.GetUpdatesAsync();


            var upDate = await _bolClient.GetUpdatesAsync();



            var userInfo = await _bolClient.GetUserProfilePhotosAsync(5594508941);

            // https://core.telegram.org/bots/api

            var url = "https://api.telegram.org/bot5364544448:AAH-Fpmz6ltrnBUsHzNDUKbVuKk__tK9Bik/getMe";
            var strInfo = await HttpHelper.GetStr(url);

            var getInfo = await _bolClient.GetChatMemberAsync("-657345271", 5364544448);


            var str = "";

            var resStr = "";
            if (data.Any())
            {
                resStr = string.Join(',', data.ToList());
            }

            // var isSendInfo = await _bolClient.SendTextMessageAsync("-657345271", content);


            // var getMemberInfo = await _bolClient.GetChatMemberAsync("657345271", 5594508941);

            if (!content.IsNullOrEmpty())
            {
                var sendInfo = await _bolClient.SendTextMessageAsync("-657345271", content);
            }

            #region msg

            var chatid = "-657345271";
            var chat = _bolClient.GetChatAsync(chatid).Result;

            //  var editMsg = await _bolClient.EditMessageTextAsync(chatid, 6, "测试数据6667777");

            var exportData = await _bolClient.ExportChatInviteLinkAsync(chatid, System.Threading.CancellationToken.None);

            var pinnedmsgtxt = chat.PinnedMessage.Text;

            var pinnedmsg = chat.PinnedMessage;

            #endregion

            return Content(rsult);
        }


        /// <summary>
        ///  TestInfo
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("TestInfo"),]
        public async Task<IActionResult> TestInfo()
        {
            var chatid = "-657345271";


            var tt = await _bolClient.GetChatAsync(chatid);




            #region
            var Keyboard =
    new KeyboardButton[][]
    {
        new KeyboardButton[]
        {
            new KeyboardButton("测试按钮1"),
            new KeyboardButton("测试按钮二")
        }
    };

            //  await _bolClient.SendTextMessageAsync(chatid, "测试信息123456",null,null,false,false,false,null,null, rkm);


            var none = new InlineKeyboardButton("测试按钮1");
            var two = new InlineKeyboardButton("测试按钮12");


            InlineKeyboardMarkup menu;

            menu = new InlineKeyboardMarkup(new[] { none, two });



            var linArray = new[] { new InlineKeyboardButton("按钮1") };
            var ReplyMarkup = new InlineKeyboardMarkup(linArray);



            string message = "message";
            string message1 = "message1";
            string botid = "5364544448:AAH-Fpmz6ltrnBUsHzNDUKbVuKk__tK9Bik";
            // string chatid = "38651047";

            // Sender.send("", "https://api.telegram.org/bot" + botid + "/sendmessage?chat_id=" + chatid + "&text=" + message + "&reply_markup={\"keyboard\":[[\"1\"],[\"2\"]],\"resize_keyboard\":\"True\",\"one_time_keyboard\":\"True\"}");

            var ddddd = HttpHelper.PostFromQueryToString("https://api.telegram.org/bot" + botid + "/sendmessage?chat_id=" + chatid + "&text=" + message1 + "&reply_markup={\"keyboard\":[[\"1\"],[\"2\"]],\"resize_keyboard\":\"True\",\"one_time_keyboard\":\"True\"}", "");

            #endregion

            //var setWebhookAsync = await _bolClient.SetWebhookAsync("",null,"","");

            //await _bolClient.AnswerCallbackQueryAsync("777152220", "666999",true);



            //await _bolClient.GetChatAdministratorsAsync(chatid);

            //  var exportData = await _bolClient.ExportChatInviteLinkAsync(chatid, System.Threading.CancellationToken.None);





            var ard =
                new string[][]
                {
        new string[] {"1-1", "1-2"},
        new string[] {"2"},
        new string[] {"3-1", "3-2" , "3-3" }
                };

            var rmucc = new ReplyKeyboardMarkup(Keyboard);

            // var gerMsgInfo = await _bolClient.SendTextMessageAsync(chatid, "测试信息123456", null, null, false, false, false, null, null, rkm);



            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            for (var Index = 1; Index < 17; Index++)
            {
                cols.Add(new KeyboardButton("" + Index));
                if (Index % 2 != 0) continue;
                rows.Add(cols.ToArray());
                cols = new List<KeyboardButton>();
            }
            var kkkkeyboard = rows.ToArray();

            var rkmcccccc = new ReplyKeyboardMarkup(kkkkeyboard);

            await _bolClient.SendTextMessageAsync(
                chatid,
                "Choose",
                replyMarkup: rkmcccccc);


            return Content("");
        }

        /// <summary>
        ///  TestInfo
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("TestSetLineInfo"),]
        public async Task<IActionResult> TestSetLineInfo(string msgStr)
        {
            var chatid = "-657345271";


            var tt = await _bolClient.GetChatAsync(chatid);

            InlineKeyboardButton urlButton = new InlineKeyboardButton("GoURL1");
            InlineKeyboardButton urlButton2 = new InlineKeyboardButton("GoURL2");
            InlineKeyboardButton urlButton3 = new InlineKeyboardButton("GoURL3");
            InlineKeyboardButton urlButton4 = new InlineKeyboardButton("GoURL4");

            urlButton.Text = "GoURL1";
            urlButton.Url = "https://www.google.com/";

            urlButton2.Text = "GoURL2";
            urlButton2.Url = "https://www.bing.com/";

            urlButton3.Text = "GoURL3";
            urlButton3.Url = "https://www.duckduckgo.com/";

            urlButton4.Text = "GoURL4";
            urlButton4.Url = "https://stackoverflow.com/";

            // Rows, every row is InlineKeyboardButton[], You can put multiple buttons!
            InlineKeyboardButton[] row1 = new InlineKeyboardButton[] { urlButton };
            InlineKeyboardButton[] row2 = new InlineKeyboardButton[] { urlButton2, urlButton3 };
            InlineKeyboardButton[] row3 = new InlineKeyboardButton[] { urlButton4 };


            // Buttons by rows
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[][] { row1, row2, row3 };

            // Keyboard
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

            // Send Message
            var result = await _bolClient.SendTextMessageAsync(chatid, "Message", replyMarkup: keyboard);

            if (!msgStr.IsNullOrEmpty())
            {
                var testMsg = await _bolClient.SendTextMessageAsync(chatid, msgStr);
            }



            return Content("");
        }


        #region TWO

        /// <summary>
        ///  测试发送消息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("TestSendGroupMsg"),]
        public async Task<IActionResult> TestSendGroupMsg(string content)
        {
            var rsult = "";

            var chartId = "-1001598235943";

            //获取信息
            var myInfo = await _twoClient.GetMeAsync();

            // await _twoClient.GetChatAsync();

            var data = await _twoClient.GetUpdatesAsync();

            var upDate = await _twoClient.GetUpdatesAsync();

            var userInfo = await _twoClient.GetUserProfilePhotosAsync(5594508941);

            // https://core.telegram.org/bots/api

            //获取用户信息
            var url = "https://api.telegram.org/bot5364544448:AAH-Fpmz6ltrnBUsHzNDUKbVuKk__tK9Bik/getMe";
            var strInfo = await HttpHelper.GetStr(url);

            //获取组信息


            //  var getInfo = await _twoClient.GetChatMemberAsync(chartId, 5364544448);

            var str = "";

            var resStr = "";
            if (data.Any())
            {
                resStr = string.Join(',', data.ToList());
            }


            if (!content.IsNullOrEmpty())
            {
                var sendInfo = await _twoClient.SendTextMessageAsync(chartId, content);
            }

            #region msg

            var chat = _twoClient.GetChatAsync(chartId).Result;

            //  var editMsg = await _bolClient.EditMessageTextAsync(chatid, 6, "测试数据6667777");

            var exportData = await _twoClient.ExportChatInviteLinkAsync(chartId, System.Threading.CancellationToken.None);

            var pinnedmsgtxt = chat.PinnedMessage?.Text ?? "";


            #endregion

            return Content(rsult);
        }

        /// <summary>
        ///  TestAddLineInfo
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("TestAddLineInfo"),]
        public async Task<IActionResult> TestAddLineInfo()
        {
            var chartId = "-1001598235943";

            #region

            //  var re = await _twoClient.SendTextMessageAsync("-1001598235943", "Choose",replyMarkup: keyboard);
            // https://api.telegram.org/bot5364544448:AAH-Fpmz6ltrnBUsHzNDUKbVuKk__tK9Bik/sendMessage?chat_id=abe555666&text=mysampletext6666

            //var sendMsgURL ="https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/sendMessage?chat_id=-1001598235943&text=mysampletext6666";
            //var msgData = await HttpHelper.GetStr(sendMsgURL);

            #endregion



            #region

            var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("1.1"),
                        new InlineKeyboardButton("1.2"),
                    },
                    new[] // second row
                    {
                        new InlineKeyboardButton("2.1"),
                        new InlineKeyboardButton("2.2"),
                    }
                });

            string keyboardStr = JsonConvert.SerializeObject(keyboard);


            StringBuilder stringBuilder = new StringBuilder("https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/");
            stringBuilder.Append("sendMessage");
            stringBuilder.Append($"?chat_id={chartId}");
            stringBuilder.Append($"&text=Choose");
            stringBuilder.Append($"&replyMarkup={keyboardStr}");

            var sendLineMsgURL = "https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/sendMessage?chat_id=-1001598235943&text=Choose";

            var lineMsgURL = stringBuilder.ToString();

            var msgData = await HttpHelper.GetStr(lineMsgURL);

            // await _bolClient.SendTextMessageAsync(replyMarkup)

            #endregion

            var result = "";

            return Content(result);
        }

        /// <summary>
        ///  AddLindBtnDataInfo
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("AddLindBtnDataInfo"),]
        public async Task<IActionResult> AddLindBtnDataInfo()
        {

            var chartId = "-1001598235943";


            // Defining buttons
            InlineKeyboardButton urlButton = new InlineKeyboardButton("GoURL1");
            InlineKeyboardButton urlButton2 = new InlineKeyboardButton("GoURL2");
            InlineKeyboardButton urlButton3 = new InlineKeyboardButton("GoURL3");
            InlineKeyboardButton urlButton4 = new InlineKeyboardButton("GoURL4");

            urlButton.Text = "GoURL1";
            urlButton.Url = "https://www.google.com/";

            urlButton2.Text = "GoURL2";
            urlButton2.Url = "https://www.bing.com/";

            urlButton3.Text = "GoURL3";
            urlButton3.Url = "https://www.duckduckgo.com/";

            urlButton4.Text = "GoURL4";
            urlButton4.Url = "https://stackoverflow.com/";

            // Rows, every row is InlineKeyboardButton[], You can put multiple buttons!
            InlineKeyboardButton[] row1 = new InlineKeyboardButton[] { urlButton };
            InlineKeyboardButton[] row2 = new InlineKeyboardButton[] { urlButton2, urlButton3 };
            InlineKeyboardButton[] row3 = new InlineKeyboardButton[] { urlButton4 };


            // Buttons by rows
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[][] { row1, row2, row3 };

            // Keyboard
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

            // Send Message  Lind
             await _twoClient.SendTextMessageAsync(chartId, "AddLineMessage", replyMarkup: keyboard);



            string keyboardStr = JsonConvert.SerializeObject(keyboard);

            var sendMsgStr = $"测试LinBtn信息:{ConvertHelper.GenerateRandomSeed()}";

            StringBuilder stringBuilder = new StringBuilder("https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/");
            stringBuilder.Append("sendMessage");
            stringBuilder.Append($"?chat_id={chartId}");
            stringBuilder.Append($"&text={sendMsgStr}");
            //  stringBuilder.Append($"&replyMarkup:{keyboardStr}");

            //var sendLineMsgURL = "https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/sendMessage?chat_id=-1001598235943&text=Choose";

            var lineMsgURL = stringBuilder.ToString();

            // var msgData = await HttpHelper.GetStr(lineMsgURL);


           // var msgData = HttpHelper.PostFromBodyToString(lineMsgURL, keyboardStr);

            var result = "";

            return Content(result);
        }

        /// <summary>
        ///  AddLindTwoBtnDataInfo
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("AddLindTwoBtnDataInfo"),]
        public async Task<IActionResult> AddLindTwoBtnDataInfo()
        {

            var chartId = "-1001598235943";

            string callbackQueryData = 'a' + new Random().Next(5_000).ToString();

            // Defining buttons
            InlineKeyboardButton urlButton = new InlineKeyboardButton("GoURL1");
            InlineKeyboardButton urlButton2 = new InlineKeyboardButton("GoURL2");
            InlineKeyboardButton urlButton3 = new InlineKeyboardButton("GoURL3");
            InlineKeyboardButton urlButton4 = new InlineKeyboardButton("GoURL4");

            //urlButton.Text = "GoURL1";
            //urlButton.Url = "https://www.google.com/";

            //urlButton2.Text = "GoURL2";
            //urlButton2.Url = "https://www.google.com/";

            //urlButton3.Text = "GoURL3";
            //urlButton3.Url = "https://www.google.com/";

            //urlButton4.Text = "GoURL4";
            //urlButton4.Url = "https://www.google.com/";


            urlButton.Text = "GoURL1";
           // urlButton.Url = "https://www.google.com/";
            urlButton.CallbackData += InlineKeyboardButton.WithCallbackData("测试1", callbackQueryData);

            urlButton2.Text = "GoURL2";
            // urlButton2.Url = "https://www.google.com/";
            urlButton2.CallbackData += InlineKeyboardButton.WithCallbackData("测试2", callbackQueryData);

            urlButton3.Text = "GoURL3";
            // urlButton3.Url = "https://www.google.com/";
            urlButton3.CallbackData += InlineKeyboardButton.WithCallbackData("测试3", callbackQueryData);

            urlButton4.Text = "GoURL4";
            //  urlButton4.Url = "https://www.google.com/";
            urlButton4.CallbackData += InlineKeyboardButton.WithCallbackData("测试4", callbackQueryData);


            // Rows, every row is InlineKeyboardButton[], You can put multiple buttons!
            InlineKeyboardButton[] row1 = new InlineKeyboardButton[] { urlButton };
            InlineKeyboardButton[] row2 = new InlineKeyboardButton[] { urlButton2, urlButton3 };
            InlineKeyboardButton[] row3 = new InlineKeyboardButton[] { urlButton4 };


            // Buttons by rows
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[][] { row1, row2, row3 };

            // Keyboard
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

            // Send Message  Lind
            await _twoClient.SendTextMessageAsync(chartId, "AddLineMessage", replyMarkup: keyboard);



            string keyboardStr = JsonConvert.SerializeObject(keyboard);

            var sendMsgStr = $"测试LinBtn信息:{ConvertHelper.GenerateRandomSeed()}";

            StringBuilder stringBuilder = new StringBuilder("https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/");
            stringBuilder.Append("sendMessage");
            stringBuilder.Append($"?chat_id={chartId}");
            stringBuilder.Append($"&text={sendMsgStr}");
            //  stringBuilder.Append($"&replyMarkup:{keyboardStr}");

            //var sendLineMsgURL = "https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/sendMessage?chat_id=-1001598235943&text=Choose";

            var lineMsgURL = stringBuilder.ToString();

            // var msgData = await HttpHelper.GetStr(lineMsgURL);


            // var msgData = HttpHelper.PostFromBodyToString(lineMsgURL, keyboardStr);

            var result = "";

            return Content(result);
        }

        /// <summary>
        ///  测试获取信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("TestGetDataInfo"),]
        public async Task<IActionResult> TestGetDataInfo()
        {
            //获取用户信息
            var meUrl = "https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/getMe";
            var meInfo = await HttpHelper.GetStr(meUrl);

            //group
            var groupUrl = "https://api.telegram.org/bot5564922207:AAE1CBVAkchdvTzd_Aht3gVutCG5Ded1NMk/getUpdates";
            var getGroupInfo = await HttpHelper.GetStr(groupUrl);

            await Should_Answer_With_Notification();

            var result = "";
            result = getGroupInfo;

            return Content(result);
        }



        #endregion


        #region

        private async Task Should_Answer_With_Notification()
        {

            var _chartId = "-1001598235943";

            string callbackQueryData = 'a' + new Random().Next(5_000).ToString();

           var msgId = await _twoClient.SendTextMessageAsync(
                chatId: _chartId,
                text: "Please click on *OK* button.",
                parseMode: ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(
                    new[]{
                        InlineKeyboardButton.WithCallbackData("测试", callbackQueryData)
                    })
            );

           // Update responseUpdate = await _fixture.UpdateReceiver.GetCallbackQueryUpdateAsync(message.MessageId);
           // CallbackQuery callbackQuery = responseUpdate.CallbackQuery!;

           await _twoClient.AnswerCallbackQueryAsync(
                callbackQueryId: $"{msgId.MessageId}",
                text: "Got it!",
                showAlert: true
            );

        }

        #endregion
    }
}
