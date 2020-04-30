using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace TurnipTracker.Model
{
    public static class PredictionUpdater
    {
        internal static (int minSell, int maxSell) Update(List<Day> days)
        {
            var sellPrices = GetPricesFromDays(days);
            var firstBuy = sellPrices[0] == 0;
            var dailyMinMax = GetDailyMinMax(sellPrices, firstBuy);
            for (var i = 0; i < days.Count; i++)
            {
                var day = days[i];
                if (!day.PriceAM.HasValue)
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: false);
                    if(min == 999 && max == 0)
                    {
                        min = 0;
                        max = 999;
                    }
                    if (min == max)
                        day.PredictionAM = $"🔮 {min}";
                    else if (min == 999 || max == 0 || min == 0 || max == 999)
                        day.PredictionAM = string.Empty;
                    else
                        day.PredictionAM = $"🔮 {min}-{max}";

                    day.PredictionAMMin = min;
                    day.PredictionAMMax = max;
                }
                else
                {
                    day.PredictionAM = string.Empty;
                    day.PredictionAMMin = 0;
                    day.PredictionAMMax = 0;
                }

                if (!day.PricePM.HasValue)
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: true);
                    if (min == 999 && max == 0)
                    {
                        min = 0;
                        max = 999;
                    }
                    if (min == max)
                        day.PredictionPM = $"🔮 {min}";
                    else if (min == 999 || max == 0 || min == 0 || max == 999)
                        day.PredictionAM = string.Empty;
                    else
                        day.PredictionPM = $"🔮 {min}-{max}";

                    day.PredictionPMMin = min;
                    day.PredictionPMMax = max;
                }
                else
                {
                    day.PredictionPM = string.Empty;
                    day.PredictionAMMin = 0;
                    day.PredictionAMMax = 0;
                }
            }
            return (dailyMinMax.WeekGuaranteedMinimum, dailyMinMax.WeekMax);
        }

        static PredictedPriceSeries GetDailyMinMax(int[] sellPrices, bool firstBuy)
        {
            if (sellPrices == null) 
                throw new ArgumentNullException(nameof(sellPrices));

            var predictor = new Predictor(sellPrices, firstBuy, PredictionPattern.IDontKnow);
            var generatedPossibilities = predictor.AnalyzePossibilities();
            var dailyMinMax = GetDailyMinMax(generatedPossibilities);
            return dailyMinMax;
        }

        static PredictedPriceSeries GetDailyMinMax(List<PredictedPriceSeries> allSeries)
        {
            foreach (var series in allSeries)
            {
                if (series.PatternNumber == PredictionPattern.All)
                    return series;
            }
            throw new ApplicationException("No All Patterns series found");
        }

        static int[] GetPricesFromDays(List<Day> days)
        {
            var prices = new List<int>();
            foreach (var day in days)
            {
                if (day.IsSunday)
                {
                    if (day.BuyPrice.HasValue && !day.FirstPurchase)
                    {
                        var price = day.BuyPrice.Value;

                        prices.Add(price);
                        prices.Add(price);
                    }
                    else
                    {
                        prices.Add(0);
                        prices.Add(0);
                    }
                }
                else
                {
                    if (day.PriceAM.HasValue)
                    {
                        prices.Add(day.PriceAM.Value);
                    }
                    else
                    {
                        prices.Add(0);
                    }

                    if (day.PricePM.HasValue)
                    {
                        prices.Add(day.PricePM.Value);
                    }
                    else
                    {
                        prices.Add(0);
                    }
                }
            }

            // Trim zero entries at end
            var lastNonZero = prices.FindLastIndex((x) => x > 0);
            // There must be two entries for Sunday even if both zero
            if (lastNonZero < 2)
                lastNonZero = 1;
            prices.RemoveRange(lastNonZero + 1, prices.Count - (lastNonZero + 1));
            return prices.ToArray();
        }
    }
}
