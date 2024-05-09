using Bislerium.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface ITopTenService
    {
        Task<TopTen> GetTopTenData(int year, int month);
    }
}
