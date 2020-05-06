using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Table;
using TurnipTracker.Shared;

namespace TurnipTracker.Functions.Helpers
{
    public static class Utils
    {
        public static string FriendCodePassword => Environment.GetEnvironmentVariable("FRIEND_CODE_PASS");
        public static string ParseToken(HttpRequest request)
        {
            var header = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
            var authHeader = header.Parameter;

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
//#if DEBUG
                //return authHeader;
//#endif
                var s = encoding.GetString(Convert.FromBase64String(authHeader));
                if (!Guid.TryParse(s, out _))
                    return null;

                return s;
            }

            return null;
        }

        public static Task<TableQuerySegment<UserEntity>> FindFriendTask(CloudTable cloudTable, string friendPublicKey)
        {
            var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, friendPublicKey);

            var rangeQuery = new TableQuery<UserEntity>().Where(publicKeyFilter);
            return cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null);
        }

        public static async Task<UserEntity> FindFriend(CloudTable cloudTable, string friendPublicKey)
        {            
            return (await FindFriendTask(cloudTable, friendPublicKey)).FirstOrDefault();
        }

        public static async Task<UserEntity> FindUserEntitySlim(CloudTable cloudTable, string privateKey, string publicKey)
        {
            var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, publicKey);
            var privateKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, privateKey);

            var rangeQuery = new TableQuery<UserEntity>()
                .Where(TableQuery.CombineFilters(publicKeyFilter, TableOperators.And, privateKeyFilter))
                .Select(new List<string> { "PartitionKey", "RowKey", "Timestamp" }); 
            return (await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null)).FirstOrDefault();
        }

        public static async Task<UserEntity> FindUserEntity(CloudTable cloudTable, string privateKey, string publicKey)
        {
            var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, publicKey);
            var privateKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, privateKey);

            var rangeQuery = new TableQuery<UserEntity>().Where(
                TableQuery.CombineFilters(publicKeyFilter, TableOperators.And, privateKeyFilter));
            return (await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null)).FirstOrDefault();
        }

        public static async Task<bool> ReachedMaxFriends(CloudTable couldTable, string publicKey)
        {
            var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, publicKey);

            var rangeQuery = new TableQuery<FriendEntity>().Where(publicKeyFilter);
            var friends = await couldTable.ExecuteQuerySegmentedAsync(rangeQuery, null);
            var count = friends.Count();
            var max = int.Parse(Environment.GetEnvironmentVariable("MAX_FRIENDS"));
            return count >= max;
        }

        public static async Task<bool> HasFriend(CloudTable cloudTable, string requesterKey, string requesteeKey)
        {
            var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, requesterKey);
            var privateKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, requesteeKey);

            var rangeQuery = new TableQuery<FriendEntity>().Where(
                TableQuery.CombineFilters(publicKeyFilter, TableOperators.And, privateKeyFilter));
            return (await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null)).Any();
        }

        public static async Task<UserEntity> MergeUserEntity(CloudTable cloudTable, UserEntity userEntity)
        {
            // Create the InsertOrReplace table operation
            var insertOperation = TableOperation.Merge(userEntity);

            // Execute the operation.
            var result = await cloudTable.ExecuteAsync(insertOperation);
            var mergedUser = result.Result as UserEntity;
            return mergedUser;
        }
    }
}
