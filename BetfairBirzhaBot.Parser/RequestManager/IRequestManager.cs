namespace BetfairBirzhaBot.Parser.RequestManager
{
    public interface IRequestManager
    {
        Task Options(string url, Dictionary<string, string> headers);

        Task<T> Get<T>(string url, Dictionary<string, string> headers);
        Task<T> Post<T>(string url, Dictionary<string, string> headers, string postData = "");
    }
}
