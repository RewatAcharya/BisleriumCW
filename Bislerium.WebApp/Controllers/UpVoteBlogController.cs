using Bislerium.Domain.Entity.Blogs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Bislerium.WebApp.Controllers
{
    public class UpVoteBlogController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create(LikeBlog vote)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                StringContent content = new StringContent(JsonConvert.SerializeObject(vote), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7281/api/UpVoteBlog/Reaction", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (apiresponse == "true")
                    {
                        TempData["success"] = (vote.Reaction == Domain.Enums.ReactionType.UpVote) ? "Upvote Successfully added" : "Downvote Successfully added";
                    }
                    else
                    {
                        TempData["error"] = "Removed successful.";
                    }
                }
            }
            return RedirectToAction("Index", "Blog");
        }
    }
}
