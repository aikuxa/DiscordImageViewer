using DiscordImageViewer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscordImageViewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly DiscordService _discordService;

        public HomeController(DiscordService discordService)
        {
            _discordService = discordService;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // Get image URLs from Discord
            var imageUrls = await _discordService.GetImageUrlsAsync();

            // Select a random image from the list
            var randomImageUrl = imageUrls.Any() ? imageUrls[new Random().Next(imageUrls.Count)] : null;

            // Return the random image URL to the view
            return View("Index", randomImageUrl);
        }

        // POST: Home/GetNewImage
        [HttpPost]
        public async Task<IActionResult> GetNewImage()
        {
            // Get image URLs from Discord
            var imageUrls = await _discordService.GetImageUrlsAsync();

            // Select a random image from the list
            var randomImageUrl = imageUrls.Any() ? imageUrls[new Random().Next(imageUrls.Count)] : null;

            // Return the random image URL as JSON
            return Json(new { imageUrl = randomImageUrl });
        }
    }
}
