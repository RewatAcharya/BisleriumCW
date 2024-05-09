using Microsoft.AspNetCore.Mvc;

namespace Bislerium.WebApp.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
