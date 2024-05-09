using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using Bislerium.Domain.Entity.Users;
using Bislerium.Domain.ViewModels;
using Bislerium.WebApp.HubService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace Bislerium.WebApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public CommentController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(Guid id, string? userId = null)
        {
            Blog blog = new();
            ApplicationUser user = new();
            List<Comment> comment = new();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Blog/GetBlog/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }

                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Comment/GetComments"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    comment = JsonConvert.DeserializeObject<List<Comment>>(apiResponse);
                }
                if (userId != null)
                {
                    using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Account/GetUser/{userId}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<ApplicationUser>(apiResponse);
                    }
                }

            }

            ViewBag.LoggedUser = user;
            ViewBag.Comments = comment.Where(x => x.CommentBlog == id).ToList();
            return View(blog);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListAllComments()
        {
            List<Comment> comment = new();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Comment/GetComments"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    comment = JsonConvert.DeserializeObject<List<Comment>>(apiResponse);
                }
            }
            return View(comment);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid id)
        {
            var blogdetails = new List<CommentHistory>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Comment/GetCommentDetail/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogdetails = JsonConvert.DeserializeObject<List<CommentHistory>>(apiResponse);
                }
            }
            return View(blogdetails);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Create(Comment comment)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7281/api/Comment/AddComment", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Comment added successfully";

                        await _hubContext.Clients.All.SendAsync("displayNotification", "");
                    }
                    else
                    {
                        TempData["error"] = "Please. Try again.";
                    }
                }
            }
            return RedirectToAction("Index", "Comment", new { id = comment.CommentBlog, userId = comment.CommentUser });
        }

        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Update(Guid id)
        {
            Comment comment = new();

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Comment/GetComment/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    comment = JsonConvert.DeserializeObject<Comment>(apiResponse);
                }
            }
            return PartialView(comment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Update(Comment comment)
        {

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync($"https://localhost:7281/api/Comment/UpdateComment", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Comment updated successfully";
                    }
                    else
                    {
                        TempData["error"] = "Please. Try again.";
                    }
                }
            }
            return RedirectToAction("Index", "Comment", new { id = comment.CommentBlog, userId = comment.CommentUser });
        }

        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Comment comment = new();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Comment/GetComment/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    comment = JsonConvert.DeserializeObject<Comment>(apiResponse);
                }

                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.DeleteAsync($"https://localhost:7281/api/Comment/DeleteComment/{id}"))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Comment deleted successfully";
                    }
                    else
                    {
                        TempData["error"] = "Please. Try again.";
                    }
                }
            }
            return RedirectToAction("Index", "Comment", new { id = comment.CommentBlog, userId = comment.CommentUser });
        }
    }
}
