using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoryController : ControllerBase
    {
        private readonly IBlogCategoryService _blogCategoryService;

        public BlogCategoryController(IBlogCategoryService blogCategoryService)
        {
            _blogCategoryService = blogCategoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, Route("AddBlogCategory")]
        public async Task<IActionResult> AddBlogCategory(Category category)
        {
            var result = await _blogCategoryService.CreateAsync(category);
            return Ok(result);
        }

        [HttpGet, Route("GetBlogCategory/{Id}")]
        public async Task<IActionResult> GetBlogCategory(Guid Id)
        {
            var result = await _blogCategoryService.GetByIdAsync(Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetBlogCategories")]
        public async Task<IActionResult> GetBlogCategories()
        {
            var result = await _blogCategoryService.GetAllAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("DeleteBlogCategory/{Id}")]
        public async Task<IActionResult> DeleteBlogCategory(Guid Id)
        {
            await _blogCategoryService.DeleteAsync(Id);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut, Route("UpdateBlogCategory")]
        public async Task<IActionResult> UpdateBlogCategory(Category category)
        {
            var result = await _blogCategoryService.UpdateAsync(category);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
