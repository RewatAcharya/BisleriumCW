using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Bislerium.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index(int? year, int? month)
        {
            if(year == null)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }
            

            DashboardVM dashboard = new DashboardVM();
            List<LikeCountVM> dailyCount = new();

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Dashboard/GetDashboardData/{year}/{month}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dashboard = JsonConvert.DeserializeObject<DashboardVM>(apiResponse);
                }

                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Dashboard/LikeDislike/{year}/{month}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dailyCount = JsonConvert.DeserializeObject<List<LikeCountVM>>(apiResponse);
                }
            }

            ViewBag.DailyCount = dailyCount;
            ViewBag.Year = $"{year}-{month}-";
            return View(dashboard);
        }

        public async Task<IActionResult> TopTen(int? year, int? month)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }

            TopTen top = new TopTen();

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Dashboard/GetTopTen/{year}/{month}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    top = JsonConvert.DeserializeObject<TopTen>(apiResponse);
                }
            }

            return View(top);
        }
    }
}
