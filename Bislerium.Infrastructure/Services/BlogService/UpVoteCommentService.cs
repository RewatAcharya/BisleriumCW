using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.Notifications;
using Bislerium.Domain.Entity.Users;
using Bislerium.Domain.Enums;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Services.HubService;
using Bislerium.Infrastructure.Services.NotificationServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.BlogService
{
    public class UpVoteCommentService : IUpVoteCommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public UpVoteCommentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> Vote(UpvoteComment commentVote)
        {
            try
            {
                var existingLike = await GetLike(commentVote.LikedComment, commentVote.LikedUser);

                CommentService commentService = new CommentService(_context, _userManager);
                var comment = await commentService.GetByIdAsync(commentVote.LikedComment);


                if (existingLike != null)
                {
                    if ((existingLike.Reaction == ReactionType.UpVote && commentVote.Reaction == ReactionType.DownVote)
                        || (existingLike.Reaction == ReactionType.DownVote && commentVote.Reaction == ReactionType.UpVote))
                    {
                        var result = await new UpVoteCommentService(_context, _userManager).UpdateAsync(existingLike.Id);
                        
                        return true;
                    }
                    else
                    {
                        if (commentVote.Reaction == ReactionType.UpVote)
                        {
                            comment.UpVotes -= 1;
                        }
                        else
                        {
                            comment.DownVotes -= 1;
                        }
                        
                        var result = await new UpVoteCommentService(_context, _userManager).Delete(existingLike.Id);
                        if (result == true)
                        {
                            _context.Comments.Update(comment);
                            await _context.SaveChangesAsync();

                        }
                        return false;
                    }
                }
                else
                {
                   
                    if (commentVote.Reaction == ReactionType.UpVote)
                    {
                        comment.UpVotes += 1;
                    }
                    else
                    {
                        comment.DownVotes += 1;
                    }
                    _context.Comments.Update(comment);

                    var result = _context.UpvoteComments.Add(commentVote);
                    await _context.SaveChangesAsync();

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while voting: {ex.Message}");
                return false;
            }
        }

        public async Task<UpvoteComment> GetLike(Guid commentId, string userId)
        {
            return await _context.UpvoteComments.FirstOrDefaultAsync(x => x.LikedComment == commentId && x.LikedUser == userId);
        }

        public async Task<UpvoteComment> UpdateAsync(Guid id)
        {
            try
            {
                var like = await _context.UpvoteComments.FirstOrDefaultAsync(x => x.Id == id);
                if (like != null)
                {
                    CommentService commentService = new CommentService(_context, _userManager);
                    var comment = await commentService.GetByIdAsync(like.LikedComment);

                    if (like.Reaction == ReactionType.UpVote)
                    {
                        like.Reaction = ReactionType.DownVote;

                        comment.UpVotes -= 1;
                        comment.DownVotes += 1;
                    }
                    else
                    {
                        like.Reaction = ReactionType.UpVote;

                        comment.UpVotes += 1;
                        comment.DownVotes -= 1;
                    }
                    _context.Comments.Update(comment);
                }
                _context.UpvoteComments.Update(like);
                await _context.SaveChangesAsync();
                return like;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting vote: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(Guid commentId)
        {
            try
            {
                var like = await _context.UpvoteComments.FirstOrDefaultAsync(x => x.Id == commentId);
                if (like == null)
                {
                    return false;
                }

                _context.UpvoteComments.Remove(like);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting vote: {ex.Message}");
                return false;
            }
        }
    }
}
