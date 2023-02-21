using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Common.Entities.ResponseEntities;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Parser.RequestManager;
using BetfairBirzhaBot.Parser.RequestManager.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace BetfairBirzhaBot.Parser
{
    public class BetfairAPI
    {
        private UserRequestData _userRequestData;
        private IRequestManager _requestManager;

        public BetfairAPI()
        {

        }

        public void SetupRequestManager(IRequestManager requestManager)
        {
            _requestManager = requestManager;
        }
        public void InitializeRequestData(UserRequestData data)
        {
            _userRequestData = data;
        }

        public async Task<MarketsResponse> GetMarketData(string eventId)
        {
            try
            {
                var payloadRequest = new JObject
                {
                    ["currencyCode"] = "EUR",
                    ["locale"] = "ru",
                    ["facets"] = new JArray
                {
                    new JObject
                    {
                        ["applyNextTo"] = 0,
                        ["maxValues"] = 0,
                        ["skipValues"] = 0,
                        ["type"] = "EVENT",
                        ["next"] = new JObject
                        {
                            ["applyNextTo"] = 0,
                            ["maxValues"] = 0,
                            ["skipValues"] = 0,
                            ["type"] = "MARKET"
                        }
                    }
                },
                    ["filter"] = new JObject
                    {
                        ["attachments"] = new JArray
                    {
                        "MARKET_LITE"
                    },
                        ["contentGroup"] = new JObject
                        {
                            ["language"] = "en",
                            ["regionCode"] = "UK"
                        },
                        ["eventTypeIds"] = new JArray
                    {
                        1
                    },
                        ["marketBettingTypes"] = new JArray
                    {
                        "ASIAN_HANDICAP_SINGLE_LINE",
                        "ASIAN_HANDICAP_DOUBLE_LINE",
                        "ODDS",
                        "LINE"
                    },
                        ["maxResults"] = 0,
                        ["productTypes"] = new JArray
                    {
                        "EXCHANGE"
                    },
                        ["selectBy"] = "RANK",
                        ["upperLevelEventIds"] = new JArray
                    {
                        eventId.ToString()
                    },
                    }
                };


                var result = await _requestManager.Post<MarketsResponse>(
                    $"https://scan-inbf.betfair.com/www/sports/navigation/facet/v1/search?_ak={_userRequestData.Key}&alt=json",
                    _userRequestData.Headers,
                    payloadRequest.ToString())
                    .ConfigureAwait(false);

                return result;
            }
            catch
            {

            }

            return null;
        }
        public async Task<bool> PlaceStake(BetData betData)
        {
            try
            {
                string url = $"https://etx.betfair.com/www/etx-json-rpc?_ak={_userRequestData.Key}&alt=json";

                var jo = new JArray
                    {
                        new JObject
                        {
                            ["method"] = "ExchangeTransactional/v1.0/place",
                            ["id"] = $"{betData.MarketId}-plc",
                            ["jsonrpc"] = "2.0",
                            ["params"] = new JObject
                            {
                                ["marketId"] = betData.MarketId,
                                ["customerRef"] = $"{GetUnixTimeStamp()}-{betData.MarketId}-plc-0",
                                ["useAvailableBonus"] = false,
                                ["instructions"] = new JArray
                                {
                                    new JObject
                                    {
                                        ["selectionId"] = betData.SelectionId,
                                        ["handicap"] = 0,
                                        ["orderType"] = "LIMIT",
                                        ["side"] = "BACK",
                                        ["limitOrder"] = new JObject
                                        {
                                            ["size"] = betData.Stake,
                                            ["price"] = betData.Coefficient,
                                            ["persistenceType"] = "LAPSE"
                                        }
                                    }
                                }
                            }
                        }
                    };

                var result = await _requestManager
                    .Post<string>(url, _userRequestData.Headers, jo.ToString())
                    .ConfigureAwait(false);
                
                return result.ToLower().Contains("success");
            }
            catch
            {
                Debug.WriteLine($"Send bet result");
            }

            return false;
        }
        public async Task<MarketDetailsResponse> GetMarketsDetails(IEnumerable<string> marketsId)
        {
            if (marketsId is null || marketsId.Count() == 0)
                return new MarketDetailsResponse();
            string url = $"https://ero.betfair.com/www/sports/exchange/readonly/v1/bymarket?_ak={_userRequestData.Key}&alt=json&currencyCode=EUR&locale=ru&marketIds={string.Join(",", marketsId)}&rollupLimit=50&rollupModel=STAKE&types=MARKET_STATE,MARKET_RATES,MARKET_DESCRIPTION,EVENT,RUNNER_DESCRIPTION,RUNNER_STATE,RUNNER_EXCHANGE_PRICES_BEST,RUNNER_METADATA,MARKET_LICENCE,MARKET_LINE_RANGE_INFO";
            
            return await _requestManager.Get<MarketDetailsResponse>(url, _userRequestData.Headers).ConfigureAwait(false);
        }
        public async Task<EventTimeline> GetEventTimeLine(string eventId)
        {
            string url = $"https://ips.betfair.com/inplayservice/v1/eventTimeline?_ak={_userRequestData.Key}&alt=json&eventId={eventId}&locale=ru&productType=EXCHANGE&regionCode=UK";
            
            return await _requestManager.Get<EventTimeline>(url, _userRequestData.Headers).ConfigureAwait(false);
        }
        public async Task<string> LoadGame(string url)
        {
            var result = await _requestManager.Get<string>(url, _userRequestData.Headers);

            return result;
        }
        public async Task<StatisticsResponse> GetAllGameStatistics(string eventId)
        {
            try
            {
                string url = $"https://betfair.betstream.betgenius.com/betstream-view/footballscorecentre/betfairfootballscorecentre/json?eventId={eventId}&culture=ru-RU&width=334&height=190&cb={GetUnixTimeStamp()}";

                var headers = new Dictionary<string, string>();

                var result = await _requestManager.Get<string>(url, headers).ConfigureAwait(false);
                var statistics = JsonConvert.DeserializeObject<StatisticsResponse>(result);

                return statistics;
            }
            catch
            {

            }
            return new StatisticsResponse();
        }

        //public async Task<Statistics> GetShortGameStatistics(string eventId)
        //{
        //    string url = $"https://betfair.betstream.betgenius.com/betstream-view/footballstatistics/betfairfootballscorecentre/json?eventId={eventId}&culture=ru-RU&width=334&height=190&cb={GetUnixTimeStamp()}";

        //    var headers = new Dictionary<string, string>();

        //    var result = await _requestManager.Get<Statistics>(url, headers).ConfigureAwait(false);

        //    return result;
        //}
        //private object _locker = new object();
        //public async Task GetStatsPlayer(string eventId)
        //{
        //    lock (_locker)
        //    {
        //        string url = $"https://videoplayer.betfair.com/GetPlayer.do?tr=1&eID={eventId}&width=334&height=190&allowPopup=true&contentType=viz&statsToggle=hide&contentOnly=true";

        //        string response = _requestManager.Get<string>(url, _userRequestData.Headers).Result;
        //        string foundKey = "data-outletkey=";
        //        string eventKeyFound = "&eventId=";
        //        int startIndex = response.IndexOf(foundKey) + foundKey.Length;
        //        int eventIndex = response.IndexOf(eventKeyFound) + eventKeyFound.Length;

        //        string userAuthKeyData = response.Substring(startIndex + 1);
        //        string eventKeyData = response.Substring(eventIndex);

        //        int endOfString = userAuthKeyData.IndexOf("'");
        //        int endOfEventKey = eventKeyData.IndexOf('&');
        //        //1 key 
        //        string key = userAuthKeyData.Substring(0, endOfString);
        //        string eventKey = eventKeyData.Substring(0, endOfEventKey);
        //    }

        //}

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            string url = $"https://scan-inbf.betfair.com/www/sports/navigation/facet/v1/search?_ak={_userRequestData.Key}&alt=json";
            var navContent = new NavigationData
            {
                CurrencyCode = "EUR",
                Facets = new List<Facet>
                {
                    new Facet
                    {
                        MaxValues = 100,
                        Next = new NextEntity
                        {
                            MaxValues = 1000,
                            SkipValues = 0,
                            Type = "EVENT",
                            Next = new NextEntity
                            {
                                MaxValues = 100,
                                Type = "COMPETITION",
                            },
                        },
                        SkipValues = 0,
                        Type = "EVENT_TYPE"
                    }
                },
                Locale = "ru",
                Filter = new Filter
                {
                    ContentGroup = new ContentGroup { Language = "en", RegionCode = "UK" },
                    EventTypeIds = new List<int> { 1 },
                    MarketBettingTypes = new List<string>
                    {
                        "ASIAN_HANDICAP_SINGLE_LINE",
                        "ASIAN_HANDICAP_DOUBLE_LINE",
                        "ODDS"
                    },
                    MarketTypeCodes = new List<string>
                    {
                        "MATCH_ODDS"
                    },
                    MaxResults = 0,
                    ProductTypes = new List<string> { "EXCHANGE" },
                    SelectBy = "RANK",
                    TurnInPlayEnabled = true
                }
            };

            string jsonContent = JsonConvert.SerializeObject(navContent);

            string responseContent = await _requestManager.Post<string>(url, _userRequestData.Headers, jsonContent).ConfigureAwait(false);

            var responseObj = JsonConvert.DeserializeObject<dynamic>(responseContent);

            string events = responseObj["attachments"]["events"].ToString();

            var eventsList = JsonConvert.DeserializeObject<Dictionary<string, Event>>(events);

            var result = new List<Event>();
            foreach (var e in eventsList.Values)
            {

                var offset = DateTime.UtcNow - e.OpenDate;
                if (offset.TotalMinutes < -15)
                    continue;

                result.Add(e);
            }

            return result;
        }

        private string GetUnixTimeStamp()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        }
    }

    public class Event
    {
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("openDate")]
        public DateTime OpenDate { get; set; }
    }

    public class ContentGroup
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("regionCode")]
        public string RegionCode { get; set; }
    }

    public class Facet
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("skipValues")]
        public int SkipValues { get; set; }

        [JsonProperty("maxValues")]
        public int MaxValues { get; set; }

        [JsonProperty("next")]
        public NextEntity Next { get; set; }
    }

    public class Filter
    {
        [JsonProperty("marketBettingTypes")]
        public List<string> MarketBettingTypes { get; set; }

        [JsonProperty("productTypes")]
        public List<string> ProductTypes { get; set; }

        [JsonProperty("marketTypeCodes")]
        public List<string> MarketTypeCodes { get; set; }

        [JsonProperty("contentGroup")]
        public ContentGroup ContentGroup { get; set; }

        [JsonProperty("turnInPlayEnabled")]
        public bool TurnInPlayEnabled { get; set; }

        [JsonProperty("maxResults")]
        public int MaxResults { get; set; }

        [JsonProperty("selectBy")]
        public string SelectBy { get; set; }

        [JsonProperty("eventTypeIds")]
        public List<int> EventTypeIds { get; set; }
    }

    public class NextEntity
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("skipValues")]
        public int SkipValues { get; set; }

        [JsonProperty("maxValues")]
        public int MaxValues { get; set; }

        [JsonProperty("next")]
        public NextEntity Next { get; set; }
    }

    public class NavigationData
    {
        [JsonProperty("filter")]
        public Filter Filter { get; set; }

        [JsonProperty("facets")]
        public List<Facet> Facets { get; set; }

        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }
    }


}
