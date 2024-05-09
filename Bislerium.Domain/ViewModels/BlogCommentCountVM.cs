using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.ViewModels
{
    public class BlogCommentCountVM
    {
        public int Day { get; set; }
        public int Blog { get; set; }
        public int Comment { get; set; }
    }
}
