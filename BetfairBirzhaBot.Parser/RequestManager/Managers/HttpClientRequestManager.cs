
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;

namespace BetfairBirzhaBot.Parser.RequestManager.Managers
{
    public class HttpClientRequestManager : IRequestManager
    {
        private HttpClient _client;

        private int _requestsSended = 0;
        private const int MaxPersistentConnections = 1000;

        public int RequestsPerMinute { get; set; }


        public HttpClientRequestManager()
        {
            ServicePointManager.DefaultConnectionLimit = 1000;

            _client = new HttpClient(new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.Default,
                AllowAutoRedirect = true,
            });

            var mt = new MediaTypeWithQualityHeaderValue("application/json");
            
            
            _client.DefaultRequestHeaders.Accept.Add(mt);
        }

        private void FillHeaders(Dictionary<string, string> headers)
        {
            _client.DefaultRequestHeaders.Clear();

            foreach (var header in headers)
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        public async Task<T> Get<T>(string url, Dictionary<string, string> headers)
        {
                try
                {
                    var requestMessage = new HttpRequestMessage();
                    requestMessage.RequestUri = new Uri(url);
                    requestMessage.Method = HttpMethod.Get;
                    requestMessage.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

                    foreach (var header in headers)
                        if (header.Key != "Content-Type")
                            requestMessage.Headers.Add(header.Key, header.Value);

                    var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);
                    string apiResponse = await ReadResponseStream(response).ConfigureAwait(false);

                    _requestsSended++;
                    
                    if (typeof(T) == typeof(string))
                        return (T)(object)apiResponse;

                    return JsonConvert.DeserializeObject<T>(apiResponse);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"GET FAILED | {url} ");
                }

                return JsonConvert.DeserializeObject<T>("");
        }

        private async Task<string> ReadResponseStream(HttpResponseMessage response)
        {
            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
            return null;
        }

        public async Task Options(string url, Dictionary<string, string> headers)
        {
            
            
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Options, url);

                //FillHeaders(headers);
                _requestsSended++;
                // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OPTIONS FAILED | {ex.ToString()} ");
            }
            finally
            {
                Debug.WriteLine($"OPTIONS OK | {url} ");
            }
        }

        public async Task<T> Post<T>(string url, Dictionary<string, string> headers, string postData = "")
        {
                try
                {
                    var requestMessage = new HttpRequestMessage();
                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri = new Uri(url);

                    foreach (var header in headers)
                        if (header.Key != "Content-Type")
                            requestMessage.Headers.Add(header.Key, header.Value);

                    if (!string.IsNullOrEmpty(postData))
                        requestMessage.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                    //FillHeaders(headers);

                    var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);

                    string apiResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    _requestsSended++;

                    if (typeof(T) == typeof(string))
                        return (T)(object)apiResponse;
                return JsonConvert.DeserializeObject<T>(apiResponse);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"POST FAILED | {url} ");
                }

                return JsonConvert.DeserializeObject<T>("");
        }
    }
}
