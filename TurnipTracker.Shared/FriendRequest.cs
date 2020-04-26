using System;
using System.Collections.Generic;
using System.Text;

namespace TurnipTracker.Shared
{
    public class FriendRequest
    {
        // Person submitting the friend request
        public string RequesterPublicKey { get; set; }
        // Friend that needs to approve it.
        public string RequesteePublicKey { get; set; }
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
