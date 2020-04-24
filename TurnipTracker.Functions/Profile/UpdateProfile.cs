using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TurnipTracker.Functions.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using TurnipTracker.Shared;
using System.Linq;
using System.Web.Http;

namespace TurnipTracker.Functions
{
    public static class UpdateProfile
    {
        [FunctionName(nameof(UpdateProfile))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [Table("User")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(UpdateProfile)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new UnauthorizedResult();

            User user = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                user = JsonConvert.DeserializeObject<User>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to deserialize user: " + ex.Message);

            }

            if (user == null ||
                string.IsNullOrWhiteSpace(user.PublicKey) ||
                string.IsNullOrWhiteSpace(user.Name) ||
                string.IsNullOrWhiteSpace(user.IslandName) ||
                string.IsNullOrWhiteSpace(user.TimeZone))
            {
                return new BadRequestResult();
            }

            UserEntity userEntity = null;
            try
            {
                var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user.PublicKey);
                var privateKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, privateKey);

                var rangeQuery = new TableQuery<UserEntity>().Where(
                    TableQuery.CombineFilters(publicKeyFilter, TableOperators.And,privateKeyFilter));
                userEntity = (await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null)).FirstOrDefault();
            }
            catch(Exception ex)
            {
                //user does not exist? correct error?
                return new InternalServerErrorResult();
            }


            userEntity.Name = user.Name;
            userEntity.IslandName = user.IslandName;
            userEntity.Fruit = user.Fruit;
            userEntity.TimeZone = user.TimeZone;
            userEntity.Status = user.Status ?? string.Empty;

            try
            {
                // Create the InsertOrReplace table operation
                var insertOperation = TableOperation.Merge(userEntity);

                // Execute the operation.
                var result = await cloudTable.ExecuteAsync(insertOperation);
                var insertedCustomer = result.Result as UserEntity;

            }
            catch (Exception ex)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("User Updated");
        }
    }
}
