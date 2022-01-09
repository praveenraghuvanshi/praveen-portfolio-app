using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Portfolio.Function
{
    public class FeedbackRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Application { get; set; }
    }

    public class Feedback
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string FeedbackId => RowKey;

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Application => PartitionKey;
    }

    public static class contact_api
    {
        [FunctionName("contact_api")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "contact")] HttpRequest req,
            [Table("feedback", "Application", "FeedbackId"), StorageAccount("AZURE_STORAGE_CONNECTION_STRING")] IAsyncCollector<Feedback> feedbackCollector,
            ILogger log)
        {
            log.LogInformation("Inside Function!!!");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
            var feedbackRequestData = (FeedbackRequest)JsonConvert.DeserializeObject<FeedbackRequest>(requestBody);
            var application = (string)feedbackRequestData.Application;
            log.LogInformation(application);


            var feedback = new Feedback
            {
                PartitionKey = feedbackRequestData.Application,
                RowKey = Guid.NewGuid().ToString(),
                Name = feedbackRequestData.Name,
                Email = feedbackRequestData.Email,
                Message = feedbackRequestData.Message
            };

            await feedbackCollector.AddAsync(feedback);
            await feedbackCollector.FlushAsync();

            return new OkObjectResult(feedback);
        }
    }
}
