using System.Web.Mvc;

namespace MoviesDBManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            return View();
        }
    }
}