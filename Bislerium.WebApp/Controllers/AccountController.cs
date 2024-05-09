using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Statics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Week21.Domain;
using Microsoft.AspNetCore.Identity;
using Bislerium.Domain.Entity.Users;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Bislerium.Domain.Entity.History;
using System.Reflection.Metadata;
using Bislerium.Domain.ViewModels;
using Microsoft.AspNetCore.SignalR;


namespace Bislerium.WebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult ResetPassword() { return View(); }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"https://localhost:7281/api/Account/ResetEmail/{email}", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Link sent successfully";
                    }
                    else
                    {
                        TempData["error"] = "There is an error try again.";
                    }
                }
            }
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ResetPasswordConfirm(string email, string token) 
        {
            var sendRequest = new TokenVM
            {
                Email = email,
                Token = token
            };

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(sendRequest), Encoding.UTF8, "application/json");


                using (var response = await httpClient.PostAsync($"https://localhost:7281/api/Account/GetToken/{email}", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<TokenVM>(apiResponse);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["error"] = "Link already expired try reseting again";
                        return RedirectToAction("Login");
                    }
                    ViewBag.Email = responseResult.Email;
                    ViewBag.Token = responseResult.Token;

                }
            }
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordModel model)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"https://localhost:7281/api/Account/ResetPassword", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Password Reset successfully";
                    }
                    else
                    {
                        TempData["error"] = "There is an error try again.";
                    }
                }
            }
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            TempData["error"] = "Logged out. Please visit again.";
            Response.Cookies.Delete("access_token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Blog");
        }

        public IActionResult Register()
        { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Week21.Domain.Register register)
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
                        TempData["success"] = "Register successful";
                    }
                    else
                    {
                        TempData["error"] = "There is an error try again.";
                    }
                }
            }
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7281/api/Account/LoginUser", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);
                    // var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(apiResponse);

                    string message = responseObject.Message;
                    //string message = responseObject["message"];

                    if (message == "Login completed")
                    {
                        TempData["success"] = "Login Completed";

                        string token = responseObject.Token;
                       // string token = responseObject["token"];

                        var handler = new JwtSecurityTokenHandler();
                        var jwtSecurityToken = handler.ReadJwtToken(token);
                        var claims = jwtSecurityToken.Claims.ToList();

                        SaveClaimsInCookie(claims);
                        SaveJwtInCookie(token);

                        if (claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                    else
                    {
                        TempData["error"] = "Login did not completed";
                        return View(login);
                    }
                }
            }
            return RedirectToAction("Index", "Blog");
        }

        private void SaveJwtInCookie(string jwt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, 
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(1) 
            };

            Response.Cookies.Append("access_token", jwt, cookieOptions);
        }

        private void SaveClaimsInCookie(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true, 
                IsPersistent = true, 
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1) 
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        [HttpPost]
        public async Task<IActionResult> SaveToken([FromBody] CreateToken createToken)
        {
            //CreateToken createToken = new CreateToken()
            //{
            //    Token = token,
            //    UserId = userId,
            //};

            using (var httpClient = new HttpClient())
            {
               
                StringContent content = new StringContent(JsonConvert.SerializeObject(createToken), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7281/api/Account/SaveFCMToken", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                }
            }
            return Json(createToken.Token);
        }
    }
}
