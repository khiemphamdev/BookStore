using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;
using System.Data.Entity;
namespace BookStore_65131483.Controllers
{
    public class ChiTietSach_65131483Controller : Controller
    {
        // GET: ChiTietSach_65131483
        BookStore_65131483Entities db = new BookStore_65131483Entities();


        // GET: /Sach/Details/5
        public ActionResult ChiTietSach(int id)
        {
            var sach = db.SACHes
             .Include(s => s.DANHGIAs)      // load đánh giá
             .FirstOrDefault(s => s.MaSach == id);
            if (sach == null)
                return HttpNotFound();

            return View(sach);
        }
    }
}