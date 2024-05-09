using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Statics
{
    public record LoginResponse(bool Flag, string Token, string Message);
    public record UserSession(string? Id, string? Name, string? Email, string? Role);
}
