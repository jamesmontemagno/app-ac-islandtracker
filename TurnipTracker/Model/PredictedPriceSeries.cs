using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable enable
namespace TurnipTracker.Model
{
    public sealed class PredictedPriceSeries
    {
        public string PatternDesc { get; }
        public PredictionPattern PatternNumber { get; }
        public List<(int min, int max)> Prices { get; }
        public double Probability { get; set; }

        public int WeekGuaranteedMinimum { get; set; }
        public int WeekMax { get; set; }

        public double CategoryTotalProbability { get; set; }

        public PredictedPriceSeries(string patternDesc, PredictionPattern patternNumber, List<(int min, int max)> prices, double probability = 0)
        {
            PatternDesc = patternDesc;
            PatternNumber = patternNumber;
            Prices = prices;
            Probability = probability;
        }

        internal (int min, int max) GetMinMax(int i, bool isPM)
        {
            var index = i * 2;
            if (isPM) index++;
            return Prices[index];
        }

        [Conditional("DEBUG")]
        public void Dump()
        {
            Console.Write($"{PatternDesc,20}");
            Console.Write($"{FmtPct(CategoryTotalProbability)}");
            Console.Write($"{FmtPct(Probability)}");
            for (var index = 1; index < Prices.Count; index++)
            {
                var minPrice = Prices[index].min;
                var maxPrice = Prices[index].max;
                string prices;
                if (minPrice == maxPrice)
                    prices = $"{minPrice}";
                else
                    prices = $"{minPrice}..{maxPrice}";
                Console.Write($"{prices,10}");
            }
            Console.Write($"{WeekGuaranteedMinimum,10}");
            Console.Write($"{WeekMax,10}");
            Console.WriteLine();

            static string FmtPct(double value)
            {
                if (value < 0.1)
                    return $"{value,10:0.00%}";
                return $"{value,10:0.0%}";
            }
        }

        public override string ToString() => PatternDesc;
    }
}