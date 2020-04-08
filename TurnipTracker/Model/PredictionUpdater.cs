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
                if (string.IsNullOrEmpty(day.PriceAM))
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: false);
                    day.PredictionAM = $"🔮 {min}-{max}";
                }
                else
                {
                    day.PredictionAM = day.PriceAM;
                }

                if (string.IsNullOrEmpty(day.PricePM))
                {
                    var (min, max) = dailyMinMax.GetMinMax(i, isPM: true);
                    day.PredictionPM = $"🔮 {min}-{max}";
                }
                else
                {
                    day.PredictionPM = day.PricePM;
                }
            }
        }

        static int[] GetPricesFromDays(List<Day> days)
        {
            var prices = new List<int>();
            for (int i = 0; i < days.Count; i++)
            {
                if (days[i].IsSunday)
                {
                    if (!string.IsNullOrEmpty(days[i].PriceAM))
                    {
                        if (int.TryParse(days[i].PriceAM, out int price))
                        {
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
                        prices.Add(0);
                        prices.Add(0);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(days[i].PriceAM))
                    {
                        if (int.TryParse(days[i].PriceAM, out int price))
                        {
                            prices.Add(price);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(days[i].PricePM))
                    {
                        if (int.TryParse(days[i].PricePM, out int price))
                        {
                            prices.Add(price);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return prices.ToArray();
        }
    }
}
