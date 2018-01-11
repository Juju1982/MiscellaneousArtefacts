using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace GregWebServices
{
    public partial class WebServiceHelper : IWebServiceHelper
    {
        private readonly HttpClient client = new HttpClient(new HttpClientHandler()
        {
            UseDefaultCredentials = true
        });
        private const string MediaType = "application/json";
        private static readonly JsonSerializerSettings errorSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public WebServiceHelper(string baseUrl) : this(baseUrl, 120) { }

        /// <param name="baseUrl"></param>
        /// <param name="timeout">Timeout (seconds)</param>
        public WebServiceHelper(string baseUrl, int timeout)
        {
            if (!baseUrl.EndsWith("/")) { baseUrl += "/"; };
            client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(timeout);
        }

        public IWebServiceHelper AddOptions(WebServiceHelperOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Headers != null)
            {
                foreach (var header in options.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            return this;
        }

        #region overloads
        public T Call<T>(HttpMethod verb, string url, object obj)
        {
            return Call<T>(verb, url, JsonConvert.SerializeObject(obj));
        }

        public async Task<T> CallAsync<T>(HttpMethod verb, string url, object obj)
        {
            return await CallAsync<T>(verb, url, JsonConvert.SerializeObject(obj));
        }

        public T Call<T>(HttpMethod verb, string url, string body)
        {
            return JsonConvert.DeserializeObject<T>(Call(verb, url, body));
        }

        public async Task<T> CallAsync<T>(HttpMethod verb, string url, string body)
        {
            return JsonConvert.DeserializeObject<T>(await CallAsync(verb, url, body));
        }

        public T Call<T>(HttpMethod verb, string url)
        {
            return JsonConvert.DeserializeObject<T>(Call(verb, url));
        }

        public async Task<T> CallAsync<T>(HttpMethod verb, string url)
        {
            return JsonConvert.DeserializeObject<T>(await CallAsync(verb, url));
        }

        public async Task<string> CallAsync(HttpMethod verb, string url, object obj)
        {
            return await CallAsync(verb, url, JsonConvert.SerializeObject(obj));
        }

        public string Call(HttpMethod verb, string url, object obj)
        {
            return Call(verb, url, JsonConvert.SerializeObject(obj));
        }

        public string Call(HttpMethod verb, string url)
        {
            return Call(verb, url, "");
        }

        public async Task<string> CallAsync(HttpMethod verb, string url)
        {
            return await CallAsync(verb, url, "");
        }
        #endregion

        public string Call(HttpMethod verb, string url, string body)
        {
            return GetResponse(GetRequestMessage(verb, url, body));
        }

        public async Task<string> CallAsync(HttpMethod verb, string url, string body)
        {
            return await GetResponseAsync(GetRequestMessage(verb, url, body));
        }

        private string HandleResponse(HttpResponseMessage response, string body)
        {
            if (response.IsSuccessStatusCode)
            {
                return body;
            }
            else
            {
                WebError error = null;
                try
                {
                    error = JsonConvert.DeserializeObject<WebError>(body, errorSerializerSettings);
                }
                catch { }

                if (error != null)
                {
                    throw new WebException(error, response.StatusCode);
                }
                else
                {
                    throw new WebException(body, response.StatusCode);
                }
            }
        }

        public static HttpStatusCode GetStatusCodeForException(Exception e)
        {
            if (e is NotFoundException)
                return HttpStatusCode.NotFound;
            else if (e is UnauthorizedAccessException)
                return HttpStatusCode.Unauthorized;
            else
                return HttpStatusCode.InternalServerError;
        }

        public static string GetErrorResponse(Exception e)
        {
            return JsonConvert.SerializeObject(new WebError { Error = e.Message, Stack = e.StackTrace }, errorSerializerSettings);
        }

        private HttpRequestMessage GetRequestMessage(HttpMethod verb, string url, string body)
        {
            url = url.TrimStart('/');
            var content = new StringContent(body, Encoding.UTF8, MediaType);
            if (verb == HttpMethod.Get) { content = null; }
            return new HttpRequestMessage(verb, url) { Content = content };
        }

        private string GetResponse(HttpRequestMessage message)
        {
            var response = client.SendAsync(message).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            return HandleResponse(response, body);
        }

        private async Task<string> GetResponseAsync(HttpRequestMessage message)
        {
            var response = await client.SendAsync(message);
            var body = await response.Content.ReadAsStringAsync();
            return HandleResponse(response, body);
        }
    }
}
