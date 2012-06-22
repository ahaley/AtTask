 /*
 * Copyright (c) 2010 AtTask, Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
 * Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;

namespace AtTaskRestExample
{
    public class RestClient
    {
        private string url;
        public bool DebugUrls { get; set; }
        /// <summary>
        /// Creates a new RestClient that sends requests to the given root URL.
        /// </summary>
        /// <param name="url">
        /// A <see cref="System.String"/>
        /// </param>
        public RestClient(string url)
        {
            if (url.EndsWith("/")) {
                this.url = url.Substring(0, url.Length - 1);
            }
            else {
                this.url = url;
            }
        }
        /// <summary>
        /// Sends a request (by default a GET request) to the given path with the given parameters sent as a querystring.
        /// </summary>
        /// <param name="path">
        /// The path to the URL you want requested. This path is appended to the URL provided in the constructor.
        /// If "http://somesite.com" is provided to the constructor and "/search" is provided as the path parameter
        /// and "q=mySearch" and "day=today" are sent as parameters
        /// then an HTTP Request will be sent to "http://somesite.com/search?q=mysearch  </param>
        public JToken DoRequest(string path, params string[] parameters)
        {
            if (!path.StartsWith("/")) {
                path = "/" + path;
            }
            string fullUrl = url + path + ToQueryString(parameters);
            if (DebugUrls)
                Console.WriteLine("Requesting: {0}", fullUrl);
            WebRequest request = HttpWebRequest.CreateDefault(new Uri(fullUrl));
            using (WebResponse response = request.GetResponse()) {
                using (Stream responseStream = response.GetResponseStream()) {
                    return ReadResponse(responseStream);
                }
            }
        }
        /// <summary>
        /// Calls DoRequest as a GET request.
        ///<seealso cref="DoRequest"/>
        /// </summary>
        /// <returns>
        /// A <see cref="JToken"/> containing the json data returned by the server.
        /// </returns>
        public JToken DoGet(string path, params string[] parameters)
        {
            return DoRequest(path, parameters);
        }
        /// <summary>
        /// Calls DoRequest as a POST request.
        ///<seealso cref="DoRequest"/>
        /// </summary>
        /// <returns>
        /// A <see cref="JToken"/> containing the json data returned by the server.
        /// </returns>
        public JToken DoPost(string path, params string[] parameters)
        {
            List<string> list = parameters.ToList();
            list.Insert(0, "method=post");
            return DoRequest(path, list.ToArray());
        }
        /// <summary>
        /// Calls DoRequest as a PUT request.
        ///<seealso cref="DoRequest"/>
        /// </summary>
        /// <returns>
        /// A <see cref="JToken"/> containing the json data returned by the server.
        /// </returns>
        public JToken DoPut(string path, params string[] parameters)
        {
            List<string> list = parameters.ToList();
            list.Insert(0, "method=put");
            return DoRequest(path, list.ToArray());
        }
        /// <summary>
        /// Calls DoRequest as a DELETE request.
        ///<seealso cref="DoRequest"/>
        /// </summary>
        /// <returns>
        /// A <see cref="JToken"/> containing the json data returned by the server.
        /// </returns>
        public JToken DoDelete(string path, params string[] parameters)
        {
            List<string> list = parameters.ToList();
            list.Insert(0, "method=delete");
            return DoRequest(path, list.ToArray());
        }
        /// <summary>
        /// Converts the given <see cref="System.String[]"/> to query string format.
        /// </summary>
        /// <returns>
        /// If the parameters array contains ["item1", "item2"] the result will be "?item1"
        /// </returns>
        private string ToQueryString(string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            parameters.ToList().ForEach(s => sb.Append(s).Append("&"));
            if (sb.Length > 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            return "?" + sb.ToString();
        }

        /// <summary>
        /// Reads the given stream to the end then creates a new JToken containing all the data read.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> that provides JSON data.
        /// </param>
        /// <returns>
        /// A <see cref="JToken"/>
        /// </returns>
        private JToken ReadResponse(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string body = reader.ReadToEnd();
            return JObject.Parse(body);
        }
    }
}

