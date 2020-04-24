using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TurnipTracker.Shared
{
    public class FriendEntity : TableEntity
    {
        public FriendEntity()
        {

        }
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
