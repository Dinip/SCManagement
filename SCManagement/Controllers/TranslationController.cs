using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services.TranslationService;


namespace SCManagement.Controllers
{
    [Authorize]
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
