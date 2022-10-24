using LogViewer.Models;
using LogViewer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogViewer.WebAPI.Controllers
{
    [ApiController]
    public class LogContentController : Controller
    {
        private readonly LogFetcherService _logFetcherService;

        public LogContentController(LogFetcherService logFetcherService)
        {
            _logFetcherService = logFetcherService;
        }

        [HttpPost("FetchLogContent")]
        public async Task<IActionResult> FetchLogContent(LogDirectoryConfiguration configuration)
        {
            var logContent = await _logFetcherService.GetContentsWithFormatAsync(configuration);
            return Ok(logContent);
        }

    }
}
