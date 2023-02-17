using Microsoft.AspNetCore.Mvc;
using SCManagement.Data;
using Newtonsoft.Json;
using System.Text;
using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Http.Headers;
using SCManagement.Services.TranslationService;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace SCManagement.Controllers
{
    public class TranslationController : Controller
    {
        private readonly ITranslationService _translationService;

        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        [HttpPost]
        public async Task<List<TranslationsContainer>> Translation(string content, string fromLang, string toLang)
        {
            return await _translationService.Translation(content, fromLang, toLang);
        }
    }
}
