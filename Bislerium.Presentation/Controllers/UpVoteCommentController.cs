using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpVoteCommentController : ControllerBase
    {
        private readonly IUpVoteCommentService _upVoteService;

        public UpVoteCommentController(IUpVoteCommentService upVoteService)
        {
            _upVoteService = upVoteService;
        }

        [HttpPost, Route("Reaction")]
        public async Task<IActionResult> Vote(UpvoteComment like)
        {
            var result = await _upVoteService.Vote(like);
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
