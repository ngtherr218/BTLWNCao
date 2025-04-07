using Microsoft.AspNetCore.Mvc;

namespace BTLWNCao.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;
            return View();
        }
    }
}
