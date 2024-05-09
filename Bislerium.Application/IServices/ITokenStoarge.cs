using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface ITokenStorage
    {
        string SendToken(string token);
    }
}
