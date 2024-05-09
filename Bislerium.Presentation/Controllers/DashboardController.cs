using Bislerium.Application.IServices;
using Bislerium.Infrastructure.Services.BlogService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboard;
        private readonly ITopTenService _topTen;

        public DashboardController(IDashboardService dashboard, ITopTenService topTen)
        {
            _dashboard = dashboard;
            _topTen = topTen;
        }

        [HttpGet, Route("GetDashboardData/{year}/{month}")]
        public async Task<IActionResult> GetData(int year, int month)
        {
            var result = await _dashboard.GetDashboardData(year, month);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("LikeDislike/{year}/{month}")]
        public async Task<IActionResult> GetLikeDislike(int year, int month)
        {
            var result = await _dashboard.ChartLikeDislikes(year, month);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetTopTen/{year}/{month}")]
        public async Task<IActionResult> GetTopTen(int year, int month)
        {
            var result = await _topTen.GetTopTenData(year, month);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
