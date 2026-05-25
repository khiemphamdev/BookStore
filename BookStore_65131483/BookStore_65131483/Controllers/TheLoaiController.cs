using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;

namespace BookStore_65131483.Controllers
{
    public class TheLoaiController : Controller
    {
        // GET: TheLoai
        BookStore_65131483Entities db = new BookStore_65131483Entities();


        // List tất cả danh mục
        public ActionResult DsTheLoai()
        {
            var list = db.THELOAIs.ToList();
            return View(list);
        }

        // GET: Thêm danh mục
        public ActionResult ThemTheLoai()
        {
            return View();
        }

        // POST: Thêm danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemTheLoai(THELOAI tl)
        {
            if (!ModelState.IsValid) return View(tl);

            string tenTheLoaiForm = (tl.TenTheLoai ?? "").Trim().ToLower();

            //  Kiểm tra xem tên thể loại đã tồn tại chưa
            bool bixTrung = db.THELOAIs.Any(t => t.TenTheLoai.Trim().ToLower() == tenTheLoaiForm);
            if (bixTrung)
            {
                ModelState.AddModelError("TenTheLoai", "Tên thể loại này đã tồn tại trong hệ thống!");
                return View(tl);
            }

            db.THELOAIs.Add(tl);
            db.SaveChanges();
            return RedirectToAction("DsTheLoai");
        }

        // GET: Sửa danh mục
        public ActionResult SuaTheLoai(int id)
        {
            var dm = db.THELOAIs.Find(id);
            if (dm == null) return HttpNotFound();
            return View(dm);
        }

        // POST: Sửa danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaTheLoai(THELOAI tl)
        {
            if (!ModelState.IsValid) return View(tl);

            // CHỐNG TRÙNG KHI SỬA: Tên trùng với thể loại khác (trừ chính nó)
            string tenTheLoaiForm = (tl.TenTheLoai ?? "").Trim().ToLower();
            bool bixTrung = db.THELOAIs.Any(t => t.TenTheLoai.Trim().ToLower() == tenTheLoaiForm && t.MaTheLoai != tl.MaTheLoai);

            if (bixTrung)
            {
                ModelState.AddModelError("TenTheLoai", "Tên thể loại này đã trùng với một danh mục khác!");
                return View(tl);
            }

            db.Entry(tl).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DsTheLoai");
        }

        // Xóa danh mục
        public ActionResult XoaTheLoai(int id)
        {
            var tl = db.THELOAIs.Find(id);
            if (tl == null) return HttpNotFound();

            bool daCoSach = db.SACHes.Any(s => s.MaTheLoai == id);

            if (daCoSach)
            {
                TempData["Error"] = "Không thể xóa thể loại này vì đang có sách thuộc danh mục này!";
                return RedirectToAction("DsTheLoai");
            }

            db.THELOAIs.Remove(tl);
            db.SaveChanges();
            TempData["Success"] = "Xóa thể loại thành công!";
            return RedirectToAction("DsTheLoai");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}