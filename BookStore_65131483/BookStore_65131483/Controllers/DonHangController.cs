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
                return RedirectToAction("Login", "TaiKhoan");
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
        public ActionResult XemGioHang()
        {
            int? maTK = Session["MaTK"] as int?;
            if (maTK == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
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
        public ActionResult DatHang(string DiaChi, string GhiChu)
        {
            int? maTK = Session["MaTK"] as int?;
            if (maTK == null)
            {
                // Đồng nhất điều hướng về TaiKhoan_65131483 thay vì "Account" cũ
                return RedirectToAction("Login", "TaiKhoan");
            }

            var donHang = db.DONHANGs.FirstOrDefault(d => d.MaTK == maTK && d.TrangThai == "TrongGioHang");
            if (donHang == null || !donHang.CHITIETDONHANGs.Any())
            {
                return Json(new { success = false, message = "Không có sản phẩm nào để đặt hàng!" });
            }

            // Kiểm tra số lượng tồn kho trước khi trừ số lượng
            foreach (var ct in donHang.CHITIETDONHANGs)
            {
                if (ct.SACH == null || ct.SACH.SoLuongTon < ct.SoLuong)
                {
                    return Json(new { success = false, message = $"Sách '{ct.SACH?.TieuDe}' không đủ số lượng trong kho!" });
                }
            }

            // Trừ kho sau khi chắc chắn tất cả mặt hàng đều đủ hàng
            foreach (var ct in donHang.CHITIETDONHANGs)
            {
                ct.SACH.SoLuongTon -= ct.SoLuong;
            }

            // Cập nhật thông tin đơn hàng sang trạng thái chờ xử lý
            donHang.NgayDat = DateTime.Now;
            donHang.DiaChiGiao = DiaChi;
            donHang.PhuongThucThanhToan = "Cod";
            donHang.GhiChu = GhiChu;
            donHang.TrangThai = "Chờ xử lý";
            donHang.TongTien = donHang.CHITIETDONHANGs.Sum(c => c.SoLuong * c.DonGia);

            // Cập nhật địa chỉ mặc định cho tài khoản
            var taiKhoan = db.TAIKHOANs.Find(maTK);
            if (taiKhoan != null)
            {
                taiKhoan.DiaChi = DiaChi;
            }

            db.SaveChanges();
            TempData["Message"] = "Đơn hàng đã được đặt thành công và đang chờ xử lý.";

            return Json(new { success = true });
        }
    }
}