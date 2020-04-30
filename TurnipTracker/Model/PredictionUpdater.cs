using System;
using System.Collections.Generic;
using System.Text;

namespace TurnipTracker.Model
{
    public static class PredictionUpdater
    {
        internal static void Update(List<Day> days)
        {
            var sellPrices = GetPricesFromDays(days);
            var dailyMinMax = Predictor.GetDailyMinMax(sellPrices);
            for (var i = 0; i < days.Count; i++)
            {
                var day = days[i];
                if (!day.PriceAM.HasValue)
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: false);
                    if (min == max)
                        day.PredictionAM = $"🔮 {min}";
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
                    if (min == max)
                        day.PredictionPM = $"🔮 {min}";
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