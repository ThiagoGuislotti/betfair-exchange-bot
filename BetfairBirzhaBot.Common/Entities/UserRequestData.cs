namespace BetfairBirzhaBot.Common.Entities
{
    public class UserRequestData
    {
        public string Key { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();

        public async Task WaitValidData()
        {
            while (Key?.Length == 0 || Headers.Count == 0)
                await Task.Delay(10);
        }
    }
}
