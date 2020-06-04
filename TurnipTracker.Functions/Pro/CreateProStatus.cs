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
using TurnipTracker.Shared;
using System.Web.Http;
using TurnipTracker.Functions.Model;

namespace TurnipTracker.Functions.Pro
{
    public static class CreateProStatus
    {
        [FunctionName(nameof(CreateProStatus))]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
             [Table("ProStatus")] CloudTable proTable,
             ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(CreateProStatus)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new UnauthorizedResult();

            ProStatus status = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                status = JsonConvert.DeserializeObject<ProStatus>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError("Unable to deserialize pro status: " + ex.Message);
            }

            if (status == null ||
                string.IsNullOrWhiteSpace(status.PublicKey) ||
                string.IsNullOrWhiteSpace(status.Receipt) ||
                !status.IsPro)
            {
                return new BadRequestErrorMessageResult("Invalid data to process request");
            }
           
            var proEntity = new ProStatusEntity(status.PublicKey, privateKey)
            {
                Receipt = status.Receipt
            };

            try
            {
                // Create the InsertOrReplace table operation
                var insertOrMergeOperation = TableOperation.InsertOrMerge(proEntity);

                // Execute the operation.
                var result = await proTable.ExecuteAsync(insertOrMergeOperation);
                var insertedPro = result.Result as ProStatusEntity;

            }
            catch (Exception ex)
            {
                log.LogError($"Error {nameof(CreateProStatus)} - Error: " + ex.Message);
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("Pro Status Created");
        }
    }
}
