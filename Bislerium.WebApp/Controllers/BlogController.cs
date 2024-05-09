using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using Bislerium.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;

namespace Bislerium.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public BlogController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private async Task<List<Blog>> GetAllBlogs(int? pageno, int? no)
        {
            var blogs = new List<Blog>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Blog/GetBlogs/{pageno}/{no}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogs = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
            return blogs;
        }

        public async Task<IActionResult> Index(int? pageno, int? no)
        {
            TempData["Pageno"] = pageno != null ? pageno : 1;
            TempData["SortBy"] = (no != null) ? no : 3;
            if (pageno == null)
            {
                pageno = 1;
            }
            if (no == null)
            {
                no = 2;
            }

            var result = await GetAllBlogs(pageno+1, no);
            
            ViewBag.HasNext = (result.Count == 0) ? false: true;

            return View(await GetAllBlogs(pageno, no));
        }

        private async Task<List<Blog>> ListBlogs()
        {
            var blogs = new List<Blog>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Blog/GetBlogs"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogs = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
            return blogs;
        }

        public async Task<IActionResult> ListUserBlog(string id)
        {
            var blogs = await ListBlogs();
            blogs = blogs.Where(x => x.Blogger == id).ToList() != null ? blogs.Where(x => x.Blogger == id).ToList() : null;

            return View(blogs);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListAllBlogs()
        {
            return View(await ListBlogs());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid id)
        {
            var blogdetails = new List<BlogHistory>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Blog/GetBlogDetail/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogdetails = JsonConvert.DeserializeObject<List<BlogHistory>>(apiResponse);
                }
            }
            return View(blogdetails);
        }

        private static async Task<List<Category>> AllCategories()
        {
            List<Category> blogCategories = new List<Category>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7281/api/BlogCategory/GetBlogCategories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogCategories = JsonConvert.DeserializeObject<List<Category>>(apiResponse);
                }
            }
            return blogCategories;
        }

        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Create()
        {
            List<Category> blogCategories = await AllCategories();
            var elist = new SelectList(blogCategories, nameof(Category.Id), nameof(Category.NameOfCategory));
            ViewBag.eventList = elist;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Create(Blog blog, IFormFile? fileUpload)
        {
            int ThreeMegaBytes = 3 * 1024 * 1024;
            if (fileUpload != null && fileUpload.Length > 0)
            {
                long fileSize = fileUpload.Length;
                if(fileSize > ThreeMegaBytes)
                {
                    TempData["error"] = "Image is more than 3 mb";
                    List<Category> blogCategories = await AllCategories();
                    var elist = new SelectList(blogCategories, nameof(Category.Id), nameof(Category.NameOfCategory));
                    ViewBag.eventList = elist;
                    return View(blog);
                }

                string filename = Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                string imgpath = Path.Combine(_env.WebRootPath, "images/blogs/", filename);
                using (FileStream streamread = new FileStream(imgpath, FileMode.Create))
                {
                    fileUpload.CopyTo(streamread);
                }
                blog.ImageUrl = filename;
            }

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                StringContent content = new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7281/api/Blog/AddBlog", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Blog added successfully";
                    }
                    else
                    {
                        TempData["error"] = "Please. Try again.";
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Update(Guid id)
        {
            Blog blog = new();
            List<Category> blogCategories = new List<Category>();

            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.GetAsync($"https://localhost:7281/api/Blog/GetBlog/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }

                using (var response = await httpClient.GetAsync("https://localhost:7281/api/BlogCategory/GetBlogCategories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogCategories = JsonConvert.DeserializeObject<List<Category>>(apiResponse);
                }

                var elist = new SelectList(blogCategories, nameof(Category.Id), nameof(Category.NameOfCategory));
                ViewBag.eventList = elist;
            }
            return PartialView(blog);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Update(Blog blog, IFormFile? fileUpload)
        {
            int ThreeMegaBytes = 3 * 1024 * 1024;
            if (fileUpload != null && fileUpload.Length > 0)
            {
                long fileSize = fileUpload.Length;
                if (fileSize > ThreeMegaBytes)
                {
                    TempData["error"] = "Image is more than 3 mb";
                    return View(blog);
                }
                string filename = Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                string imgpath = Path.Combine(_env.WebRootPath, "images/blogs/", filename);
                using (FileStream streamread = new FileStream(imgpath, FileMode.Create))
                {
                    fileUpload.CopyTo(streamread);
                }
                blog.ImageUrl = filename;
            }
            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                StringContent content = new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync($"https://localhost:7281/api/Blog/UpdateBlog", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Blog updated successfully";
                    }
                    else
                    {
                        TempData["error"] = "May be there is an issues. Try again.";
                    }
                }
            }
            return RedirectToAction("ListUserBlog", "Blog", new { id = blog.Blogger });
        }

        [Authorize(Roles = "Admin, Blogger")]
        public async Task<IActionResult> Delete(Guid id)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = Request.Cookies["access_token"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Login");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.DeleteAsync($"https://localhost:7281/api/Blog/DeleteBlog/{id}"))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Blog deleted successfully";
                    }
                    else
                    {
                        TempData["error"] = "May be there is an issues. Try again.";
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
