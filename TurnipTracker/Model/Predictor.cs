using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace TurnipTracker.Model
{
    // C# version of https://github.com/mikebryant/ac-nh-turnip-prices/blob/master/js/predictions.js
    [SuppressMessage("Style", "IDE0007:Use implicit type", Justification = "Explicit types are useful when converting from dynamically typed JS")]
    public static class Predictor
    {
        // The reverse-engineered code is not perfectly accurate, especially as it's not
        // 32-bit ARM floating point. So, be tolerant of slightly unexpected inputs
        const double fudgeFactor = 5;

        static readonly Dictionary<PredictionPattern, int> patternCounts = new Dictionary<PredictionPattern, int>
        {
            { PredictionPattern.Fluctuating, 56 },
            { PredictionPattern.LargeSpike, 7 },
            { PredictionPattern.Decreasing, 1 },
            { PredictionPattern.SmallSpike, 8 },
        };

        static readonly Dictionary<PredictionPattern, Dictionary<PredictionPattern, double>> probabilityMatrix =
            new Dictionary<PredictionPattern, Dictionary<PredictionPattern, double>>
        {
            { PredictionPattern.Fluctuating ,  new Dictionary<PredictionPattern, double>
                {
                    { PredictionPattern.Fluctuating, 0.20 },
                    { PredictionPattern.LargeSpike,  0.30 },
                    { PredictionPattern.Decreasing,  0.15 },
                    { PredictionPattern.SmallSpike, 0.35 },
                }
            },
            { PredictionPattern.LargeSpike ,  new Dictionary<PredictionPattern, double>
                {
                    { PredictionPattern.Fluctuating, 0.50 },
                    { PredictionPattern.LargeSpike,  0.05 },
                    { PredictionPattern.Decreasing,  0.20 },
                    { PredictionPattern.SmallSpike, 0.25 },
                }
            },
            { PredictionPattern.Decreasing ,  new Dictionary<PredictionPattern, double>
                {
                    { PredictionPattern.Fluctuating, 0.25 },
                    { PredictionPattern.LargeSpike,  0.45 },
                    { PredictionPattern.Decreasing,  0.05 },
                    { PredictionPattern.SmallSpike, 0.25 },
                }
            },
            { PredictionPattern.SmallSpike ,  new Dictionary<PredictionPattern, double>
                {
                    { PredictionPattern.Fluctuating, 0.45 },
                    { PredictionPattern.LargeSpike,  0.25 },
                    { PredictionPattern.Decreasing,  0.15 },
                    { PredictionPattern.SmallSpike, 0.15 },
                }
            },
        };

        const double rateMultiplier = 10000;

        static int IntCeil(double val) => (int)Math.Truncate(val + 0.99999);

        static double MinimumRateFromGivenAndBase(int givenPrice, int buyPrice) => rateMultiplier * (givenPrice - 0.99999) / buyPrice;

        static double MaximumRateFromGivenAndBase(int givenPrice, int buyPrice) => rateMultiplier * (givenPrice + 0.00001) / buyPrice;

        static int GetPrice(double rate, int basePrice) => IntCeil(rate * basePrice / rateMultiplier);

        // This corresponds to the code:
        //   for (int i = start; i<start + length; i++)
        //   {
        //     sellPrices[work++] =
        //       intceil(randfloat(rateMin / RATE_MULTIPLIER, rateMax / RATE_MULTIPLIER) * basePrice);
        //   }
        //
        // Would modify the predictedPrices array.
        // If the givenPrices won't match, returns false, otherwise returns true
        static bool GenerateIndividualRandomPrice(
          int[] givenPrices, List<(int min, int max)> predictedPrices, int start, int length, double rateMin, double rateMax)
        {
            rateMin *= rateMultiplier;
            rateMax *= rateMultiplier;

            int buyPrice = givenPrices[0];

            for (int i = start; i < start + length; i++)
            {
                int minPred = GetPrice(rateMin, buyPrice);
                int maxPred = GetPrice(rateMax, buyPrice);

                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred - fudgeFactor || givenPrices[i] > maxPred + fudgeFactor)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return false;
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }

                predictedPrices.Add((minPred, maxPred));
            }
            return true;
        }

        // This corresponds to the code:
        //   rate = randfloat(start_rateMin, start_rateMax);
        //   for (int i = start; i<start + length; i++)
        //   {
        //     sellPrices[work++] = intceil(rate* basePrice);
        //     rate -= randfloat(rateDecayMin, rateDecayMax);
        //   }
        //
        // Would modify the predictedPrices array.
        // If the givenPrices won't match, returns false, otherwise returns true
        static bool GenerateDecreasingRandomPrice(
          int[] givenPrices, List<(int min, int max)> predictedPrices, int start, int length, double rateMin,
          double rateMax, double rateDecayMin, double rateDecayMax)
        {
            rateMin *= rateMultiplier;
            rateMax *= rateMultiplier;
            rateDecayMin *= rateMultiplier;
            rateDecayMax *= rateMultiplier;

            int buyPrice = givenPrices[0];

            for (int i = start; i < start + length; i++)
            {
                int minPred = GetPrice(rateMin, buyPrice);
                int maxPred = GetPrice(rateMax, buyPrice);
                if (i < givenPrices.Length && givenPrices[i] > 0)
                {
                    if (givenPrices[i] < minPred - fudgeFactor || givenPrices[i] > maxPred + fudgeFactor)
                    {
                        // Given price is out of predicted range, so this is the wrong pattern
                        return false;
                    }
                    if (givenPrices[i] >= minPred || givenPrices[i] <= maxPred)
                    {
                        // The value in the FUDGE_FACTOR range is ignored so the rate range would not be empty.
                        double realRateMin = MinimumRateFromGivenAndBase(givenPrices[i], buyPrice);
                        double realRateMax = MaximumRateFromGivenAndBase(givenPrices[i], buyPrice);
                        rateMin = Math.Max(rateMin, realRateMin);
                        rateMax = Math.Min(rateMax, realRateMax);
                    }
                    minPred = givenPrices[i];
                    maxPred = givenPrices[i];
                }

                predictedPrices.Add((minPred, maxPred));

                rateMin -= rateDecayMax;
                rateMax -= rateDecayMin;
            }
            return true;
        }

        // This corresponds to the code:
        //   rate = randfloat(rateMin, rateMax);
        //   sellPrices[work++] = intceil(randfloat(rateMin, rate) * basePrice) - 1;
        //   sellPrices[work++] = intceil(rate* basePrice);
        //   sellPrices[work++] = intceil(randfloat(rateMin, rate) * basePrice) - 1;
        //
        // Would modify the predictedPrices array.
        // If the givenPrices won't match, returns false, otherwise returns true
        static bool GeneratePeakPrice(
            int[] givenPrices, List<(int min, int max)> predictedPrices, int start, double rateMin, double rateMax)
        {
            rateMin *= rateMultiplier;
            rateMax *= rateMultiplier;

            int buyPrice = givenPrices[0];

            // Main spike 1
            int minPred = GetPrice(rateMin, buyPrice) - 1;
            int maxPred = GetPrice(rateMax, buyPrice) - 1;
            if (start < givenPrices.Length && givenPrices[start] > 0)
            {
                if (givenPrices[start] < minPred - fudgeFactor || givenPrices[start] > maxPred + fudgeFactor)
                {
                    // Given price is out of predicted range, so this is the wrong pattern
                    return false;
                }
                minPred = givenPrices[start];
                maxPred = givenPrices[start];
            }
            predictedPrices.Add((minPred, maxPred));

            // Main spike 2
            minPred = predictedPrices[start].min;
            maxPred = IntCeil(2.0 * buyPrice);
            if (start + 1 < givenPrices.Length && givenPrices[start + 1] > 0)
            {
                if (givenPrices[start + 1] < minPred - fudgeFactor || givenPrices[start + 1] > maxPred + fudgeFactor)
                {
                    // Given price is out of predicted range, so this is the wrong pattern
                    return false;
                }
                minPred = givenPrices[start + 1];
                maxPred = givenPrices[start + 1];
            }
            predictedPrices.Add((minPred, maxPred));

            // Main spike 3
            minPred = IntCeil(1.4 * buyPrice) - 1;
            maxPred = predictedPrices[start + 1].max - 1;
            if (start + 2 < givenPrices.Length && givenPrices[start + 2] > 0)
            {
                if (givenPrices[start + 2] < minPred - fudgeFactor || givenPrices[start + 2] > maxPred + fudgeFactor)
                {
                    // Given price is out of predicted range, so this is the wrong pattern
                    return false;
                }
                minPred = givenPrices[start + 2];
                maxPred = givenPrices[start + 2];
            }
            predictedPrices.Add((minPred, maxPred));

            return true;
        }

        static PredictedPriceSeries? GeneratePatternFluctuatingWithLengths(
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
            if (!GenerateIndividualRandomPrice(
                    givenPrices, predictedPrices, 2, highPhase1Len, 0.9, 1.4))
            {
                return null;
            }

            // Dec Phase 1
            if (!GenerateDecreasingRandomPrice(
                    givenPrices, predictedPrices, 2 + highPhase1Len, decPhase1Len,
                    0.6, 0.8, 0.04, 0.1))
            {
                return null;
            }

            // High Phase 2
            if (!GenerateIndividualRandomPrice(givenPrices, predictedPrices,
                2 + highPhase1Len + decPhase1Len, highPhase2Len, 0.9, 1.4))
            {
                return null;
            }

            // Dec Phase 2
            if (!GenerateDecreasingRandomPrice(
                    givenPrices, predictedPrices,
                    2 + highPhase1Len + decPhase1Len + highPhase2Len,
                    decPhase2Len, 0.6, 0.8, 0.04, 0.1))
            {
                return null;
            }

            // High Phase 3
            if (2 + highPhase1Len + decPhase1Len + highPhase2Len + decPhase2Len + highPhase3Len != 14)
            {
                throw new ApplicationException("Phase lengths don't add up");
            }

            int prevLength = 2 + highPhase1Len + decPhase1Len + highPhase2Len + decPhase2Len;
            if (!GenerateIndividualRandomPrice(
                     givenPrices, predictedPrices, prevLength, 14 - prevLength, 0.9,
                    1.4))
            {
                return null;
            }

            return new PredictedPriceSeries("Fluctuating", PredictionPattern.Fluctuating, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePatternFluctuating(int[] givenPrices)
        {
            for (var decPhase1Len = 2; decPhase1Len < 4; decPhase1Len++)
            {
                for (var highPhase1Len = 0; highPhase1Len < 7; highPhase1Len++)
                {
                    for (var highPhase3Len = 0; highPhase3Len < (7 - highPhase1Len - 1 + 1); highPhase3Len++)
                    {
                        var series = GeneratePatternFluctuatingWithLengths(givenPrices, highPhase1Len, decPhase1Len, 7 - highPhase1Len - highPhase3Len, 5 - decPhase1Len, highPhase3Len);
                        if (series != null)
                            yield return series;
                    }
                }
            }
        }

        static PredictedPriceSeries? GeneratePatternLargeSpikeWithPeak(
           int[] givenPrices,
           int peakStart)
        {

            int buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            if (!GenerateDecreasingRandomPrice(
                    givenPrices, predictedPrices, 2, peakStart - 2, 0.85, 0.9, 0.03, 0.05))
            {
                return null;
            }

            // Now each day is independent of next
            var minRandoms = new double[] { 0.9, 1.4, 2.0, 1.4, 0.9, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4 };
            var maxRandoms = new double[] { 1.4, 2.0, 6.0, 2.0, 1.4, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9 };
            for (int i = peakStart; i < 14; i++)
            {
                if (!GenerateIndividualRandomPrice(
                        givenPrices, predictedPrices, i, 1, minRandoms[i - peakStart], maxRandoms[i - peakStart]))
                {
                    return null;
                }
            }
            return new PredictedPriceSeries("Large spike", PredictionPattern.LargeSpike, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePatternLargeSpike(int[] givenPrices)
        {
            for (var peakStart = 3; peakStart < 10; peakStart++)
            {
                var series = GeneratePatternLargeSpikeWithPeak(givenPrices, peakStart);
                if (series != null)
                    yield return series;
            }
        }

        static IEnumerable<PredictedPriceSeries> GeneratePatternDecreasing(int[] givenPrices)
        {
            var buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            if (!GenerateDecreasingRandomPrice(
                    givenPrices, predictedPrices, 2, 14 - 2, 0.85, 0.9, 0.03, 0.05))
            {
                yield break;
            }

            yield return new PredictedPriceSeries("Decreasingg", PredictionPattern.Decreasing, predictedPrices);
        }

        static PredictedPriceSeries? GeneratePatternSmallSpikeWithPeak(
            int[] givenPrices,
            int peakStart)
        {
            var buyPrice = givenPrices[0];
            var predictedPrices = new List<(int min, int max)> { (buyPrice, buyPrice), (buyPrice, buyPrice) };

            if (!GenerateDecreasingRandomPrice(
                    givenPrices, predictedPrices, 2, peakStart - 2, 0.4, 0.9, 0.03,
                    0.05))
            {
                return null;
            }

            // The peak
            if (!GenerateIndividualRandomPrice(
                    givenPrices, predictedPrices, peakStart, 2, 0.9, 1.4))
            {
                return null;
            }

            if (!GeneratePeakPrice(
                    givenPrices, predictedPrices, peakStart + 2, 1.4, 2.0))
            {
                return null;
            }

            if (peakStart + 5 < 14)
            {
                if (!GenerateDecreasingRandomPrice(
                        givenPrices, predictedPrices, peakStart + 5,
                        14 - (peakStart + 5), 0.4, 0.9, 0.03, 0.05))
                {
                    return null;
                }
            }

            return new PredictedPriceSeries("Small spike", PredictionPattern.SmallSpike, predictedPrices);
        }

        static IEnumerable<PredictedPriceSeries> GeneratePatternSmallSpike(int[] givenPrices)
        {
            for (var peakStart = 2; peakStart < 10; peakStart++)
            {
                var series = GeneratePatternSmallSpikeWithPeak(givenPrices, peakStart);
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
                foreach (var series in GeneratePatternFluctuating(sellPrices))
                    yield return series;
                foreach (var series in GeneratePatternLargeSpike(sellPrices))
                    yield return series;
                foreach (var series in GeneratePatternDecreasing(sellPrices))
                    yield return series;
                foreach (var series in GeneratePatternSmallSpike(sellPrices))
                    yield return series;
            }
            return possibilities;
        }

        static double RowProbability(PredictedPriceSeries possibility, PredictionPattern previousPattern)
            => probabilityMatrix[previousPattern][possibility.PatternNumber] / patternCounts[possibility.PatternNumber];

        static List<PredictedPriceSeries> GetProbabilities(List<PredictedPriceSeries> possibilities, PredictionPattern previousPattern)
        {
            if (previousPattern == PredictionPattern.IDontKnow || previousPattern == PredictionPattern.All)
            {
                return possibilities;
            }

            var maxPercent = possibilities.Select((poss) => RowProbability(poss, previousPattern)).Sum();

            for (int i = 0; i < possibilities.Count; i++)
            {
                possibilities[i].Probability = RowProbability(possibilities[i], previousPattern) / maxPercent;
            }
            return possibilities;
        }

        public static List<PredictedPriceSeries> AnalyzePossibilities(int[] sellPrices, PredictionPattern previousPattern)
        {
            if (sellPrices == null) throw new ArgumentNullException(nameof(sellPrices));

            var generatedPossibilities = GeneratePossibilities(sellPrices);
            GetProbabilities(generatedPossibilities, previousPattern);

            var dailyMinMax = GetDailyMinMax(generatedPossibilities);
            generatedPossibilities.Add(dailyMinMax);

            return generatedPossibilities;
        }

        public static PredictedPriceSeries GetDailyMinMax(int[] sellPrices)
        {
            if (sellPrices == null) throw new ArgumentNullException(nameof(sellPrices));

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
