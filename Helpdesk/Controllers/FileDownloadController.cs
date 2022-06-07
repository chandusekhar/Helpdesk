using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Controllers
{
    public class FileDownloadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
