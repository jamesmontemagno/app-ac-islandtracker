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
            for (int i = 0; i < days.Count; i++)
            {
                Day day = days[i];
                if (!day.PriceAM.HasValue)
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: false);
                    day.PredictionAM = $"🔮 {min}-{max}";
                }
                else
                {
                    day.PredictionAM = string.Empty;
                }

                if (!day.PricePM.HasValue)
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: true);
                    day.PredictionPM = $"🔮 {min}-{max}";
                }
                else
                {
                    day.PredictionPM = string.Empty;
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
                    if (day.BuyPrice.HasValue)
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

                    if (day.PricePM.HasValue)
                    {
                        prices.Add(day.PricePM.Value);
                    }
                }
            }
            return prices.ToArray();
        }
    }
}
