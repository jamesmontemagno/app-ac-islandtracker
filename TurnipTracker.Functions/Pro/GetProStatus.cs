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
using System.Net.Http;
using System.Net;
using System.Text;
using TurnipTracker.Functions.Helpers;
using TurnipTracker.Shared;
using System.Linq;
using TurnipTracker.Functions.Model;

namespace TurnipTracker.Functions.Pro
{
    public static class GetProStatus
    {
        [FunctionName(nameof(GetProStatus))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProStatus/{myPublicKey}")] HttpRequest req,
            string myPublicKey,
            [Table("ProStatus")] CloudTable proTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(GetProStatus)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("You are not authorized to make this request.")
                };

            if (string.IsNullOrWhiteSpace(myPublicKey))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Invalid data to process request")
                };
            }



            ProStatus status = null;
            try
            {
                var publicKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, myPublicKey);
                var privateKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, privateKey);

                var rangeQuery = new TableQuery<ProStatusEntity>().Where(
                    TableQuery.CombineFilters(publicKeyFilter, TableOperators.And, privateKeyFilter));
                var entity = (await proTable.ExecuteQuerySegmentedAsync(rangeQuery, null)).FirstOrDefault();


                status = new ProStatus
                {
                    IsPro = entity != null,
                    Receipt = entity?.Receipt ?? string.Empty
                };

            }
            catch (Exception ex)
            {
                log.LogError($"Error {nameof(GetFriends)} - Error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            var json = JsonConvert.SerializeObject(status);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
