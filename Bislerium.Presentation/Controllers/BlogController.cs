using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [Authorize(Roles = "Admin, Blogger")]
        [HttpPost, Route("AddBlog")]
        public async Task<IActionResult> AddBlog(Blog blog)
        {
            var result = await _blogService.CreateAsync(blog);
            return Ok(result);
        }

        [HttpGet, Route("GetBlog/{id}")]
        public async Task<IActionResult> GetBlog(Guid id)
        {
            var result = await _blogService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetBlogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var result = await _blogService.GetAllAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetBlogs/{pageno}/{no}")]
        public async Task<IActionResult> GetAllBlogs(int pageno, int no)
        {
            var result = await _blogService.GetAllAsync(pageno, no);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetBlogDetail/{id}")]
        public async Task<IActionResult> GetBlogDetail(Guid id)
        {
            var result = await _blogService.GetBlogDetailAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Blogger")]
        [HttpDelete, Route("DeleteBlog/{Id}")]
        public async Task<IActionResult> DeleteBlog(Guid Id)
        {
            await _blogService.DeleteAsync(Id);
            return Ok();
        }

        [Authorize(Roles = "Admin, Blogger")]
        [HttpPut, Route("UpdateBlog")]
        public async Task<IActionResult> UpdatBlog(Blog blog)
        {
            var result = await _blogService.UpdateAsync(blog);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
