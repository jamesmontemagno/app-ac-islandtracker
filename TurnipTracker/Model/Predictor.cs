using System;
using System.Collections.Generic;

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
            int buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            // High Phase 1
            for (var i = 2; i < 2 + highPhase1Len; i++)
            {
                double minPred = Math.Floor(0.9 * buyPrice);
                double maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length)
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
                double minPred = Math.Floor(minRate * buyPrice / 10000.0);
                double maxPred = Math.Ceiling(maxRate * buyPrice / 10000.0);


                if (i < givenPrices.Length)
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
                double minPred = Math.Floor(0.9 * buyPrice);
                double maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length)
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
                double minPred = Math.Floor(minRate * buyPrice / 10000);
                double maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                if (i < givenPrices.Length)
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
                double minPred = Math.Floor(0.9 * buyPrice);
                double maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length)
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
            return new PredictedPriceSeries("high, decreasing, high, decreasing, high", 0, predictedPrices);
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
            int buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            double minRate = 8500;
            double maxRate = 9000;

            for (var i = 2; i < peakStart; i++)
            {
                double minPred = Math.Floor(minRate * buyPrice / 10000.0);
                double maxPred = Math.Ceiling(maxRate * buyPrice / 10000.0);


                if (i < givenPrices.Length)
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
            double[] minRandoms = new double[] { 0.9, 1.4, 2.0, 1.4, 0.9, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4 };
            double[] maxRandoms = new double[] { 1.4, 2.0, 6.0, 2.0, 1.4, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9 };
            for (var i = peakStart; i < 14; i++)
            {
                double minPred = Math.Floor(minRandoms[i - peakStart] * buyPrice);
                double maxPred = Math.Ceiling(maxRandoms[i - peakStart] * buyPrice);

                if (i < givenPrices.Length)
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
            return new PredictedPriceSeries("decreasing, high spike, random lows", 1, predictedPrices);
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
            int buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            double minRate = 8500;
            double maxRate = 9000;
            for (var i = 2; i < 14; i++)
            {
                double minPred = Math.Floor(minRate * buyPrice / 10000);
                double maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                if (i < givenPrices.Length)
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
            yield return new PredictedPriceSeries("always decreasing", 2, predictedPrices);
        }

        static PredictedPriceSeries? GeneratePattern3WithPeak(
            int[] givenPrices,
            int peakStart)
        {
            int buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            double minRate = 4000;
            double maxRate = 9000;

            for (var i = 2; i < peakStart; i++)
            {
                double minPred = Math.Floor(minRate * buyPrice / 10000);
                double maxPred = Math.Ceiling(maxRate * buyPrice / 10000);


                if (i < givenPrices.Length)
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
                double minPred = Math.Floor(0.9 * buyPrice);
                double maxPred = Math.Ceiling(1.4 * buyPrice);
                if (i < givenPrices.Length)
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
                double minPred = Math.Floor(1.4 * buyPrice) - 1;
                double maxPred = Math.Ceiling(2.0 * buyPrice) - 1;
                if (peakStart + 2 < givenPrices.Length)
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
                double maxPred = Math.Ceiling(2.0 * buyPrice);
                if (peakStart + 3 < givenPrices.Length)
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
                double minPred = Math.Floor(1.4 * buyPrice) - 1;
                double maxPred = predictedPrices[peakStart + 3].max - 1;
                if (peakStart + 4 < givenPrices.Length)
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


                        if (i < givenPrices.Length)
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

            return new PredictedPriceSeries("decreasing, spike, decreasing", 3, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePattern3(int[] givenPrices)
        {
            for (int peakStart = 2; peakStart < 10; peakStart++)
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
                    int[] iterSellPrices = new int[sellPrices.Length];
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
            for (int i = 0; i < 14; i++)
            {
                int min = int.MaxValue;
                int max = 0;
                foreach (var entry in series)
                {
                    if (entry.Prices[i].min < min)
                        min = entry.Prices[i].min;
                    if (entry.Prices[i].max > max)
                        max = entry.Prices[i].max;
                }
                weekMinMaxs.Add((min, max));
            }
            return new PredictedPriceSeries("predicted min/max across all patterns", 4, weekMinMaxs);
        }
    }
}