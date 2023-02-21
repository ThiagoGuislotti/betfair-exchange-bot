using BetfairBirzhaBot.Base;

namespace BetfairBirzhaBot.Core
{
    public class TelegramMessageSettings : BaseViewModel
    {
        public bool WinResultsStart { get; set; }
        public bool WinResultsLive { get; set; }
        public bool BothToScoreStart { get; set; }
        public bool BothToScoreLive { get; set; }
        public bool TotalsStartFirstHalf { get; set; }
        public string TotalsStartFirstHalfDiapason { get; set; }
        public bool TotalsStartFullGame { get; set; }
        public string TotalsStartFullDiapason { get; set; }
        public bool TotalsFirstHalfLive { get; set; }
        public string TotalsFirstHalfLiveDiapason { get; set; }
        public bool TotalsFullGameLive { get; set; }
        public string TotalsFullGameLiveDiapason { get; set; }

        public void Update(TelegramMessageSettings tg)
        {
            WinResultsStart = tg.WinResultsStart;
            WinResultsLive = tg.WinResultsLive;
            BothToScoreStart = tg.BothToScoreStart;
            BothToScoreLive = tg.BothToScoreLive;
            TotalsStartFirstHalf = tg.TotalsStartFirstHalf;
            TotalsStartFirstHalfDiapason = tg.TotalsStartFirstHalfDiapason;
            TotalsStartFullGame = tg.TotalsStartFullGame;
            TotalsStartFullDiapason = tg.TotalsStartFullDiapason;
            TotalsFirstHalfLive = tg.TotalsFirstHalfLive;
            TotalsFirstHalfLiveDiapason = tg.TotalsFirstHalfLiveDiapason;
            TotalsFullGameLive = tg.TotalsFullGameLive;
            TotalsFullGameLiveDiapason = tg.TotalsFullGameLiveDiapason;

            OnPropertyChanged(nameof(WinResultsStart));
            OnPropertyChanged(nameof(WinResultsLive));
            OnPropertyChanged(nameof(BothToScoreStart));
            OnPropertyChanged(nameof(BothToScoreLive));
            OnPropertyChanged(nameof(TotalsStartFirstHalf));
            OnPropertyChanged(nameof(TotalsStartFirstHalfDiapason));
            OnPropertyChanged(nameof(TotalsStartFullGame));
            OnPropertyChanged(nameof(TotalsStartFullDiapason));
            OnPropertyChanged(nameof(TotalsFirstHalfLive));
            OnPropertyChanged(nameof(TotalsFirstHalfLiveDiapason));
            OnPropertyChanged(nameof(TotalsFullGameLive));
            OnPropertyChanged(nameof(TotalsFullGameLiveDiapason));
        }

    }
}
