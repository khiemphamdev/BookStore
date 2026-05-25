using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;
namespace BookStore_65131483.Controllers
{
    public class SachController : Controller
    {
        private readonly BookStore_65131483Entities db = new BookStore_65131483Entities();

        // 1. HIỂN THỊ DANH SÁCH SÁCH
        public ActionResult DsSach()
        {
            var list = db.SACHes
                  .Include(s => s.TACGIA)
                  .Include(s => s.THELOAI)
                  .Where(s => s.IsActive == true)
                  .OrderByDescending(s => s.NgayTao)
                  .ToList();

            return View(list);
        }

        // 2. GET: THÊM SÁCH
        public ActionResult ThemSach()
        {
            LoadDropdowns();
            return View();
        }

        // 3. POST: THÊM SÁCH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSach(SACH sach)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(sach.MaTacGia, sach.MaTheLoai);
                return View(sach);
            }

            // Phòng trường hợp TieuDe bị null bằng toán tử null-coalescing (??)
            string tieuDeForm = (sach.TieuDe ?? "").Trim().ToLower();

            // Kiểm tra sách đã tồn tại chưa
            var sachTonTai = db.SACHes.FirstOrDefault(s =>
                s.TieuDe.Trim().ToLower() == tieuDeForm && s.MaTacGia == sach.MaTacGia
            );

            if (sachTonTai != null)
            {
                // Cập nhật lại thông tin dựa trên dữ liệu mới nhập
                sachTonTai.IsActive = true;
                sachTonTai.NgayTao = DateTime.Now;
                sachTonTai.MaTheLoai = sach.MaTheLoai;
                sachTonTai.GiaBan = sach.GiaBan;
                sachTonTai.SoLuongTon = sach.SoLuongTon;

                db.SaveChanges();
                TempData["Success"] = "Sách đã tồn tại, hệ thống đã tự động kích hoạt và cập nhật lại!";
                return RedirectToAction("DsSach");
            }

            // Xử lý upload ảnh bìa mới hoàn toàn
            sach.AnhBia = ProcessUploadImage(Request.Files["AnhBia"]) ?? "/Images/noImage.png";
            sach.IsActive = true;
            sach.NgayTao = DateTime.Now;

            db.SACHes.Add(sach);
            db.SaveChanges();
            return RedirectToAction("DsSach");
        }

        // 4. GET: SỬA SÁCH
        public ActionResult SuaSach(int id)
        {
            var sach = db.SACHes.Find(id);
            if (sach == null) return HttpNotFound();

            LoadDropdowns(sach.MaTacGia, sach.MaTheLoai);
            return View(sach);
        }

        // 5. POST: SỬA SÁCH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaSach(SACH sach)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(sach.MaTacGia, sach.MaTheLoai);
                return View(sach);
            }

            var oldSach = db.SACHes.Find(sach.MaSach);
            if (oldSach == null) return HttpNotFound();

            // Xử lý lưu ảnh: Nếu chọn ảnh mới thì đổi, không thì giữ lại đường dẫn cũ
            string newImagePath = ProcessUploadImage(Request.Files["AnhBia"]);
            if (!string.IsNullOrEmpty(newImagePath))
            {
                oldSach.AnhBia = newImagePath;
            }

            // Đồng bộ dữ liệu cập nhật ngắn gọn
            if (sach.TieuDe != null)
            oldSach.TieuDe = sach.TieuDe;
            if (sach.MaTacGia != null)
            oldSach.MaTacGia = sach.MaTacGia;
            if (sach.MaTheLoai != null)
            oldSach.MaTheLoai = sach.MaTheLoai;
            oldSach.GiaBan = sach.GiaBan;
            oldSach.SoLuongTon = sach.SoLuongTon;
            oldSach.MoTa = sach.MoTa;
            if (sach.NgonNgu != null)
            oldSach.NgonNgu = sach.NgonNgu;

            db.SaveChanges();
            return RedirectToAction("DsSach");
        }

        // 6. XÓA SÁCH
        public ActionResult XoaSach(int id)
        {
            var sach = db.SACHes.Find(id);
            if (sach == null) return HttpNotFound();

            bool coChiTietDonHang = db.CHITIETDONHANGs.Any(ct => ct.MaSach == id);
            if (coChiTietDonHang)
            {
                sach.IsActive = false; // Soft delete (Xóa mềm)
                db.SaveChanges();
                TempData["Error"] = "Sách đã tồn tại trong đơn hàng, hệ thống đã chuyển sang chế độ 'Ngừng bán'!";
                return RedirectToAction("DsSach");
            }

            db.SACHes.Remove(sach); // Hard delete (Xóa cứng)
            db.SaveChanges();
            return RedirectToAction("DsSach");
        }
        private void LoadDropdowns(int? selectedTacGia = null, int? selectedTheLoai = null)
        {
            ViewBag.MaTacGia = new SelectList(db.TACGIAs, "MaTacGia", "TenTacGia", selectedTacGia);
            ViewBag.MaTheLoai = new SelectList(db.THELOAIs, "MaTheLoai", "TenTheLoai", selectedTheLoai);
        }

        // Hàm xử lý Upload ảnh tập trung (Sửa bug Guid ở code cũ và tái sử dụng cho cả Thêm/Sửa)
        private string ProcessUploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string originalFileName = Path.GetFileName(file.FileName);
                // Tạo tên file duy nhất bằng Guid để tránh ghi đè tệp trùng tên lên server
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(originalFileName);
                string serverPath = Server.MapPath("~/Images/" + uniqueFileName);

                file.SaveAs(serverPath);
                return "/Images/" + uniqueFileName;
            }
            return null;
        }

        // Giải phóng kết nối cơ sở dữ liệu khi dùng xong
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}