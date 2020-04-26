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
    public static class GetFriendRequests
    {
        [FunctionName(nameof(GetFriendRequests))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("FriendRequest")] CloudTable friendRequestTable,
            [Table("Friend")] CloudTable friendTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(SubmitFriendRequest)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            MyPendingFriendRequest friendRequest = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                friendRequest = JsonConvert.DeserializeObject<MyPendingFriendRequest>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to deserialize user: " + ex.Message);

            }

            if (friendRequest == null ||
                string.IsNullOrWhiteSpace(friendRequest.MyPublicKey))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = await Utils.FindUserEntity(userTable, privateKey, friendRequest.MyPublicKey);
                if (user == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.LogInformation("User doesn't exist: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            var requests = new List<PendingFriendRequest>();
            try
            {
                var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", 
                    QueryComparisons.Equal, friendRequest.MyPublicKey);

                var rangeQuery = new TableQuery<FriendRequestEntity>().Where(publicKeyFilter);

                var existingFriendRequests = await friendRequestTable.ExecuteQuerySegmentedAsync(rangeQuery, null);

                // spin off tasks for finding all friends
                var tasks = new List<Task<TableQuerySegment<UserEntity>>>();
                foreach (var f in existingFriendRequests)
                {
                    tasks.Add(Utils.FindFriendTask(userTable, f.RequesteePublicKey));
                }

                await Task.WhenAll(tasks);

                for (var i = 0; i < tasks.Count; i++)
                {
                    var t = tasks[i];
                    var friend = t.Result?.Results?.FirstOrDefault();
                    if (friend == null)
                        if (friend == null)
                        continue;
                    requests.Add(new PendingFriendRequest
                    {
                        CreationDateUTC = existingFriendRequests.ElementAt(i).Timestamp.UtcDateTime,
                        IslandName = friend.IslandName,
                        Name = friend.Name,
                        RequesterPublicKey = friend.PublicKey
                    });
                }

            }
            catch (Exception ex)
            {
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
