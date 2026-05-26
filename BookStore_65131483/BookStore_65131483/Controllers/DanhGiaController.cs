using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;
using System.Data.Entity;

namespace BookStore_65131483.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly BookStore_65131483Entities db = new BookStore_65131483Entities();

        // Hàm helper dùng chung để lấy MaTK, tránh lặp code và an toàn hơn
        private int? GetMaTK()
        {
            return Session["MaTK"] as int?;
        }

        // GET: DanhGia_65131483/DanhGia?maDH=...
        public ActionResult DanhGia(int maDH)
        {
            var maTK = GetMaTK();
            if (maTK == null)
                return RedirectToAction("Login", "TaiKhoan_65131483");

            // Kiểm tra xem đơn hàng có tồn tại và thuộc về tài khoản này không
            var donHangExists = db.DONHANGs.Any(d => d.MaDH == maDH && d.MaTK == maTK);
            if (!donHangExists)
                return HttpNotFound();

            // Tối ưu Query: Lấy các sách trong đơn hàng chưa được đánh giá bởi đơn hàng này
            var sachChuaDanhGia = db.CHITIETDONHANGs
                .Where(ct => ct.MaDH == maDH && !db.DANHGIAs.Any(dg => dg.MaSach == ct.MaSach && dg.MaDH == maDH))
                .Include(ct => ct.SACH)
                .ToList();

            return View(sachChuaDanhGia);
        }

        // POST: DanhGia_65131483/LuuDanhGia
        [HttpPost]
        public ActionResult LuuDanhGia(int maSach, byte diem, string binhLuan, int maDH)
        {
            try
            {
                // 1. Kiểm tra đăng nhập ngay lập tức
                var maTK = GetMaTK();
                if (maTK == null)
                {
                    return Json(new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại!" });
                }

                // 2. Tìm kiếm đánh giá cũ (nếu có)
                var danhGiaHienTai = db.DANHGIAs.FirstOrDefault(d => d.MaSach == maSach && d.MaTK == maTK && d.MaDH == maDH);

                if (danhGiaHienTai != null)
                {
                    // Cập nhật đánh giá cũ
                    danhGiaHienTai.Diem = diem;
                    danhGiaHienTai.BinhLuan = binhLuan?.Trim(); // Tránh lưu khoảng trắng thừa
                    danhGiaHienTai.NgayDanhGia = DateTime.Now; // Cập nhật lại ngày sửa
                }
                else
                {
                    // Thêm mới đánh giá
                    var newDanhGia = new DANHGIA
                    {
                        MaSach = maSach,
                        MaTK = maTK.Value,
                        Diem = diem,
                        MaDH = maDH,
                        NgayDanhGia = DateTime.Now,
                        BinhLuan = binhLuan?.Trim()
                    };
                    db.DANHGIAs.Add(newDanhGia);
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Đánh giá thành công!" });
            }
            catch (Exception ex)
            {
                // Ghi nhận lỗi vào Output window của Visual Studio khi đang debug
                System.Diagnostics.Debug.WriteLine($"Lỗi Lưu Đánh Giá: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau." });
            }
        }

        // Giải phóng tài nguyên bộ nhớ khi Controller không còn sử dụng
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}