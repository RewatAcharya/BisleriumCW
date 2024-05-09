using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using Week21.Domain;

namespace Bislerium.WebApp.Controllers
{
    public class ProfileController : Controller
    {

        private readonly IWebHostEnvironment _env;

        public ProfileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<IActionResult> Index(string id)
        {
            ApplicationUser? user = new();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Account/GetUser/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<ApplicationUser>(apiResponse);
                }
            }
            return View(user);
        }

        public IActionResult ChangePassword() { return View(); }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM vm, string userId)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vm), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync($"https://localhost:7281/api/Account/UpdatePassword/{userId}", content))
                    {
                        string apiresponse = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["success"] = "Password change successfully";
                        }
                        else
                        {
                            TempData["error"] = "May be password did not matched. Try again.";
                        }
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> EditProfile(string id)
        {
            ApplicationUser? user = new();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Account/GetUser/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<ApplicationUser>(apiResponse);
                }
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser appUser, IFormFile? ProfileUpload, IFormFile? CoverUpload)
        {
            if (ModelState.IsValid)
            {
                if (ProfileUpload != null)
                {
                    string profileName = Guid.NewGuid() + Path.GetExtension(ProfileUpload.FileName);
                    string profilePath = Path.Combine(_env.WebRootPath, @"Images\Profiles\", profileName);
                    using (FileStream stream = new FileStream(profilePath, FileMode.Create))
                    {
                        ProfileUpload.CopyTo(stream);
                    }
                    appUser.ProfileUrl = profileName;
                }
                if (CoverUpload != null)
                {
                    string coverName = Guid.NewGuid() + Path.GetExtension(CoverUpload.FileName);
                    string coverPath = Path.Combine(_env.WebRootPath, @"Images\Covers\", coverName);
                    using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                    {
                        CoverUpload.CopyTo(stream);
                    }
                    appUser.CoverUrl = coverName;
                }
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(appUser), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync($"https://localhost:7281/api/Account/EditProfile", content))
                    {
                        string apiresponse = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["success"] = "Profile changes successfully";
                        }
                        else
                        {
                            TempData["error"] = "May be there is an issues. Try again.";
                        }
                    }
                }

            }
            return RedirectToAction("Index", "Profile", new { id = appUser.Id });
        }

        [Authorize(Roles = "Admin, Blogger")]
        public IActionResult Delete(string id)
        {
            ViewBag.Id = id;
            return PartialView();
        }


        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.DeleteAsync($"https://localhost:7281/api/Account/DeleteUser/{id}"))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                }
            }
            TempData["success"] = "Profile deleted. Please visit again.";
            Response.Cookies.Delete("access_token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Blog");
        }
    }
}
