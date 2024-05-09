using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.ViewModels
{
    public class LikeCountVM
    {
        public int Day { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Blog { get; set; }
        public int Comment { get; set; }
    }
}
