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

        // ================== QUẢN LÝ USER ==================
        public ActionResult UserList()
        {
            var list = db.TAIKHOANs
                         .OrderBy(t => t.MaTK)
                         .Skip(1)
                         .ToList();

            return View(list);
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var tk = db.TAIKHOANs.Find(id);
            return View(tk);
        }

        [HttpPost]
        public ActionResult EditUser(TAIKHOAN tk)
        {
            var old = db.TAIKHOANs.Find(tk.MaTK);
            if (db.TAIKHOANs.Any(x => x.Email == tk.Email && x.MaTK != tk.MaTK))
            {
                ViewBag.Error = "Email đã tồn tại";
                return View(tk);
            }

            if (db.TAIKHOANs.Any(x => x.SDT == tk.SDT && x.MaTK != tk.MaTK))
            {
                ViewBag.Error = "Số điện thoại đã tồn tại";
                return View(tk);
            }

            old.HoTen = tk.HoTen;
            old.Email = tk.Email;
            old.SDT = tk.SDT;
            old.DiaChi = tk.DiaChi;
            old.VaiTro = tk.VaiTro;

            db.SaveChanges();
            return RedirectToAction("UserList");
        }

        public ActionResult DeleteUser(int id)
        {
            var tk = db.TAIKHOANs.Find(id);
            if (tk != null)
            {
                db.TAIKHOANs.Remove(tk);
                db.SaveChanges();
            }
            return RedirectToAction("UserList");
        }

        // ================== QUẢN LÝ ĐƠN HÀNG ==================
        public ActionResult OrderList()
        {
            var donHangs = db.DONHANGs
                             .Include(d => d.TAIKHOAN)
                             .Where(d => d.TrangThai != "TrongGioHang")
                             .ToList()
                             .OrderByDescending(d => d.NgayDat);

            return View(donHangs);
        }

        public ActionResult CapNhatTrangThai(int maDH, int trangThai)
        {
            var donHang = db.DONHANGs.FirstOrDefault(d => d.MaDH == maDH);

            if (donHang == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("OrderList");
            }

            switch (trangThai)
            {
                case 1:
                    donHang.TrangThai = "DaHuy";
                    break;
                case 2:
                    donHang.TrangThai = "DangVanChuyen";
                    break;
                case 3:
                    donHang.TrangThai = "HoanThanh";
                    break;
                default:
                    TempData["Message"] = "Trạng thái không hợp lệ!";
                    return RedirectToAction("OrderList");
            }

            db.SaveChanges();
            TempData["Message"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("OrderList");
        }

        public ActionResult XemCTDH(int maDH)
        {
            var chiTietDonHangs = db.CHITIETDONHANGs
                                     .Where(d => d.MaDH == maDH)
                                     .ToList();

            return View(chiTietDonHangs);
        }

    }
}