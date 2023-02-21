using BetfairBirzhaBot.Common.Helpers;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.WebBot
{
    public class PlaywrightBotFactory
    {
        private IPlaywright _playwright;
        public PlaywrightBotFactory()
        {
            Task.Run(async () => {
                _playwright = await Playwright.CreateAsync();
            }).Wait();
        }
        public async Task<IBrowserContext> CreateBot(string userPath, bool headless = false)
        {
            var context = await _playwright.Chromium.LaunchPersistentContextAsync(userPath, new BrowserTypeLaunchPersistentContextOptions()
            {
                Channel = "chrome",
                Args = new []
                {
                    "--flag-switches-begin",
                    "--flag-switches-end",
                },
                IgnoreDefaultArgs = new[]
                {
                   "--incognito",
                    "--enable-automation",
                    "--no-sandbox",
                },
                ViewportSize = ViewportSize.NoViewport,
                ChromiumSandbox = false,
                Headless = headless,
                Timeout = 120 * 1000,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
            });

            

            string antifraud = AssemblyHelper.ReadResource("betfair-antifraud.js");
            string script = AssemblyHelper.ReadResource("betfair-script.js");

            await context.AddInitScriptAsync(antifraud + "\n" + script);
            await context.Pages[0].GotoAsync("https://www.betfair.com/exchange/plus/ru/%D1%84%D1%83%D1%82%D0%B1%D0%BE%D0%BB-%D1%81%D1%82%D0%B0%D0%B2%D0%B8%D1%82%D1%8C-1/inplay");
            return context;
        }

        private string GenerateUserDataPath()
        {
            long random = DateTimeOffset.Now.ToUnixTimeSeconds();

            return $"playwright-{random}";
        }
    }
}
