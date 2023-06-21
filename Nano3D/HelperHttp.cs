using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Nano3D
{
    internal class HelperHttp
    {
        public const string ip = "127.0.0.1";
        public static string port = "8080"; // A free port will be found on plugin load.
        public static string address = "http://"+ip+":"+port;
        public const string route = "/api/rhino/v1";

        public const string routeHollowing = route+ "/hollowing";
        public static string UrlHollowing = address + routeHollowing;

        // To find a free port, first these options are checked.
        private static readonly uint[] portPool = { 8080, 7123, 5976, 4422 };

        /// <summary>
        /// First checks the port pool for available ports.
        /// If no available ports are found in the pool,
        /// the method generates up to 1000 random port numbers and checks their availability.
        /// It is not a foolproof method, but it should be sufficient for most use cases.
        /// </summary>
        /// <returns></returns>
        public static int FindAvailablePort()
        {
            foreach (var port in portPool)
            {
                var endPoint = new IPEndPoint(IPAddress.Any, (int)port);
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        socket.Bind(endPoint);
                        return (int)port;
                    }
                    catch (SocketException)
                    {
                        // Port is already in use, try the next one
                    }
                }
            }

            // No available ports found in the pool, try random ports
            const int maxAttempts = 1000;
            var random = new Random();
            for (var i = 0; i < maxAttempts; i++)
            {
                var port = random.Next(1024, 65535);
                var endPoint = new IPEndPoint(IPAddress.Any, port);
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        socket.Bind(endPoint);
                        return port;
                    }
                    catch (SocketException)
                    {
                        // Port is already in use, try the next one
                    }
                }
            }

            // No available ports found
            return -1;
        }

        /// <summary>
        /// Send POST requests by providing form-data parameters of type text and file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static async Task<byte[]> SendPostRequest(string url, Dictionary<string, string> fields, Dictionary<string, byte[]> files)
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
                HttpResponseMessage response = await httpClient.PostAsync(url, form);
                // Read the response as a byte array
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
}
