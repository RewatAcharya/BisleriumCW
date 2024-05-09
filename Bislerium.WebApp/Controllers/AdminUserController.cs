using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.Users;
using Bislerium.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Week21.Domain;

namespace Bislerium.WebApp.Controllers
{
    public class AdminUserController : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? role = null)
        {
            List<UserVM> allUsers = new List<UserVM>();

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Account/GetUsers"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    allUsers = JsonConvert.DeserializeObject<List<UserVM>>(apiResponse);
                }
            }

            if (!string.IsNullOrEmpty(role))
            {
                allUsers = allUsers.Where(x => x.Role == role).ToList();
            }

            return View(allUsers);
        }

        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Register register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(register), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7281/api/Account/registerUser", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "New Admin added successfully";
                    }
                    else
                    {
                        TempData["error"] = "There is an error try again.";
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
