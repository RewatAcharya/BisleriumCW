using Bislerium.Domain.Entity.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Statics
{
    public static class SetStatics
    {
        public static int Upvote = 2;
        public static int Downvote = -1;
        public static int Comment = 1;

        public static int CalculatePopularity(int upVotes, int downVote, int comment)
        {
            return (Upvote * upVotes) + (Downvote * downVote) + (Comment * comment);
        }
    }
}
