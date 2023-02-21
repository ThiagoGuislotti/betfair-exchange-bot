using BetfairBirzhaBot.Common.Enums;
using BetfairBirzhaBot.Models;
using System;

namespace BetfairBirzhaBot.Services
{
    public class LogService
    {
        public event Action<LogItemModel> OnLog;

        public void Info(string text)
        {
            OnLog(new LogItemModel(FormatMessage(text), ELogType.INFO));
        }

        public void Error(string text)
        {
            OnLog(new LogItemModel(FormatMessage(text), ELogType.ERROR));
        }

        public void Warning(string text)
        {
            OnLog(new LogItemModel(FormatMessage(text), ELogType.WARNING));
        }

        public void Success(string text)
        {
            OnLog(new LogItemModel(FormatMessage(text), ELogType.SUCCESS));
        }

        public void Processing(string text)
        {
            OnLog(new LogItemModel(FormatMessage(text), ELogType.PROCESSING));
        }

        private string FormatMessage(string text)
        {
            return $"[ {DateTime.Now.ToString("HH:mm:ss")} ] {text}";
        }
    }

  
}
