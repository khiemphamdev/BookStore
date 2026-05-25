using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;

namespace BookStore_65131483.Controllers
{
    public class TrangAdminController : Controller
    {
        BookStore_65131483Entities db = new BookStore_65131483Entities();
        // ================== DASHBOARD ==================
        [HttpGet]
        public ActionResult Dashboard()
        {

            ViewBag.TongDonHang = db.DONHANGs.Where(d => d.TrangThai == "HoanThanh").Count();


            ViewBag.TongDoanhThu = db.DONHANGs
                                     .Where(d => d.TrangThai.Trim().ToLower() == "hoanthanh" && d.TongTien.HasValue)
                                     .Sum(d => d.TongTien) ?? 0;

            // 3. Sách đã bán
            if (db.CHITIETDONHANGs.Any(ct => ct.DONHANG.TrangThai.Trim().ToLower() == "hoanthanh"))
            {
                ViewBag.SachDaBan = db.CHITIETDONHANGs
                                      .Where(ct => ct.DONHANG.TrangThai.Trim().ToLower() == "hoanthanh")
                                      .Sum(ct => ct.SoLuong);
            }
            else
            {
                ViewBag.SachDaBan = 0;
            }

            // 4. Số khách hàng 
            ViewBag.SoKhachHang = db.TAIKHOANs
                                    .Where(tk => tk.VaiTro.Trim().ToLower() == "user")
                                    .Count();

            return View();
        }

    }
}