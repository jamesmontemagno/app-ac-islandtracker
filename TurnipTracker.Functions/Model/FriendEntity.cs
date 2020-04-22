using System;
using Microsoft.Azure.Cosmos.Table;

namespace TurnipTracker.Shared
{
    public class FriendEntity : TableEntity
    {
        public FriendEntity(string requesterPublicKey, string requesteePublicKey)
        {
            PartitionKey = requesterPublicKey;
            RowKey = requesteePublicKey;
        }

        // PartitionKey
        public string RequesterPublicKey => PartitionKey;

        // RowKey
        public string RequesteePublicKey => RowKey;
    }
}
