using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_65131483.Models;

namespace BookStore_65131483.Controllers
{
    public class TacGiaController : Controller
    {
      
        private readonly BookStore_65131483Entities db = new BookStore_65131483Entities();

        // 1. DANH SÁCH TÁC GIẢ
        public ActionResult AuthorList()
        {
            var list = db.TACGIAs.OrderByDescending(tg => tg.MaTacGia).ToList();
            return View(list);
        }

        // 2. GET: THÊM TÁC GIẢ
        public ActionResult AddAuthor()
        {
            return View();
        }

        // 3. POST: THÊM TÁC GIẢ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuthor(TACGIA tg)
        {
            if (!ModelState.IsValid) return View(tg);
            string tenTacGiaForm = (tg.TenTacGia ?? "").Trim().ToLower();

            bool biTrung = db.TACGIAs.Any(t => t.TenTacGia.Trim().ToLower() == tenTacGiaForm);
            if (biTrung)
            {
                ModelState.AddModelError("TenTacGia", "Tên tác giả này đã tồn tại trong hệ thống!");
                return View(tg);
            }

            db.TACGIAs.Add(tg);
            db.SaveChanges();
            TempData["Success"] = "Thêm tác giả mới thành công!";
            return RedirectToAction("AuthorList");
        }

        // 4. GET: SỬA TÁC GIẢ
        public ActionResult EditAuthor(int id)
        {
            var tg = db.TACGIAs.Find(id);
            if (tg == null) return HttpNotFound();
            return View(tg);
        }

        // 5. POST: SỬA TÁC GIẢ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAuthor(TACGIA tg)
        {
            if (!ModelState.IsValid) return View(tg);

            string tenTacGiaForm = (tg.TenTacGia ?? "").Trim().ToLower();
            bool biTrung = db.TACGIAs.Any(t => t.TenTacGia.Trim().ToLower() == tenTacGiaForm && t.MaTacGia != tg.MaTacGia);

            if (biTrung)
            {
                ModelState.AddModelError("TenTacGia", "Tên tác giả này đã trùng với một tác giả khác trong hệ thống!");
                return View(tg);
            }

            db.Entry(tg).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Success"] = "Cập nhật thông tin tác giả thành công!";
            return RedirectToAction("AuthorList");
        }

        // 6. XÓA TÁC GIẢ 
        public ActionResult DeleteAuthor(int id)
        {
            var tg = db.TACGIAs.Find(id);
            if (tg == null) return HttpNotFound();

            bool daCoSach = db.SACHes.Any(s => s.MaTacGia == id);
            if (daCoSach)
            {
                TempData["Error"] = "Không thể xóa tác giả này";
                return RedirectToAction("AuthorList");
            }

            db.TACGIAs.Remove(tg);
            db.SaveChanges();
            TempData["Success"] = "Xóa tác giả thành công!";
            return RedirectToAction("AuthorList");
        }
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