using System.Diagnostics;
using DocumentationAndReports.Models;
using Microsoft.AspNetCore.Mvc;
using DocumentationAndReports.Data;

namespace DocumentationAndReports.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DutyScheduleContext _db;

        public HomeController(ILogger<HomeController> logger, DutyScheduleContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            List<Schedule> objScheduleList = _db.Schedules.ToList();
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
