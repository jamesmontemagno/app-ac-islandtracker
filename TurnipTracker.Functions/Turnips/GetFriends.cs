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
using TurnipTracker.Functions.Helpers;
using System.Net.Http;
using System.Net;
using TurnipTracker.Shared;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TurnipTracker.Functions
{
    public static class GetFriends
    {
        [FunctionName(nameof(GetFriends))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetFriends/{myPublicKey}")] HttpRequest req,
            string myPublicKey,
            [Table("Friend")] CloudTable friendTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(SubmitFriendRequest)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (string.IsNullOrWhiteSpace(myPublicKey))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = await Utils.FindUserEntity(userTable, privateKey, myPublicKey);
                if (user == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.LogInformation("User doesn't exist: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            var statuses = new List<FriendStatus>();
            try
            {
                var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, myPublicKey);

                var rangeQuery = new TableQuery<FriendEntity>().Where(publicKeyFilter);

                var friends = await friendTable.ExecuteQuerySegmentedAsync(rangeQuery, null);

                // spin off tasks for finding all friends
                var tasks = new List<Task<TableQuerySegment<UserEntity>>>();
                foreach(var f in friends)
                {
                    tasks.Add(Utils.FindFriendTask(userTable, f.RequesteePublicKey));
                }

                await Task.WhenAll(tasks);

                foreach (var t in tasks)
                {
                    var friend = t.Result?.Results?.FirstOrDefault();
                    if (friend == null)
                        continue;
                    statuses.Add(new FriendStatus
                    {
                        AMPrice = friend.AMPrice,
                        BuyPrice = friend.BuyPrice,
                        Fruit = friend.Fruit,
                        IslandName = friend.IslandName,
                        Name = friend.Name,
                        PMPrice = friend.PMPrice,
                        PublicKey = friend.PublicKey,
                        Status = friend.Status,
                        TimeZone = friend.TimeZone,
                        TurnipUpdateDayOfYear = friend.TurnipUpdateDayOfYear,
                        TurnipUpdateTimeUTC = friend.TurnipUpdateTimeUTC,
                        TurnipUpdateYear = friend.TurnipUpdateYear
                    });
                }

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error {nameof(GetFriends)} - Error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            var json = JsonConvert.SerializeObject(statuses);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
