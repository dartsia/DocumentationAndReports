using System.Diagnostics;
using DocumentationAndReports.Models;
using Microsoft.AspNetCore.Mvc;
using DocumentationAndReports.Data;
using Microsoft.EntityFrameworkCore;

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
            return View(objScheduleList);
        }

        // Генерація нового розкладу
        [HttpPost]
        //[Route("generate-schedule")]
        public async Task<IActionResult> GenerateSchedule()
        {
            try
            {
                var currentDate = DateTime.Now;
                int month = currentDate.Month;
                int year = currentDate.Year;

                List<string> weekendDays = GetWeekendDays(year, month);
                int daysInMonth = DateTime.DaysInMonth(year, month);
                List<Schedule> duties = new();

                // Завантаження солдатів з бази
                var soldiers = await _db.Soldiers.ToListAsync();

                // Очистка попереднього розкладу
                await _db.Database.ExecuteSqlRawAsync("DELETE FROM schedule");

                for (int i = 2; i <= daysInMonth; i++)
                {
                    var rawDate = new DateTime(year, month, i);
                    var date = DateOnly.FromDateTime(rawDate);
                    string dateStr = rawDate.ToString("yyyy-MM-dd");

                    bool isWeekend = weekendDays.Contains(dateStr);

                    // Відбір доступних солдатів
                    var availableSoldiers = soldiers.Where(s =>
                        s.Gender != "female" &&
                        !s.Holidays.Contains(date) &&
                        !s.SickLeaves.Contains(date) &&
                        (s.LastDuty == null || s.LastDuty.Date != dateStr) &&
                        (s.LastDuty == null || s.LastDuty.Date != rawDate.AddDays(-1).ToString("yyyy-MM-dd"))
                    ).ToList();

                    ShuffleList(availableSoldiers);

                    // Чергування у частині
                    var unitDutySoldier = availableSoldiers.FirstOrDefault(s => s.UnitDuties < 5);
                    if (unitDutySoldier != null)
                    {
                        duties.Add(new Schedule { Date = date, SoldierName = unitDutySoldier.Name, Type = "У частині" });
                        unitDutySoldier.UnitDuties++;
                        unitDutySoldier.LastDuty = new Duty { Date = dateStr, Type = "У частині" };
                        availableSoldiers.Remove(unitDutySoldier);
                    }

                    // Чергування поза частиною
                    var outsideDutySoldier = availableSoldiers.FirstOrDefault(s =>
                        s.OutsideDuties < 5 && (!isWeekend || !s.WeekendWeeks.Contains(GetWeekOfMonth(rawDate)))
                    );

                    if (outsideDutySoldier != null)
                    {
                        duties.Add(new Schedule { Date = date, SoldierName = outsideDutySoldier.Name, Type = "Поза частиною" });
                        outsideDutySoldier.OutsideDuties++;
                        outsideDutySoldier.LastDuty = new Duty { Date = dateStr, Type = "Поза частиною" };
                        if (isWeekend)
                        {
                            outsideDutySoldier.WeekendWeeks.Add(GetWeekOfMonth(rawDate));
                        }
                    }
                }

                // Додаємо дані у базу
                await _db.Schedules.AddRangeAsync(duties);
                await _db.SaveChangesAsync();

                TempData["Message"] = "Розклад успішно згенеровано!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating schedule: " + ex.Message);
                TempData["Error"] = "Помилка генерації розкладу.";
                return RedirectToAction("Index");
            }
        }

        private List<string> GetWeekendDays(int year, int month)
        {
            var weekendDays = new List<string>();
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int i = 1; i <= daysInMonth; i++)
            {
                var date = new DateTime(year, month, i);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendDays.Add(date.ToString("yyyy-MM-dd"));
                }
            }
            return weekendDays;
        }

        private int GetWeekOfMonth(DateTime date)
        {
            return (date.Day - 1) / 7 + 1;
        }

        private void ShuffleList<T>(List<T> list)
        {
            Random rng = new();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        [Route("schedule-for-one")]
        public IActionResult ScheduleForOne(string? name)
        {
            if(name == null)
            {
                return NotFound();
            }
            List<Schedule> objScheduleForOne = _db.Schedules
                .Where(s => s.SoldierName == name)
                .ToList();

            return View(objScheduleForOne);
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
