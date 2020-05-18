using System;
namespace TurnipTracker.Model
{
    public enum Fruit
    {
        Apple = 0,
        Cherry = 1,
        Orange = 2,
        Peach = 3,
        Pear = 4
    }

    public class FruitItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
