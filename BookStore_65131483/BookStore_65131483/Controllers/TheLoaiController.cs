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
            if (ModelState.IsValid)
            {
                db.THELOAIs.Add(tl);
                db.SaveChanges();
                return RedirectToAction("DsTheLoai");
            }
            return View(tl);
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
            if (ModelState.IsValid)
            {
                db.Entry(tl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DsTheLoai");
            }
            return View(tl);
        }

        // Xóa danh mục
        public ActionResult XoaTheLoai(int id)
        {
            var tl = db.THELOAIs.Find(id);
            if (tl == null) return HttpNotFound();
            db.THELOAIs.Remove(tl);
            db.SaveChanges();
            return RedirectToAction("DsTheLoai");
        }
    }
}