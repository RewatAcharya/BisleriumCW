using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Infrastructure.Services.BlogService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize(Roles = "Blogger")]
        [HttpPost, Route("AddComment")]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            var result = await _commentService.CreateAsync(comment);
            return Ok(result);
        }

        [HttpGet, Route("GetComment/{id}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var result = await _commentService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetComments")]
        public async Task<IActionResult> GetComments()
        {
            var result = await _commentService.GetAllAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet, Route("GetCommentDetail/{id}")]
        public async Task<IActionResult> GetBlogDetail(Guid id)
        {
            var result = await _commentService.GetCommentDetailAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Authorize(Roles = "Blogger")]
        [HttpDelete, Route("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(Guid Id)
        {
            await _commentService.DeleteAsync(Id);
            return Ok();
        }

        [Authorize(Roles = "Blogger")]
        [HttpPut, Route("UpdateComment")]
        public async Task<IActionResult> UpdateComment(Comment comment)
        {
            var result = await _commentService.UpdateAsync(comment);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
