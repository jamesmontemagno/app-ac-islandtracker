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

namespace TurnipTracker.Functions
{
    public static class RemoveFriend
    {
        [FunctionName(nameof(RemoveFriend))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req,
            [Table("Friend")] CloudTable friendTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(RemoveFriend)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new UnauthorizedResult();

            Friend friend = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                friend = JsonConvert.DeserializeObject<Friend>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to deserialize user: " + ex.Message);

            }

            if (friend == null ||
                string.IsNullOrWhiteSpace(friend.MyPublicKey) ||
                string.IsNullOrWhiteSpace(friend.FriendPublicKey))
            {
                return new BadRequestResult();
            }

            try
            {
                var user = await Utils.FindUserEntity(userTable, privateKey, friend.MyPublicKey);
                if(user == null)
                    return new BadRequestResult();
            }
            catch (Exception ex)
            {
                log.LogInformation("User doesn't exist: " + ex.Message);
                return new BadRequestResult();
            }


            try
            {
                var requester = friend.MyPublicKey;
                var requestee = friend.FriendPublicKey;

                
                var removeOperation1 = TableOperation.Delete(new FriendRequestEntity(requester, requestee));
                var removeOperation2 = TableOperation.Delete(new FriendRequestEntity(requestee, requester));
                var batch2 = new TableBatchOperation();
                batch2.Add(removeOperation1);
                batch2.Add(removeOperation2);

                // Execute the operation.
                var result = await friendTable.ExecuteBatchAsync(batch2);
                if (result == null || result.Count != 2)
                    return new InternalServerErrorResult();
            }
            catch (Exception ex)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("Friend Request Created");
        }
    }
}
