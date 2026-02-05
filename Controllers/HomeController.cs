using Microsoft.AspNetCore.Mvc;

namespace NotesApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Notes");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
