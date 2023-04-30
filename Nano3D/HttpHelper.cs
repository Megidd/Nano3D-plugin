using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        /// Use this method like this:
        /// ```
        /// string url = "http://example.com/api";
        /// Dictionary<string, string> fields = new Dictionary<string, string>();
        /// fields.Add("param1", "value1");
        /// fields.Add("param2", "value2");
        /// Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
        /// byte[] fileData = File.ReadAllBytes(@"C:\path\to\file.txt");
        /// files.Add("file1", fileData);
        /// string response = HttpHelper.SendHttpPostRequest(url, fields, files);
        /// Console.WriteLine(response);
        /// ```
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string SendHttpPostRequest(string url, Dictionary<string, string> fields, Dictionary<string, byte[]> files)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] trailerBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            using (MemoryStream requestStream = new MemoryStream())
            {
                // Write the fields to the request body
                if (fields != null)
                {
                    foreach (KeyValuePair<string, string> field in fields)
                    {
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        string fieldHeader = "Content-Disposition: form-data; name=\"" + field.Key + "\"\r\n\r\n";
                        byte[] fieldHeaderBytes = Encoding.UTF8.GetBytes(fieldHeader);
                        requestStream.Write(fieldHeaderBytes, 0, fieldHeaderBytes.Length);
                        byte[] fieldValueBytes = Encoding.UTF8.GetBytes(field.Value);
                        requestStream.Write(fieldValueBytes, 0, fieldValueBytes.Length);
                    }
                }
                // Write the files to the request body
                if (files != null)
                {
                    foreach (KeyValuePair<string, byte[]> file in files)
                    {
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        string fileHeader = "Content-Disposition: form-data; name=\"" + file.Key + "\"; filename=\"" + file.Key + "\"\r\n";
                        string fileContentType = "Content-Type: application/octet-stream\r\n\r\n";
                        byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader + fileContentType);
                        requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                        requestStream.Write(file.Value, 0, file.Value.Length);
                    }
                }
                requestStream.Write(trailerBytes, 0, trailerBytes.Length);
                request.ContentLength = requestStream.Length;
                using (Stream requestStream2 = request.GetRequestStream())
                {
                    requestStream.Position = 0;
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = requestStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        requestStream2.Write(buffer, 0, bytesRead);
                    }
                }
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
