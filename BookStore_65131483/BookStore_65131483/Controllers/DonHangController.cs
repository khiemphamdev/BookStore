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
        [HttpPost]
        public JsonResult ThemGioHang(int maSach, int soLuong = 1)
        {
            // 1. Kiểm tra đăng nhập trước
            int? maTK = Session["MaTK"] as int?;
            if (maTK == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập" });
            }

            // 2. Kiểm tra sách có tồn tại không
            var sach = db.SACHes.Find(maSach);
            if (sach == null)
            {
                return Json(new { success = false, message = "Sách không tồn tại" });
            }

            // Lấy giá thực tế từ Database 
            decimal giaBan = sach.GiaBan;

            // 3. Tìm hoặc tạo mới Giỏ hàng
            var gioHang = db.DONHANGs.FirstOrDefault(g => g.MaTK == maTK && g.TrangThai == "TrongGioHang");
            if (gioHang == null)
            {
                gioHang = new DONHANG
                {
                    MaTK = maTK.Value,
                    TrangThai = "TrongGioHang",
                    NgayDat = DateTime.Now,
                    TongTien = 0
                };
                db.DONHANGs.Add(gioHang);
                db.SaveChanges(); // Lưu để lấy MaDH tự tăng
            }

            // 4. Kiểm tra xem sách này đã có trong chi tiết giỏ hàng chưa
            var chiTiet = db.CHITIETDONHANGs
                            .FirstOrDefault(c => c.MaDH == gioHang.MaDH && c.MaSach == maSach);

            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
            }
            else
            {
                // Đảm bảo Constructor của bạn nhận đúng tham số: MaDH, MaSach, SoLuong, DonGia
                chiTiet = new CHITIETDONHANG(gioHang.MaDH, maSach, soLuong, giaBan);
                db.CHITIETDONHANGs.Add(chiTiet);
            }

            db.SaveChanges();

            // 5. Cập nhật lại tổng tiền chính xác của Đơn hàng
            gioHang.TongTien = db.CHITIETDONHANGs
                                 .Where(c => c.MaDH == gioHang.MaDH)
                                 .Sum(c => (c.SoLuong * c.DonGia)) ;
            db.SaveChanges();

            // Tính tổng số lượng item hiển thị trên Icon Giỏ hàng
            int tongSoLuong = db.CHITIETDONHANGs
                                .Where(c => c.MaDH == gioHang.MaDH)
                                .Sum(c => (int?)c.SoLuong) ?? 0;

            return Json(new { success = true, tongSoLuong });
        }
    }
}