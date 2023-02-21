using Leaf.xNet;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BetfairBirzhaBot.Parser.RequestManager.Managers
{
    public class LeafXNetRequestManager : IRequestManager
    {
        public async Task<T> Get<T>(string url, Dictionary<string, string> headers)
        {
            //Debug.WriteLine($"GET | {url}");
            try
            {
                using (var req = new HttpRequest())
                {
                    //req.Proxy = HttpProxyClient.DebugHttpProxy;
                    req.IgnoreProtocolErrors = true;
                    req.ConnectTimeout = 2 * 1000;
                    req.KeepAliveTimeout = 2 * 1000;
                    req.ReadWriteTimeout = 2 * 1000;
                    req.EnableEncodingContent = false;
                    foreach (var header in headers)
                        req.AddHeader(header.Key, header.Value);

                    var response = req.Get(url);
                    var responseText = response.ToString();

                    //Debug.WriteLine($"GET | {response.StatusCode}");

                    if (typeof(T) == typeof(string))
                        return (T)(object)responseText;

                    return JsonConvert.DeserializeObject<T>(responseText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GET | Failed | {url}");
            }

            return JsonConvert.DeserializeObject<T>("");
        }

        public async Task<T> Post<T>(string url, Dictionary<string, string> headers, string postData = "")
        {
            try
            {
                //Debug.WriteLine($"POST | {url}");
                using (var req = new HttpRequest())
                {
                    //req.Proxy = HttpProxyClient.DebugHttpProxy;
                    req.IgnoreProtocolErrors = true;
                    req.ConnectTimeout = 2 * 1000;
                    req.KeepAliveTimeout = 2 * 1000;
                    req.ReadWriteTimeout = 2 * 1000;
                    req.EnableEncodingContent = false;

                    foreach (var header in headers)
                        req.AddHeader(header.Key, header.Value);

                    var content = new Leaf.xNet.StringContent(postData);
                    var response = req.Post(url, content);

                    //Debug.WriteLine($"POST | {response.StatusCode}");
                    var responseText = response.ToString();

                    if (typeof(T) == typeof(string))
                        return (T)(object)responseText;

                    return JsonConvert.DeserializeObject<T>(responseText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST | FAILED | {ex.Message}");
            }

            return JsonConvert.DeserializeObject<T>("");
        }

        public async Task Options(string url, Dictionary<string, string> headers)
        {
            try
            {
                //Debug.WriteLine($"POST | {url}");
                using (var req = new HttpRequest())
                {
                    req.Proxy = HttpProxyClient.DebugHttpProxy;
                    req.IgnoreProtocolErrors = true;
                    req.ConnectTimeout = 2 * 1000;
                    req.KeepAliveTimeout = 2 * 1000;
                    req.ReadWriteTimeout = 2 * 1000;
                    req.EnableEncodingContent = true;

                    foreach (var header in headers)
                        req.AddHeader(header.Key, header.Value);

                    req.Options(url);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST | FAILED | {ex.Message}");
            }
        }
    }
}
