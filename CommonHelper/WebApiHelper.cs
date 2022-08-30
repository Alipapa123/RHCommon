using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class WebApiHelper
    {
        /// <summary>
        /// WebApi基础网址,
        /// </summary>
        public static string BaseUri = "";
        #region GET请求--异步方法

        /// <summary>
        /// GET请求--异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">方法</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static async Task<T> TryGetAsync<T>(string action, Dictionary<string, string> param)
        {
            using (HttpClient client = new HttpClient())
            {
                //基地址/域名
                client.BaseAddress = new Uri(BaseUri);
                //设定传输格式为json
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");

                StringBuilder strUri = new StringBuilder();
                //方法
                strUri.AppendFormat("{0}?", action);
                //参数
                if (param.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in param)
                    {
                        strUri.AppendFormat("{0}={1}&&", pair.Key, pair.Value);
                    }
                    strUri.Remove(strUri.Length - 2, 2); //去掉'&&'
                }
                else
                {
                    strUri.Remove(strUri.Length - 1, 1); //去掉'？'
                }
                HttpResponseMessage response = await client.GetAsync(strUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                return default(T);
            }
        }

        #endregion

        #region 无参数GET请求--异步方法

        /// <summary>
        /// 无参数GET请求--异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<T> TryGetAsync<T>(string action)
        {
            using (HttpClient client = new HttpClient())
            {
                //基地址/域名
                client.BaseAddress = new Uri(BaseUri);
                //设定传输格式为json
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(action);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                return default(T);
            }
        }

        #endregion
        #region POST请求--异步方法

        /// <summary>
        /// POST请求--异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">方法</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static async Task<T> TryPostAsync<T>(string action, object param)
        {
            using (HttpClient client = new HttpClient())
            {
                //基地址/域名
                client.BaseAddress = new Uri(BaseUri);
                //设定传输格式为json
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsync(action, param, new JsonMediaTypeFormatter());
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                return default(T);
            }
        }

        #endregion

        #region 批量文件上传--含参数--异步方法

        /// <summary>
        /// 批量文件上传--含参数--异步方法
        /// </summary>
        /// <param name="action"></param>
        /// <param name="filePaths"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<string> TryUploadAsync(string action, Dictionary<string, string> filePaths,
            Dictionary<string, string> param)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    //基地址/域名
                    client.BaseAddress = new Uri(BaseUri);
                    foreach (var filePath in filePaths)
                    {
                        var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath.Key));
                        fileContent.Headers.ContentDisposition =
                            new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = filePath.Value
                            };
                        content.Add(fileContent);
                    }
                    foreach (var pair in param)
                    {
                        var dataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(pair.Value));
                        dataContent.Headers.ContentDisposition =
                            new ContentDispositionHeaderValue("attachment") { Name = pair.Key };
                        content.Add(dataContent);
                    }
                    HttpResponseMessage response = await client.PostAsync(action, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    return await Task.FromResult("");
                }
            }
        }

        #endregion


    }
}
