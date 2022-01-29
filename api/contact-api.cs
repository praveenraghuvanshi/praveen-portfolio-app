using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;

namespace Portfolio.Function
{
    public class FeedbackRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Application { get; set; }

        public string Token { get; set; }
    }

    public class Feedback
    {
        public string FeedbackId => Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Application { get; set; }
    }

    public class RecapatchaResponse
    {
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTimeOffset ChallengeTs { get; set; }

        public string Hostname { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }

    public static class contact_api
    {
        [FunctionName("contact_api")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "contact")] HttpRequest req,
            [ServiceBus("feedbackqueue", Microsoft.Azure.WebJobs.ServiceBus.ServiceBusEntityType.Queue, Connection = "AZURE_SERVICE_BUS_CS")] IAsyncCollector<string> feedbackCollector, //StorageAccount("AZURE_SERVICE_BUS_CONNECTION_STRING")] IAsyncCollector<Feedback> feedbackCollector,
            ILogger log)
        {
            log.LogInformation("Inside Function!!!");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var feedbackRequestData = (FeedbackRequest)JsonConvert.DeserializeObject<FeedbackRequest>(requestBody);

            /* this is the payload I send from my site */
            if (string.IsNullOrWhiteSpace(feedbackRequestData.Name)) return new BadRequestResult();
            if (string.IsNullOrWhiteSpace(feedbackRequestData.Email)) return new BadRequestResult();
            if (string.IsNullOrWhiteSpace(feedbackRequestData.Subject)) return new BadRequestResult();
            if (string.IsNullOrWhiteSpace(feedbackRequestData.Token)) return new BadRequestResult();

            // Validate captcha
            var captchaResponse = await ValidateCaptchaAsync(req, feedbackRequestData.Token);
            if (!captchaResponse.Success)
            {
                log.LogInformation($"reCAPTCHA verification failed.");
                return new BadRequestObjectResult(new[] { "reCAPTCHA verification failed." });
            }

            var feedback = new Feedback
            {
                Name = feedbackRequestData.Name,
                Email = feedbackRequestData.Email,
                Subject = feedbackRequestData.Subject,
                Message = feedbackRequestData.Message,
                Application = feedbackRequestData.Application
            };

            await feedbackCollector.AddAsync(JsonConvert.SerializeObject(feedback));
            await feedbackCollector.FlushAsync();

            return new OkObjectResult(feedback);
        }

        private static async Task<RecapatchaResponse> ValidateCaptchaAsync(HttpRequest req, string token)
        {
            // validate recaptcha token
            using (var client = new HttpClient())
            {
                var query = new QueryBuilder();
                query.Add("secret", Environment.GetEnvironmentVariable("RECAPTCHA_SECRET_KEY"));
                query.Add("response", token);
                query.Add("remoteIp", req.HttpContext.Connection.RemoteIpAddress.ToString());

                var recaptchaUri = new UriBuilder("https://www.google.com/recaptcha/api/siteverify");
                recaptchaUri.Query = query.ToString();

                var request = new HttpRequestMessage(HttpMethod.Post, recaptchaUri.ToString());

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return new RecapatchaResponse { Success = false, ErrorCodes = { response.StatusCode.ToString() } };  // recaptcha rejected our request
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<RecapatchaResponse>(responseString);

                return responseData;
            }
        }
    }
}
