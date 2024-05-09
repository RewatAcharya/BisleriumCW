using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Infrastructure.Services.BlogService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UpVoteBlogController : ControllerBase
    {
        private readonly IUpVoteBlogService _upVoteBlogService;

        public UpVoteBlogController(IUpVoteBlogService upVoteBlogService)
        {
            _upVoteBlogService = upVoteBlogService;
        }

        [HttpPost, Route("Reaction")]
        public async Task<IActionResult> Vote(LikeBlog like)
        {
            var result = await _upVoteBlogService.Vote(like);
            if (result == true)
            {
                return Ok(result);
            }
            else 
            {
                return Ok(result);
            }
        }
    }
}
