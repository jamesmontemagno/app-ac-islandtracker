using System;
using System.Collections.Generic;

#nullable enable
namespace TurnipTracker.Model
{
    public static class Predictor
    {
        static double MinimumRateFromGivenAndBase(int givenPrice, int buyPrice) => 10000.0 * (givenPrice - 1) / buyPrice;

        static double MaximumRateFromGivenAndBase(int givenPrice, int buyPrice) => 10000.0 * givenPrice / buyPrice;

        static PredictedPriceSeries? GeneratePattern0WithLengths(
            int[] givenPrices,
            int highPhase1Len,
            int decPhase1Len,
            int highPhase2Len,
            int decPhase2Len,
            int highPhase3Len)
        {
            var buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            // High Phase 1
            for (var i = 2; i < 2 + highPhase1Len; i++)
            {
                var minPred = Math.Floor(0.9 * buyPrice);
                var maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));
            }

            // Dec Phase 1
            double minRate = 6000;
            double maxRate = 8000;
            for (var i = 2 + highPhase1Len; i < 2 + highPhase1Len + decPhase1Len; i++)
            {
                var minPred = Math.Floor(minRate * buyPrice / 10000.0);
                var maxPred = Math.Ceiling(maxRate * buyPrice / 10000.0);


                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                    minRate = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                    maxRate = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

                minRate -= 1000;
                maxRate -= 400;
            }

