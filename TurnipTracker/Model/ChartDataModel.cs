using System;
using System.Collections.Generic;
using System.Text;

namespace TurnipTracker.Model
{
    public class ChartDataModel
    {
        public string Name { get; set; }

        public double Value { get; set; }

        public ChartDataModel(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
