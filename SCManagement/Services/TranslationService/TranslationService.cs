using Newtonsoft.Json;
using SCManagement.Models;
using System.Text;

namespace SCManagement.Services.TranslationService
{
    public class TranslationService : ITranslationService
    {
        private readonly string _endpoint;
        private readonly string _subscriptionKey;
        private readonly string _region;

        public TranslationService(IConfiguration configuration)
        {
            _subscriptionKey = configuration["TranslatorAPIKey"];
            _region = configuration["TranslatorLocation"];
            _endpoint = configuration["TranslatorAPIEndpoint"];
        }

        public Task? Translate(IEnumerable<ITranslation> translations)
        {
            if (translations == null) return Task.CompletedTask;

            var translation = translations.FirstOrDefault(x => x.Value != "" && x.Value != null);

            if (translation == null) return Task.CompletedTask;

            var remainTranslations = translations.Where(x => x.Value == "" || x.Value == null).ToList();

            foreach (var t in remainTranslations)
            {
                t.Value = Translation(translation.Value, translation.Language, t.Language).Result[0].Translations[0].Text;
            }

            return Task.CompletedTask;
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
                request.RequestUri = new Uri(_endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _region);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                string jsonString = await response.Content.ReadAsStringAsync();

                // deserialize the JSON string into an object
                List<TranslationsContainer> containers = JsonConvert.DeserializeObject<List<TranslationsContainer>>(jsonString);

                return containers;
            }
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