            // High Phase 2
            for (var i = 2 + highPhase1Len + decPhase1Len; i < 2 + highPhase1Len + decPhase1Len + highPhase2Len; i++)
            {
                var minPred = Math.Floor(0.9 * buyPrice);
                var maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));
            }

            // Dec Phase 2
            minRate = 6000;
            maxRate = 8000;
            for (var i = 2 + highPhase1Len + decPhase1Len + highPhase2Len; i < 2 + highPhase1Len + decPhase1Len + highPhase2Len + decPhase2Len; i++)
            {
                var minPred = Math.Floor(minRate * buyPrice / 10000);
                var maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                    minRate = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                    maxRate = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

                minRate -= 1000;
                maxRate -= 400;
            }

            // High Phase 3
            if (2 + highPhase1Len + decPhase1Len + highPhase2Len + decPhase2Len + highPhase3Len != 14)
            {
                throw new ApplicationException("Phase lengths don't add up");
            }
            for (var i = 2 + highPhase1Len + decPhase1Len + highPhase2Len + decPhase2Len; i < 14; i++)
            {
                var minPred = Math.Floor(0.9 * buyPrice);
                var maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

            }
            return new PredictedPriceSeries("Fluctuating", 0, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePattern0(int[] givenPrices)
        {
            for (var decPhase1Len = 2; decPhase1Len < 4; decPhase1Len++)
            {
                for (var highPhase1Len = 0; highPhase1Len < 7; highPhase1Len++)
                {
                    for (var highPhase3Len = 0; highPhase3Len < (7 - highPhase1Len - 1 + 1); highPhase3Len++)
                    {
                        var series = GeneratePattern0WithLengths(givenPrices, highPhase1Len, decPhase1Len, 7 - highPhase1Len - highPhase3Len, 5 - decPhase1Len, highPhase3Len);
                        if (series != null)
                            yield return series;
                    }
                }
            }
        }

        static PredictedPriceSeries? GeneratePattern1WithPeak(
           int[] givenPrices,
           int peakStart)
        {
            var buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            double minRate = 8500;
            double maxRate = 9000;

            for (var i = 2; i < peakStart; i++)
            {
                var minPred = Math.Floor(minRate * buyPrice / 10000.0);
                var maxPred = Math.Ceiling(maxRate * buyPrice / 10000.0);


                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                    minRate = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                    maxRate = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

                minRate -= 500;
                maxRate -= 300;
            }

            // Now each day is independent of next
            var minRandoms = new double[] { 0.9, 1.4, 2.0, 1.4, 0.9, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4 };
            var maxRandoms = new double[] { 1.4, 2.0, 6.0, 2.0, 1.4, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9 };
            for (var i = peakStart; i < 14; i++)
            {
                var minPred = Math.Floor(minRandoms[i - peakStart] * buyPrice);
                var maxPred = Math.Ceiling(maxRandoms[i - peakStart] * buyPrice);

                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

            }
            return new PredictedPriceSeries("Large spike", PredictionPattern.LargeSpike, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePattern1(int[] givenPrices)
        {
            for (var peakStart = 3; peakStart < 10; peakStart++)
            {
                var series = GeneratePattern1WithPeak(givenPrices, peakStart);
                if (series != null)
                    yield return series;
            }
        }

        static IEnumerable<PredictedPriceSeries> GeneratePattern2(int[] givenPrices)
        {
            var buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            double minRate = 8500;
            double maxRate = 9000;
            for (var i = 2; i < 14; i++)
            {
                var minPred = Math.Floor(minRate * buyPrice / 10000);
                var maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        yield break;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                    minRate = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                    maxRate = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

                minRate -= 500;
                maxRate -= 300;
            }
            yield return new PredictedPriceSeries("Decreasingg", PredictionPattern.Decreasing, predictedPrices);
        }

        static PredictedPriceSeries? GeneratePattern3WithPeak(
            int[] givenPrices,
            int peakStart)
        {
            var buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            double minRate = 4000;
            double maxRate = 9000;

            for (var i = 2; i < peakStart; i++)
            {
                var minPred = Math.Floor(minRate * buyPrice / 10000);
                var maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                    minRate = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                    maxRate = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                }

                predictedPrices.Add(((int)minPred, (int)maxPred));

                minRate -= 500;
                maxRate -= 300;
            }

            // The peak

            for (var i = peakStart; i < peakStart + 2; i++)
            {
                var minPred = Math.Floor(0.9 * buyPrice);
                var maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }
                predictedPrices.Add(((int)minPred, (int)maxPred));
            }

            // Main spike 1
            {
                var minPred = Math.Floor(1.4 * buyPrice) - 1;
                var maxPred = Math.Ceiling(2.0 * buyPrice) - 1;
                if (peakStart + 2 < givenPrices.Length && givenPrices[peakStart + 2] > 0)
                {
                    if (givenPrices[peakStart + 2] < minPred || givenPrices[peakStart + 2] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[peakStart + 2];
                    maxPred = givenPrices[peakStart + 2];
                }
                predictedPrices.Add(((int)minPred, (int)maxPred));
            }

            // Main spike 2
            {
                double minPred = predictedPrices[peakStart + 2].min;
                var maxPred = Math.Ceiling(2.0 * buyPrice);
                if (peakStart + 3 < givenPrices.Length && givenPrices[peakStart + 3] > 0)
                {
                    if (givenPrices[peakStart + 3] < minPred || givenPrices[peakStart + 3] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[peakStart + 3];
                    maxPred = givenPrices[peakStart + 3];
                }
                predictedPrices.Add(((int)minPred, (int)maxPred));
            }

            // Main spike 3
            {
                var minPred = Math.Floor(1.4 * buyPrice) - 1;
                double maxPred = predictedPrices[peakStart + 3].max - 1;
                if (peakStart + 4 < givenPrices.Length && givenPrices[peakStart + 4] > 0)
                {
                    if (givenPrices[peakStart + 4] < minPred || givenPrices[peakStart + 4] > maxPred)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return null;
                    }
                    minPred = givenPrices[peakStart + 4];
                    maxPred = givenPrices[peakStart + 4];
                }
                predictedPrices.Add(((int)minPred, (int)maxPred));

                if (peakStart + 5 < 14)
                {
                    minRate = 4000;
                    maxRate = 9000;

                    for (var i = peakStart + 5; i < 14; i++)
                    {
                        minPred = Math.Floor(minRate * buyPrice / 10000);
                        maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                        if (i < givenPrices.Length && givenPrices[i] > 0)
                        {
                            if (givenPrices[i] < minPred || givenPrices[i] > maxPred)
                            {
                                // Given price is out of predicted range, so this is the wrong pattern
                                return null;
                            }
                            minPred = givenPrices[i];
                            maxPred = givenPrices[i];
                            minRate = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                            maxRate = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                        }
                        predictedPrices.Add(((int)minPred, (int)maxPred));

                        minRate -= 500;
                        maxRate -= 300;
                    }
                }
            }

            return new PredictedPriceSeries("Small spike", PredictionPattern.SmallSpike, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePattern3(int[] givenPrices)
        {
            for (var peakStart = 2; peakStart < 10; peakStart++)
            {
                var series = GeneratePattern3WithPeak(givenPrices, peakStart);
                if (series != null)
                    yield return series;
            }
        }

        static List<PredictedPriceSeries> GeneratePossibilities(int[] sellPrices)
        {
            var possibilities = new List<PredictedPriceSeries>();
            if (sellPrices[0] > 0)
            {
                possibilities.AddRange(Enumerate(sellPrices));
            }
            else
            {
                for (var buyPrice = 90; buyPrice <= 110; buyPrice++)
                {
                    // Create a copy so as not to mutate passed in values
                    var iterSellPrices = new int[sellPrices.Length];
                    Array.Copy(sellPrices, iterSellPrices, sellPrices.Length);
                    iterSellPrices[0] = iterSellPrices[1] = buyPrice;
                    possibilities.AddRange(Enumerate(iterSellPrices));
                }
            }
            static IEnumerable<PredictedPriceSeries> Enumerate(int[] sellPrices)
            {
                foreach (var series in GeneratePattern0(sellPrices))
                    yield return series;
                foreach (var series in GeneratePattern1(sellPrices))
                    yield return series;
                foreach (var series in GeneratePattern2(sellPrices))
                    yield return series;
                foreach (var series in GeneratePattern3(sellPrices))
                    yield return series;
            }
            return possibilities;
        }

        public static List<PredictedPriceSeries> AnalyzePossibilities(int[] sellPrices)
        {
            var generatedPossibilities = GeneratePossibilities(sellPrices);

            var dailyMinMax = GetDailyMinMax(generatedPossibilities);
            generatedPossibilities.Add(dailyMinMax);

            return generatedPossibilities;
        }

        public static PredictedPriceSeries GetDailyMinMax(int[] sellPrices)
        {
            var generatedPossibilities = GeneratePossibilities(sellPrices);
            var dailyMinMax = GetDailyMinMax(generatedPossibilities);
            return dailyMinMax;
        }

        static PredictedPriceSeries GetDailyMinMax(List<PredictedPriceSeries> series)
        {
            var weekMinMaxs = new List<(int min, int max)>();
            for (var i = 0; i < 14; i++)
            {
                int min;
                int max;
                if (series.Count == 0)
                {
                    // If no series found then return 0 to 999
                    // This happens when the input data is invalid
                    min = 0;
                    max = 999;
                }
                else
                {
                    min = int.MaxValue;
                    max = 0;
                    foreach (var entry in series)
                    {
                        if (entry.Prices[i].min < min)
                            min = entry.Prices[i].min;
                        if (entry.Prices[i].max > max)
                            max = entry.Prices[i].max;
                    }
                }
                weekMinMaxs.Add((min, max));
            }
            return new PredictedPriceSeries("All patterns", PredictionPattern.All, weekMinMaxs);
        }
    }
}