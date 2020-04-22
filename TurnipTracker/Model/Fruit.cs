using System;
namespace TurnipTracker.Model
{
    public enum Fruit
    {
        Apple,
        Cherry,
        Orange,
        Peach,
        Pear
    }

    public class FruitItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
