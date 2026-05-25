using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BookStore_65131483.Models;

namespace BookStore_65131483.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        public ActionResult Index()
        {
            return View();
        }
        BookStore_65131483Entities db = new BookStore_65131483Entities();
        // ================= ĐĂNG KÝ =================
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TAIKHOAN tk)
        {
            if (db.TAIKHOANs.Any(x => x.TenDangNhap == tk.TenDangNhap))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại";
                return View();
            }
            if (db.TAIKHOANs.Any(x => x.Email == tk.Email))
            {
                ViewBag.Error = "Email đã tồn tại";
                return View();
            }
            if (db.TAIKHOANs.Any(x => x.SDT == tk.SDT))
            {
                ViewBag.Error = "Số điện thoại đã tồn tại";
                return View();
            }


            tk.MatKhau = HashPassword(tk.MatKhau);
            tk.VaiTro = "user";

            db.TAIKHOANs.Add(tk);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        // ================= ĐĂNG NHẬP =================
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string TenDangNhap, string MatKhau)
        {
            string pass = HashPassword(MatKhau);

            var user = db.TAIKHOANs
                .FirstOrDefault(x => x.TenDangNhap == TenDangNhap && x.MatKhau == pass);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            Session["MaTK"] = user.MaTK;
            Session["TenDangNhap"] = user.TenDangNhap;
            Session["VaiTro"] = user.VaiTro;

            if (user.VaiTro == "user")
            {
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("Dashboard", "TrangAdmin_65131483");
        }

        // ================= ĐĂNG XUẤT =================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // ================= HASH PASSWORD =================
        private string HashPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
        }

    }
}