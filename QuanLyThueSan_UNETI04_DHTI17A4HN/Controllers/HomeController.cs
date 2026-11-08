using Microsoft.AspNetCore.Mvc;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using System.Diagnostics;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Admin đăng nhập xong thì "Trang chủ" chính là Dashboard quản trị
            if (HttpContext.Session.GetString(SessionKeys.VaiTro) == "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

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
