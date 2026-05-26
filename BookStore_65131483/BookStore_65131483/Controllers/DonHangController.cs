using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;

namespace BookStore_65131483.Controllers
{
    public class DonHangController : Controller
    {
        // GET: DonHang
        public ActionResult Index()
        {
            return View();
        }
        private BookStore_65131483Entities db = new BookStore_65131483Entities();
        private DONHANG LayGioHang()
        {
            int? maTK = Session["MaTK"] as int?;
            if (maTK == null) return null;

            return db.DONHANGs
                     .Include(g => g.CHITIETDONHANGs.Select(c => c.SACH))
                     .FirstOrDefault(g => g.MaTK == maTK && g.TrangThai == "TrongGioHang");
        }

        public ActionResult ThanhToan()
        {
            int? maTK = Session["MaTK"] as int?;
            if (maTK == null)
            {
                return RedirectToAction("Login", "TaiKhoan_65131483");
            }

            var gioHang = LayGioHang();
            if (gioHang == null || !gioHang.CHITIETDONHANGs.Any())
            {
                ViewBag.Message = "Giỏ hàng trống!";
                return View();
            }

            return View(gioHang);
        }
    }
}