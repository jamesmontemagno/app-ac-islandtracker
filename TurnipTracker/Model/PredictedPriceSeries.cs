using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TurnipTracker.Model
{
    public sealed class PredictedPriceSeries
    {
        public string PatternDesc { get; }
        public PredictionPattern PatternNumber { get; }
        public List<(int min, int max)> Prices { get; }

        public PredictedPriceSeries(string patternDesc, PredictionPattern patternNumber, List<(int min, int max)> prices)
        {
            this.PatternDesc = patternDesc;
            this.PatternNumber = patternNumber;
            this.Prices = prices;
        }

        public int WeeklyMin
        {
            get
            {
                var min = int.MaxValue;
                for (var i = 2; i < this.Prices.Count; i++)
                {
                    var entry = this.Prices[i];
                    if (entry.min < min)
                        min = entry.min;
                }
                return min;
            }
        }

        public int WeeklyMax
        {
            get
            {
                var max = 0;
                for (var i = 2; i < this.Prices.Count; i++)
                {
                    var entry = this.Prices[i];
                    if (entry.max > max)
                        max = entry.max;
                }
                return max;
            }
        }

        internal (int min, int max) GetMinMax(int i, bool isPM)
        {
            var index = i * 2;
            if (isPM) index++;
            return this.Prices[index];
        }

        [Conditional("DEBUG")]
        public void Dump()
        {
            Console.Write($"{PatternDesc,40}");
            for (var index = 1; index < this.Prices.Count; index++)
            {
                var minPrice = this.Prices[index].min;
                var maxPrice = this.Prices[index].max;
                string prices;
                if (minPrice == maxPrice)
                    prices = $"{minPrice}";
                else
                    prices = $"{minPrice}..{maxPrice}";
                Console.Write($"{prices,10}");
            }
            Console.Write($"{this.WeeklyMin,10}");
            Console.Write($"{this.WeeklyMax,10}");
            Console.WriteLine();
        }

        public override string ToString() => this.PatternDesc;
    }
}
