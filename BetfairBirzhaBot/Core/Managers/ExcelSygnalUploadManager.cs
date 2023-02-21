using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Common.Entities.MarketEntities;
using BetfairBirzhaBot.Converters;
using BetfairBirzhaBot.Filters.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetfairBirzhaBot.Core.Managers
{
    public class ExcelSygnalUploadManager
    {
        private readonly string _filename = "ARCHIVE.xlsx";
        private readonly string _templateFilename = "archive-template.xlsx";
        private IXLWorksheet _currentWorksheet;
        private XLWorkbook _currentWorkbook;

        public async Task Upload(List<StrategySygnalResult> sygnals, string path)
        {
            string destpath = Path.Combine(path, _filename);
            int startRow = 10;

            if (File.Exists(destpath))
            {
                _currentWorkbook = new XLWorkbook(destpath);
                _currentWorksheet = _currentWorkbook.Worksheets.First();

                var lastRow = _currentWorksheet.LastRowUsed();

                startRow = lastRow.RowNumber() + 1;
            }

            

            if (!File.Exists(destpath))
            {
                File.Copy(_templateFilename, destpath);

                _currentWorkbook = new XLWorkbook(destpath);
                _currentWorksheet = _currentWorkbook.Worksheets.First();
            }



            int r = startRow;
            foreach (var sygnal in sygnals)
            {
                try
                {
                    int c = 2;
                    var g = sygnal.Game;
                    var winCoefsPrematch = GetWinCoefficients(g.WinMarketsStartGame);
                    var winCoefsLive = GetWinCoefficients(g.WinMarketsCurrent);
                    var dcPrematch = GetDCCoefficients(g.DoubleChanceMarketsStart);
                    var dcLive = GetDCCoefficients(g.DoubleChanceMarketsCurrent);
                    var s = g?.Statistics?.Statistics ?? new Statistics();

                    Set(r, c++, sygnal.Index);
                    Set(r, c++, g.ElapsedMinutes);
                    Set(r, c++, g.League);
                    Set(r, c++, sygnal.StrategyName);
                    Set(r, c++, g.Teams[0].Name);
                    Set(r, c++, g.Teams[1].Name);


                    Set(r, c++, $"{g.Teams[0].Score} - {g.Teams[1].Score}");

                    string afterFirstHalfScore = string.Empty;

                    if (g.FirstHalfHomeScore != -1)
                        afterFirstHalfScore = $"{g.FirstHalfHomeScore} - {g.FirstHalfAwayScore}";
                    Set(r, c++, afterFirstHalfScore);

                    string lastTimeScore = "";
                    if (g.LastHomeScore != -1)
                        lastTimeScore = $"{g.LastHomeScore} - {g.LastAwayScore}";

                    Set(r, c++, lastTimeScore);
                    Set(r, c++, "");
                    

                    Set(r, c++, winCoefsPrematch[0]);
                    Set(r, c++, winCoefsPrematch[1]);
                    Set(r, c++, winCoefsPrematch[2]);
                    Set(r, c++, dcPrematch[0]);
                    Set(r, c++, dcPrematch[1]);
                    Set(r, c++, dcPrematch[2]);
                    Set(r, c++, BothConvert(g.BothToScoreMarketsStart.Find(x => x.Type == EBothToScoreType.Yes)));
                    Set(r, c++, BothConvert(g.BothToScoreMarketsStart.Find(x => x.Type == EBothToScoreType.No)));

                    Set(r, c++, GetCorrectScoreCoef(0, 0, g.CorrectScoreMarketsPrematch));
                    Set(r, c++, GetCorrectScoreCoef(1, 0, g.CorrectScoreMarketsPrematch));
                    Set(r, c++, GetCorrectScoreCoef(0, 1, g.CorrectScoreMarketsPrematch));
                    Set(r, c++, GetCorrectScoreCoef(1, 1, g.CorrectScoreMarketsPrematch));

                    for (double p = 0.5; p <= 6.5; p += 1)
                    {
                        var total = g.TotalMarketsStart.Find(x => x.Parameter == p);
                        if (total == null)
                        {
                            c += 2;
                            continue;
                        }

                        Set(r, c++, total.Over.Coefficient);
                        Set(r, c++, total.Under.Coefficient);
                    }

                    for (double p = 0.5; p <= 6.5; p += 1)
                    {
                        var total = g.TotalMarketsFirstHalfStart.Find(x => x.Parameter == p);
                        if (total == null)
                        {
                            c += 2;
                            continue;
                        }

                        Set(r, c++, total.Over.Coefficient);
                        Set(r, c++, total.Under.Coefficient);
                    }

                    //LIVE 
                    Set(r, c++, winCoefsLive[0]);
                    Set(r, c++, winCoefsLive[1]);
                    Set(r, c++, winCoefsLive[2]);
                    Set(r, c++, dcLive[0]);
                    Set(r, c++, dcLive[1]);
                    Set(r, c++, dcLive[2]);
                    Set(r, c++, BothConvert(g.BothToScoreMarketsCurrent.Find(x => x.Type == EBothToScoreType.Yes)));
                    Set(r, c++, BothConvert(g.BothToScoreMarketsCurrent.Find(x => x.Type == EBothToScoreType.No)));

                    Set(r, c++, GetCorrectScoreCoef(0, 0, g.CorrectScoreMarkets));
                    Set(r, c++, GetCorrectScoreCoef(1, 0, g.CorrectScoreMarkets));
                    Set(r, c++, GetCorrectScoreCoef(0, 1, g.CorrectScoreMarkets));
                    Set(r, c++, GetCorrectScoreCoef(1, 1, g.CorrectScoreMarkets));

                    for (double p = 0.5; p <= 6.5; p += 1)
                    {
                        var total = g.TotalMarketsCurrent.Find(x => x.Parameter == p);
                        if (total == null)
                        {
                            c += 2;
                            continue;
                        }

                        Set(r, c++, total.Over.Coefficient);
                        Set(r, c++, total.Under.Coefficient);
                    }

                    for (double p = 0.5; p <= 6.5; p += 1)
                    {
                        var total = g.TotalMarketsFirstHalfCurrent.Find(x => x.Parameter == p);
                        if (total == null)
                        {
                            c += 2;
                            continue;
                        }

                        Set(r, c++, total.Over.Coefficient);
                        Set(r, c++, total.Under.Coefficient);
                    }



                    Set(r, c++, s.HomeAttacks);
                    Set(r, c++, s.AwayAttacks);
                    Set(r, c++, s.HomeDangerousAttacks);
                    Set(r, c++, s.AwayDangerousAttacks);
                    Set(r, c++, s.HomeShotsOnTarget);
                    Set(r, c++, s.AwayShotsOnTarget);
                    Set(r, c++, s.HomeShotsOffTarget);
                    Set(r, c++, s.AwayShotsOffTarget);
                    Set(r, c++, s.HomeYellowCards);
                    Set(r, c++, s.AwayYellowCards);
                    Set(r, c++, s.HomeRedCards);
                    Set(r, c++, s.AwayRedCards);
                    Set(r, c++, s.HomeCorners);
                    Set(r, c++, s.AwayCorners);

                    r++;
                }
                catch (Exception ex)
                {

                }
            }

            _currentWorkbook.SaveAs(destpath);
        }

        private void Set<T>(int row, int column, T value)
        {
            if (value == null)
                return;
            if (typeof(T) == typeof(double))
            {
                string text = value.ToString() ?? "0";
                double coef = text.ConvertToDouble();
                if (coef == 0)
                    return;
            }
            _currentWorksheet.Cell(row, column).SetValue(value);
        }

        private double GetCorrectScoreCoef(int home, int away, List<CorrectScoreMarket> markets)
        {
            var market = markets.Find(x => x.Home == home && x.Away == away);
            if (market is null)
                return 0;

            return market.Coefficient;
        }

        private double BothConvert(BothToScoreMarket market)
        {
            if (market == null)
                return 0;

            return market.Coefficient;
        }
        private List<double> GetWinCoefficients(List<WinMarket> markets)
        {
            var result = new List<double>();

            var home = markets.Find(x => x.Type == ETeamType.Home);
            var draw = markets.Find(x => x.Type == ETeamType.Draw);
            var away = markets.Find(x => x.Type == ETeamType.Away);

            result.Add((home != null ? home.Coefficient : 0));
            result.Add((draw != null ? draw.Coefficient : 0));
            result.Add((away != null ? away.Coefficient : 0));

            return result;
        }


        private List<double> GetDCCoefficients(List<DoubleChanceMarket> markets)
        {
            var result = new List<double>();

            var home = markets.Find(x => x.Type == EDoubleChanceType.WinHomeOrDraw);
            var draw = markets.Find(x => x.Type == EDoubleChanceType.WinHomeOrAway);
            var away = markets.Find(x => x.Type == EDoubleChanceType.WinAwayOrDraw);

            result.Add((home != null ? home.Coefficient : 0));
            result.Add((draw != null ? draw.Coefficient : 0));
            result.Add((away != null ? away.Coefficient : 0));

            return result;
        }
    }
}
