using Microsoft.AspNetCore.Mvc;

using Touchsides_Assignment.Middleware;
using Touchsides_Assignment.Services.Interfaces;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly ILogger<HomeController> _logger;
        private readonly ITextReaderService _textReaderService;

        public HomeController(IWebHostEnvironment webHost, ILogger<HomeController> logger, ITextReaderService textReaderService)
        {
            _webHost = webHost;
            _logger = logger;
            _textReaderService = textReaderService;
        }

        public IActionResult Index()
        {
            ViewBag.HasAdditionalInfo = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentNullException(nameof(file), "No file was uploaded");
                }

                string fileFolder = Path.Combine(_webHost.WebRootPath, "uploads");

                if (!Directory.Exists(fileFolder))
                {
                    Directory.CreateDirectory(fileFolder);
                }

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(fileFolder, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                ViewBag.Message = fileName + " File uploaded";
               string fileContent = await System.IO.File.ReadAllTextAsync(filePath);

                var result = _textReaderService.GetMostFrequentWord(fileContent);
                ViewBag.MostFrequentWord = result.Word;
                ViewBag.MostFrequentWordCount = result.Count;

                var mostFrequent7CharWordResult = _textReaderService.GetMostFrequent7CharacterWord(fileContent);
                ViewBag.MostFrequent7CharWord = mostFrequent7CharWordResult.Word;
                ViewBag.MostFrequent7CharWordCount = mostFrequent7CharWordResult.Count;

                var highestScoringWordResult = _textReaderService.GetHighestScoringWords(fileContent);
                ViewBag.HighestScoringWord = string.Join(", ", highestScoringWordResult.Words);
                ViewBag.HighestScoringWordScore = highestScoringWordResult.Score;

                ViewBag.HasAdditionalInfo = true;
                return View();
            }


            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                await GlobalExceptionHandler.HandleExceptionAsync(HttpContext, ex);

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}