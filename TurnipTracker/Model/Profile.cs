using System;
using MvvmHelpers;
using Newtonsoft.Json;

namespace TurnipTracker.Model
{
    public class Profile : ObservableObject
    {
        [JsonIgnore]
        public Action SaveProfileAction { get; set; }

        string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value, onChanged: SaveProfileAction);
        }

        string islandName = string.Empty;
        public string IslandName
        {
            get => islandName;
            set => SetProperty(ref islandName, value, onChanged: SaveProfileAction);
        }

        int fruit = (int)Model.Fruit.Apple;
        public int Fruit
        {
            get => fruit;
            set => SetProperty(ref fruit, value, onChanged: SaveProfileAction);
        }
    }
}
