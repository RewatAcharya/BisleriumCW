using Bislerium.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboardData(int year, int month);
        Task<LineChart> GetDataRange();
        Task<List<LikeCountVM>> ChartLikeDislikes(int year, int month);
    }
}
