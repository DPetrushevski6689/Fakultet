using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFaculty.Models;

namespace MVCFaculty.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public HomeController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await userManager.GetUserAsync(User);
                if ((await userManager.IsInRoleAsync(appUser, "Admin")))
                {
                    return RedirectToAction("Index", "Courses", null);
                }
                if ((await userManager.IsInRoleAsync(appUser, "Teacher")))
                {
                    return RedirectToAction("Courses", "Teacher", null);
                }
                if ((await userManager.IsInRoleAsync(appUser, "Student")))
                {
                    return RedirectToAction("Enrollments", "Student", null);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", null);
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
