using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using SCManagement.Models;

namespace SCManagement.Services.TranslationService
{
    public interface ITranslationService
    {
        public Task<List<TranslationsContainer>> Translation(string content, string fromLang, string toLang);

        public Task? Translate(IEnumerable<ITranslation> translations);
    }
}
