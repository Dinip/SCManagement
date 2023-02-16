using Microsoft.AspNetCore.Mvc;
using SCManagement.Data;
using Newtonsoft.Json;
using System.Text;
using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Http.Headers;

namespace SCManagement.Controllers
{
    public class TranslationController : Controller
    {
        private readonly string Endpoint;
        private readonly string SubscriptionKey ;
        private readonly string Region;

        public TranslationController(IConfiguration configuration)
        {
            SubscriptionKey = configuration["TranslatorAPIKey"];
            Region = configuration["TranslotarLocation"];
            Endpoint = configuration["TranslatorAPIEndpoint"];
        }

        public async Task<string> Translation(string content, string fromLang, string toLang)
        {
            string route = "/translate?api-version=3.0&from=" + fromLang.Split("-")[0] +"&to=" + toLang.Split("-")[0];

            object[] body = new object[] { new { Text = content } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(Endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", Region);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                
                return await response.Content.ReadAsStringAsync();
            };
        }
    }
}
