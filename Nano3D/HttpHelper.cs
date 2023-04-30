using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Nano3D
{
    internal class HttpHelper
    {
        public const string ip = "127.0.0.1";
        public static string port = "8080";
        public static string address = "http://"+ip+":"+port;
        public const string route = "/api/rhino/v1";

        public const string routeHollowing = route+ "/hollowing";
        public static string UrlHollowing = address + routeHollowing;

        /// <summary>
        /// Send POST requests by providing form-data parameters of type text and file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static byte[] SendPostRequest(string url, Dictionary<string, string> fields, Dictionary<string, byte[]> files)
        {
            using (HttpClient httpClient = new HttpClient())
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                // Add the fields to the request
                if (fields != null)
                {
                    foreach (KeyValuePair<string, string> field in fields)
                    {
                        form.Add(new StringContent(field.Value), field.Key);
                    }
                }
                // Add the files to the request
                if (files != null)
                {
                    foreach (KeyValuePair<string, byte[]> file in files)
                    {
                        form.Add(new ByteArrayContent(file.Value), file.Key, file.Key);
                    }
                }
                // Send the request and get the response
                HttpResponseMessage response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = form
                }).Result;
                // Read the response as a byte array
                return response.Content.ReadAsByteArrayAsync().Result;
            }
        }
    }
}
