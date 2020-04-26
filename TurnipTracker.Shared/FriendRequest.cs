using System;
using System.Collections.Generic;
using System.Text;

namespace TurnipTracker.Shared
{
    public class FriendRequest
    {
        public string MyPublicKey { get; set; }
        public string FriendPublicKey { get; set; }
    }

    public class MyPendingFriendRequest
    {
        public string MyPublicKey { get; set; }
    }

    public class MyFriendsRequest
    {
        public string MyPublicKey { get; set; }
    }
}
