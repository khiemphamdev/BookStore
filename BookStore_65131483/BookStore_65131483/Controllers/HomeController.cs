using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;

namespace BookStore_65131483.Controllers
{
    public class HomeController : Controller
    {
        BookStore_65131483Entities db = new BookStore_65131483Entities();

        public ActionResult Index()
        {
            var list = db.SACHes
            .Where(s => s.IsActive == true)
             .OrderBy(s => s.MaSach)
             .Take(8)
             .ToList();
            return View(list);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}