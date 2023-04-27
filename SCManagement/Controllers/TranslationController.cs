using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services.TranslationService;


namespace SCManagement.Controllers
{
    [Authorize]
    public class TranslationController : Controller
    {
        private readonly ITranslationService _translationService;

        /// <summary>
        /// Translations controller constructor, injects all the services needed
        /// </summary>
        /// <param name="translationService"></param>
        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }


        /// <summary>
        /// Translate the content from a language to another
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fromLang"></param>
        /// <param name="toLang"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TranslationsContainer>> Translation(string content, string fromLang, string toLang)
        {
            return await _translationService.Translation(content, fromLang, toLang);
        }
    }
}
