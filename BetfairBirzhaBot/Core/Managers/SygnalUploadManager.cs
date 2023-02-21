using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Models;
using Microsoft.Office.Interop.Excel;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Core
{
    public class SygnalUploadManager
    {
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
        private double BothConvert(BothToScoreMarket market)
        {
            if (market == null)
                return 0;

            return market.Coefficient;
        }

        public async Task Upload(List<StrategySygnalResult> list, string path)
        {
            try
            {
                List<string> allRows = new();
                int row = 1;

                var firstLine = new List<string>
                {
                    "Название стратегии",
                    "Игра",
                    "Лига",
                    "Ссылка",
                    "Счёт 1",
                    "Счёт 2",
                    "Минута",
                    "П1 старт",
                    "Х старт",
                    "П2 старт",
                    "П1 текущий",
                    "Х текущий",
                    "П2 текущий",
                    "ОЗ ДА старт",
                    "ОЗ НЕТ старт",
                    "ОЗ ДА текущий",
                    "ОЗ НЕТ текущий",
                };

                for (double i = 0.5; i < 6.5; i += 1)
                {
                    firstLine.Add($"ТБ {i} вся игра старт");
                    firstLine.Add($"ТМ {i} вся игра старт");
                }

                for (double i = 0.5; i < 6.5; i += 1)
                {
                    firstLine.Add($"ТБ {i} вся игра лайв");
                    firstLine.Add($"ТМ {i} вся игра лайв");
                }

                for (double i = 0.5; i < 6.5; i += 1)
                {
                    firstLine.Add($"ТБ {i} тайм старт");
                    firstLine.Add($"ТМ {i} тайм старт");
                }

                for (double i = 0.5; i < 6.5; i += 1)
                {
                    firstLine.Add($"ТБ {i} тайм лайв");
                    firstLine.Add($"ТМ {i} тайм лайв");
                }

                firstLine.Add($"АТК 1");
                firstLine.Add($"АТК 2");
                firstLine.Add($"ОП АТК 1");
                firstLine.Add($"ОП АТК 2");
                firstLine.Add($"В СТВОР 1");
                firstLine.Add($"В СТВОР 2");
                firstLine.Add($"В СТОРОНУ 1");
                firstLine.Add($"В СТОРОНУ 2");
                firstLine.Add($"ЖЕЛТЫЕ КАРТОЧКИ 1");
                firstLine.Add($"ЖЕЛТЫЕ КАРТОЧКИ 2");
                firstLine.Add($"КРАСНЫЕ КАРТОЧКИ 1");
                firstLine.Add($"КРАСНЫЕ КАРТОЧКИ 2");

                if (!File.Exists(Path.Combine(path, "result.csv")))
                    allRows.Add(String.Join(';', firstLine));

                foreach (var sygnal in list)
                {
                    List<string> cswLine = new();
                    int column = 0;
                    try
                    {
                        var homeWinStart = sygnal.Game.WinMarketsStartGame.Find(x => x.Type == Common.Entities.ETeamType.Home);

                        var startWinCoefs = GetWinCoefficients(sygnal.Game.WinMarketsStartGame);
                        var currentWinCoefs = GetWinCoefficients(sygnal.Game.WinMarketsCurrent);

                        cswLine.Add(sygnal.StrategyName); 
                        cswLine.Add(sygnal.Game.Title);
                        cswLine.Add(sygnal.Game.League);
                        cswLine.Add(sygnal.Game.Url);
                        cswLine.Add(sygnal.Game.Teams[0].Score.ToString());
                        cswLine.Add(sygnal.Game.Teams[1].Score.ToString());
                        cswLine.Add(sygnal.Game.ElapsedMinutes.ToString());
                        cswLine.Add(startWinCoefs[0].ToString());
                        cswLine.Add(startWinCoefs[1].ToString());
                        cswLine.Add(startWinCoefs[2].ToString());

                        cswLine.Add(currentWinCoefs[0].ToString());
                        cswLine.Add(currentWinCoefs[1].ToString());
                        cswLine.Add(currentWinCoefs[2].ToString());

                        cswLine.Add(BothConvert(sygnal.Game.BothToScoreMarketsStart.Find(x => x.Type == Common.Entities.EBothToScoreType.Yes)).ToString());
                        cswLine.Add(BothConvert(sygnal.Game.BothToScoreMarketsStart.Find(x => x.Type == Common.Entities.EBothToScoreType.No)).ToString());
                        cswLine.Add(BothConvert(sygnal.Game.BothToScoreMarketsCurrent.Find(x => x.Type == Common.Entities.EBothToScoreType.Yes)).ToString());
                        cswLine.Add(BothConvert(sygnal.Game.BothToScoreMarketsCurrent.Find(x => x.Type == Common.Entities.EBothToScoreType.No)).ToString());

                        int totalCounter = 0;
                        for (double i = 0.5; i < 6.5; i += 1)
                        {
                            var total = sygnal.Game.TotalMarketsStart.Find(x => x.Parameter == i);
                            if (total == null)
                            {
                                column += 2;
                                continue;
                            }

                            cswLine.Add(total.Over.Coefficient.ToString());
                            cswLine.Add(total.Under.Coefficient.ToString());
                        }

                        for (double i = 0.5; i < 6.5; i += 1)
                        {
                            var total = sygnal.Game.TotalMarketsCurrent.Find(x => x.Parameter == i);
                            if (total == null)
                            {
                                totalCounter += 2;
                                continue;
                            }

                            cswLine.Add(total.Over.Coefficient.ToString());
                            cswLine.Add(total.Under.Coefficient.ToString());
                        }

                        for (double i = 0.5; i < 6.5; i += 1)
                        {
                            var total = sygnal.Game.TotalMarketsFirstHalfStart.Find(x => x.Parameter == i);
                            if (total == null)
                            {
                                totalCounter += 2;
                                continue;
                            }

                            cswLine.Add(total.Over.Coefficient.ToString());
                            cswLine.Add(total.Under.Coefficient.ToString());
                        }

                        for (double i = 0.5; i < 6.5; i += 1)
                        {
                            var total = sygnal.Game.TotalMarketsFirstHalfCurrent.Find(x => x.Parameter == i);
                            if (total == null)
                            {
                                totalCounter += 2;
                                continue;
                            }

                            cswLine.Add(total.Over.Coefficient.ToString());
                            cswLine.Add(total.Under.Coefficient.ToString());
                        }

                        var s = sygnal.Game.Statistics.Statistics;

                        firstLine.Add(s.HomeAttacks.ToString());
                        firstLine.Add(s.AwayAttacks.ToString());
                        firstLine.Add(s.HomeDangerousAttacks.ToString());
                        firstLine.Add(s.AwayDangerousAttacks.ToString());
                        firstLine.Add(s.HomeShotsOnTarget.ToString());
                        firstLine.Add(s.AwayShotsOnTarget.ToString());
                        firstLine.Add(s.HomeShotsOffTarget.ToString());
                        firstLine.Add(s.AwayShotsOnTarget.ToString());
                        firstLine.Add(s.HomeYellowCards.ToString());
                        firstLine.Add(s.AwayYellowCards.ToString());
                        firstLine.Add(s.HomeRedCards.ToString());
                        firstLine.Add(s.AwayRedCards.ToString());


                        allRows.Add(string.Join(';', cswLine));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                File.AppendAllLines(Path.Combine(path, "result.csv"), allRows, Encoding.UTF8);
                
            }
            catch (Exception ex)
            {

            }

        }

        public async Task UploadSygnals(List<StrategySygnalResult> list)
        {
            //var wb = new Workbook();
            //var ws = wb.Worksheets.Create("Results");
            //int row = 1;

            //foreach (var sygnal in list)
            //{
            //    int column = 0;
            //    try
            //    {
            //        var homeWinStart = sygnal.Game.WinMarketsStartGame.Find(x => x.Type == Common.Entities.ETeamType.Home);

            //        var startWinCoefs = GetWinCoefficients(sygnal.Game.WinMarketsStartGame);
            //        var currentWinCoefs = GetWinCoefficients(sygnal.Game.WinMarketsCurrent);

            //        ws.Rows[row].Columns[column++].Text = sygnal.StrategyName;
            //        ws.Rows[row].Columns[column++].Text = sygnal.Game.Title;
            //        ws.Rows[row].Columns[column++].NumberValue = sygnal.Game.Teams[0].Score;
            //        ws.Rows[row].Columns[column++].NumberValue = sygnal.Game.Teams[1].Score;
            //        ws.Rows[row].Columns[column++].Text = sygnal.Game.Url;
            //        ws.Rows[row].Columns[column++].NumberValue = sygnal.Game.ElapsedMinutes;
            //        ws.Rows[row].Columns[column++].NumberValue = startWinCoefs[0];
            //        ws.Rows[row].Columns[column++].NumberValue = startWinCoefs[1];
            //        ws.Rows[row].Columns[column++].NumberValue = startWinCoefs[2];

            //        ws.Rows[row].Columns[column++].NumberValue = currentWinCoefs[0];
            //        ws.Rows[row].Columns[column++].NumberValue = currentWinCoefs[1];
            //        ws.Rows[row].Columns[column++].NumberValue = currentWinCoefs[2];

            //        ws.Rows[row].Columns[column++].NumberValue = BothConvert(sygnal.Game.BothToScoreMarketsStart.Find(x => x.Type == Common.Entities.EBothToScoreType.Yes));
            //        ws.Rows[row].Columns[column++].NumberValue = BothConvert(sygnal.Game.BothToScoreMarketsStart.Find(x => x.Type == Common.Entities.EBothToScoreType.No));
            //        ws.Rows[row].Columns[column++].NumberValue = BothConvert(sygnal.Game.BothToScoreMarketsCurrent.Find(x => x.Type == Common.Entities.EBothToScoreType.Yes));
            //        ws.Rows[row].Columns[column++].NumberValue = BothConvert(sygnal.Game.BothToScoreMarketsCurrent.Find(x => x.Type == Common.Entities.EBothToScoreType.No));

            //        int totalCounter = 0;
            //        for (double i = 0.5; i < 6.5; i += 1)
            //        {
            //            var total = sygnal.Game.TotalMarketsStart.Find(x => x.Parameter == i);
            //            if (total == null)
            //            {
            //                column += 2;
            //                continue;
            //            }

            //            ws.Rows[row].Columns[column++].NumberValue = total.Over.Coefficient;
            //            ws.Rows[row].Columns[column++].NumberValue = total.Under.Coefficient;
            //        }

            //        for (double i = 0.5; i < 6.5; i += 1)
            //        {
            //            var total = sygnal.Game.TotalMarketsCurrent.Find(x => x.Parameter == i);
            //            if (total == null)
            //            {
            //                totalCounter += 2;
            //                continue;
            //            }

            //            ws.Rows[row].Columns[column++].NumberValue = total.Over.Coefficient;
            //            ws.Rows[row].Columns[column++].NumberValue = total.Under.Coefficient;
            //        }

            //        for (double i = 0.5; i < 6.5; i += 1)
            //        {
            //            var total = sygnal.Game.TotalMarketsFirstHalfStart.Find(x => x.Parameter == i);
            //            if (total == null)
            //            {
            //                totalCounter += 2;
            //                continue;
            //            }

            //            ws.Rows[row].Columns[column++].NumberValue = total.Over.Coefficient;
            //            ws.Rows[row].Columns[column++].NumberValue = total.Under.Coefficient;
            //        }

            //        for (double i = 0.5; i < 6.5; i += 1)
            //        {
            //            var total = sygnal.Game.TotalMarketsFirstHalfCurrent.Find(x => x.Parameter == i);
            //            if (total == null)
            //            {
            //                totalCounter += 2;
            //                continue;
            //            }

            //            ws.Rows[row].Columns[column++].NumberValue = total.Over.Coefficient;
            //            ws.Rows[row].Columns[column++].NumberValue = total.Under.Coefficient;
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

            //ws.SaveToFile("test.xls", ";");
        }

    }
}
