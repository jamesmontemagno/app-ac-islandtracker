using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Web.Http;
using TurnipTracker.Shared;
using TurnipTracker.Functions.Helpers;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Linq;

namespace TurnipTracker.Functions
{
    public static class GetFriendRequestCount
    {
        [FunctionName(nameof(GetFriendRequestCount))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetFriendRequestCount/{myPublicKey}")] HttpRequest req,
            string myPublicKey,
            [Table("FriendRequest")] CloudTable friendRequestTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(GetFriendRequestCount)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (string.IsNullOrWhiteSpace(myPublicKey))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = await Utils.FindUserEntitySlim(userTable, privateKey, myPublicKey);
                if (user == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.LogInformation("User doesn't exist: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            var requests = new FriendRequestCount();
            try
            {
                var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", 
                    QueryComparisons.Equal, myPublicKey);

                var rangeQuery = new TableQuery<FriendRequestEntity>().Where(publicKeyFilter).Select(new List<string> { "PartitionKey", "RowKey", "Timestamp" });

                var existingFriendRequests = await friendRequestTable.ExecuteQuerySegmentedAsync(rangeQuery, null);

                requests.Count =  existingFriendRequests.Count();

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error {nameof(GetFriendRequests)} - Error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            var json = JsonConvert.SerializeObject(requests);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
