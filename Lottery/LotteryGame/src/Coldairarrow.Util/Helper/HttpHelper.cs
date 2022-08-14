using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Util.Helper
{
    public class HttpHelper
    {
        public static async Task<string> GetStr(string url)
        {
            try
            {
                var resultStr = "";
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                };

                using (var httpClient = new HttpClient(handler))
                {
                    resultStr = await httpClient.GetStringAsync(url);
                }
                return resultStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static async Task<string> GetStreamStr(string url)
        {
            try
            {
                var resultStr = "";
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                };

                using (var httpClient = new HttpClient())
                {
                    Stream stream = await httpClient.GetStreamAsync(url);
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    resultStr = reader.ReadToEnd();
                    reader.Close();
                }
                return resultStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static async Task<string> GetByteToStr(string url)
        {
            try
            {
                var resultStr = "";
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                };

                using (var httpClient = new HttpClient())
                {

                    var result = await httpClient.GetByteArrayAsync(url);
                    resultStr = Encoding.Default.GetString(result);
                }
                return resultStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 默认为 UTF-8, 根据需要可以修改为 GBK,GB2312等 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static async Task<string> GetDataAsync(string uri, string charset = "UTF-8")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    httpClient.DefaultRequestHeaders.Add("Accept",
                     "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

                    httpClient.DefaultRequestHeaders.Add("User-Agent",
                  "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");

                    var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
                    // 读取字符流
                    var result = await response.Content.ReadAsStreamAsync();
                    // 使用指定的字符编码读取字符流， 默认编码：UTF-8，其他如：GBK
                    var stream = new StreamReader(result, Encoding.GetEncoding(charset));
                    // 字符流转为字符串并返回
                    return stream.ReadToEnd();
                }
            }
            catch (HttpRequestException hre)
            {
                hre.ToString();
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string PostFromQueryToString(string url, string reqData)
        {
            string strUrl = new UriBuilder(url)
            {
                Query = reqData
            }.ToString();
            if (strUrl.StartsWith("https")) { System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls; }

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {

                    using (HttpResponseMessage response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, strUrl)).Result)
                    {
                        string resultStr = string.Empty;
                        if (response.IsSuccessStatusCode)
                        {
                            Task<string> t = response.Content.ReadAsStringAsync();
                            resultStr = t.Result;
                        }
                        response.Dispose();
                        return resultStr;
                    }
                }
                catch (System.AggregateException ex)
                {
                    return ex.Message;
                }
                finally
                {
                    httpClient.Dispose();
                }
            }
        }

        public static string PostFromBodyToString(string strUrl, string reqData)
        {
            try
            {

                if (strUrl.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                string result = string.Empty;
                using (HttpContent httpContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(reqData), System.Text.Encoding.UTF8, "application/json"))
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        try
                        {
                            using (HttpResponseMessage httpRes = httpClient.PostAsync(strUrl, httpContent).Result)
                            {
                                if (httpRes.IsSuccessStatusCode)
                                {
                                    result = httpRes.Content.ReadAsStringAsync().Result;
                                }
                                httpRes.Dispose();
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            httpClient.Dispose();
                            httpContent.Dispose();
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string PostFromFormToString(string url, Dictionary<string, string> reqData)
        {
            try
            {
                string result = string.Empty;
                using (HttpContent httpContent = new FormUrlEncodedContent(reqData))
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        try
                        {
                            using (HttpResponseMessage httpRes = httpClient.PostAsync(url, httpContent).Result)
                            {
                                if (httpRes.IsSuccessStatusCode)
                                {
                                    result = httpRes.Content.ReadAsStringAsync().Result;
                                }
                                httpRes.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            httpClient.Dispose();
                            httpContent.Dispose();
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="Url">The URL.</param>
        /// <param name="postDataStr">The post data string.</param>
        /// <param name="headName"></param>
        /// <param name="postType"></param>
        /// <param name="token"></param>
        /// <returns>System.String.</returns>
        public static string WebHttpGet(string Url, string postDataStr, string headName = null, string token = null, string postType = null)
        {
            Url = Url + ((postDataStr == "") ? "" : "?") + postDataStr;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            if (!string.IsNullOrEmpty(headName) && !string.IsNullOrEmpty(token))
            {
                request.Headers.Add(headName ?? "", token ?? "");
            }
            string contentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(postType))
            {
                switch (postType)
                {
                    case "txt":
                        contentType = "application/x-www-form-urlencoded";
                        break;
                    case "json":
                        contentType = "application/json; charset=utf-8";
                        break;
                    case "html":
                        contentType = "text/html";
                        break;
                    case "textjson":
                        contentType = "text/json";
                        break;
                }
            }
            request.ContentType = contentType;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="Url">The URL.</param>
        /// <param name="postDataStr">The post data string.</param>
        /// <param name="headName"></param>
        /// <param name="token"></param>
        /// <returns>System.String.</returns>
        public static string HttpGet(string Url, string postDataStr, string headName = null, string token = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + ((postDataStr == "") ? "" : "?") + postDataStr);
            request.Method = "GET";
            if (!string.IsNullOrEmpty(headName) && !string.IsNullOrEmpty(token))
            {
                request.Headers.Add(headName ?? "", token ?? "");
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="Url">The URL.</param>
        /// <param name="postDataStr">The post data string.</param>
        /// <param name="headDic"></param>
        /// <returns>System.String.</returns>
        public static string HttpDicGet(string Url, string postDataStr, Dictionary<string, string> headDic)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + ((postDataStr == "") ? "" : "?") + postDataStr);
            request.Method = "GET";

            if (headDic.Any())
            {
                foreach (KeyValuePair<string, string> item in headDic)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public static string HttpGet(string url, Dictionary<string, string> headDic)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);
            if (headDic.Any())
            {
                foreach (KeyValuePair<string, string> item in headDic)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = httpClient.Send(request);
            Stream stream = response.Content.ReadAsStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string value = reader.ReadToEnd();
            reader.Close();
            return value;
        }

        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36";

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受   
        }

        public static string PostHttps(string url, IDictionary<string, string> parameters, Encoding charset)
        {
            HttpWebRequest request = null;
            CookieContainer cookie = new CookieContainer();
            //HTTPSQ请求
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = cookie;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            request.KeepAlive = true;
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
            //request.Headers["Cookie"] = "username=aaaaaa; Language=zh_CN";
            //如果需要POST数据   
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, WebUtility.UrlEncode(parameters[key]));
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, WebUtility.UrlEncode(parameters[key]));
                    }
                    i++;
                }
                byte[] data = charset.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

            // return (HttpWebResponse)request.GetResponse();
        }

        public static string HttpHeadPostInfo(string url, Dictionary<string, string> headDic)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (HttpRequestMessage message, X509Certificate2 cert, X509Chain chain, SslPolicyErrors error) => true
            };
            HttpClient httpClient = new HttpClient(handler);
            if (headDic.Any())
            {
                foreach (KeyValuePair<string, string> item in headDic)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = httpClient.Send(request);
            Stream stream = response.Content.ReadAsStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string value = reader.ReadToEnd();
            reader.Close();
            return value;
        }

        public static string HttpPost(string url, Dictionary<string, string> headDic)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);
            if (headDic.Any())
            {
                foreach (KeyValuePair<string, string> item in headDic)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = httpClient.Send(request);
            Stream stream = response.Content.ReadAsStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string value = reader.ReadToEnd();
            reader.Close();
            return value;
        }

        // Post请求
        public string PostResponse(string url, string postData, out string statusCode)
        {
            string result = string.Empty;
            //设置Http的正文
            HttpContent httpContent = new StringContent(postData);
            //设置Http的内容标头
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //设置Http的内容标头的字符
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                //异步Post
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                //输出Http响应状态码
                statusCode = response.StatusCode.ToString();
                //确保Http响应成功
                if (response.IsSuccessStatusCode)
                {
                    //异步读取json
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        // 泛型：Post请求
        public T PostResponse<T>(string url, string postData) where T : class, new()
        {
            T result = default(T);

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    string s = t.Result;
                    //Newtonsoft.Json
                    string json = JsonConvert.DeserializeObject(s).ToString();
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            return result;
        }

        // 泛型：Get请求
        public T GetResponse<T>(string url) where T : class, new()
        {
            T result = default(T);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    string s = t.Result;
                    string json = JsonConvert.DeserializeObject(s).ToString();
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            return result;
        }

        // Get请求
        public string GetResponse(string url, out string statusCode)
        {
            string result = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                statusCode = response.StatusCode.ToString();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        // Put请求
        public string PutResponse(string url, string putData, out string statusCode)
        {
            string result = string.Empty;
            HttpContent httpContent = new StringContent(putData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;
                statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        // 泛型：Put请求
        public T PutResponse<T>(string url, string putData) where T : class, new()
        {
            T result = default(T);
            HttpContent httpContent = new StringContent(putData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    string s = t.Result;
                    string json = JsonConvert.DeserializeObject(s).ToString();
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            return result;
        }

    }
}
