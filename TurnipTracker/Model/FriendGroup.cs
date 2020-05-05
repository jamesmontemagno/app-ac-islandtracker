using System.Collections.Generic;
using MvvmHelpers;
using TurnipTracker.Shared;

namespace TurnipTracker.Model
{
    public class FriendGroup : Grouping<string, FriendStatus>
    {
        public FriendGroup(string key, IEnumerable<FriendStatus> items) : base(key, items)
        {
        }
    }
}
