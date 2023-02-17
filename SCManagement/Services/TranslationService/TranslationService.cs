using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCManagement.Data;
using SCManagement.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace SCManagement.Services.TranslationService
{
    public class TranslationService : ITranslationService
    {
        private readonly string Endpoint;
        private readonly string SubscriptionKey;
        private readonly string Region;
        private readonly ApplicationDbContext _context;

        public TranslationService(ApplicationDbContext context, IConfiguration configuration)
        {
            SubscriptionKey = configuration["TranslatorAPIKey"];
            Region = configuration["TranslotarLocation"];
            Endpoint = configuration["TranslatorAPIEndpoint"];
            _context = context;
        }

        public async Task<int> Translate(IEnumerable<ClubTranslations> clubTranslations)
        {
            var remainTranslations = new List<ClubTranslations>(clubTranslations);
            foreach (ClubTranslations translations in clubTranslations)
            {
                if (translations.Value != null)
                {
                    remainTranslations.Remove(translations);
                    foreach (ClubTranslations _translations1 in remainTranslations)
                    {
                        if (_translations1.Value == null)
                            _translations1.Value = Translation(translations.Value, translations.Language, _translations1.Language).Result[0].Translations[0].Text;
                    }
                    return await _context.SaveChangesAsync();
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<List<TranslationsContainer>> Translation(string content, string fromLang, string toLang)
        {
            string route = "/translate?api-version=3.0&from=" + fromLang.Split("-")[0] + "&to=" + toLang.Split("-")[0];

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

                string jsonString = await response.Content.ReadAsStringAsync();

                // deserialize the JSON string into an object
                List<TranslationsContainer> containers = JsonConvert.DeserializeObject<List<TranslationsContainer>>(jsonString);

                return containers;
            };
        }
    }

    public class Translation
    {
        public string Text { get; set; }
        public string To { get; set; }
    }

    public class TranslationsContainer
    {
        public Translation[] Translations { get; set; }
    }
}
