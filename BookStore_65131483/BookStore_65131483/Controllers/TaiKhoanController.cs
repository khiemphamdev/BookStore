using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
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

        public ActionResult TaiKhoan()
        {
            var maTK = GetMaTK();
            if (maTK == null)
                return RedirectToAction("Login", "TaiKhoan_65131483");

            var tk = db.TAIKHOANs.Find(maTK);
            return View(tk);
        }
        [HttpPost]
        public ActionResult TaiKhoan(TAIKHOAN model, HttpPostedFileBase Avatar)
        {
            var maTK = GetMaTK();
            if (maTK == null)
                return RedirectToAction("Login", "TaiKhoan_65131483");

            var tk = db.TAIKHOANs.Find(maTK);

            if (model.HoTen != null)
                tk.HoTen = model.HoTen;
            if (model.Email != null)
                tk.Email = model.Email;
            if (model.SDT != null)
                tk.SDT = model.SDT;
            if (model.DiaChi != null)
                tk.DiaChi = model.DiaChi;

            // Upload avatar
            //if (Avatar != null && Avatar.ContentLength > 0)
            //{
            //    string fileName = System.IO.Path.GetFileName(Avatar.FileName);
            //    string path = Server.MapPath("~/Content/Avatar/" + fileName);
            //    Avatar.SaveAs(path);
            //    tk. = fileName;
            //}

            db.SaveChanges();
            ViewBag.Success = "Cập nhật thành công";
            return View(tk);
        }


        // Lấy mã tài khoản đang đăng nhập
        private int? GetMaTK()
        {
            return Session["MaTK"] as int?;
        }
        public ActionResult DonHang()
        {
            var maTK = GetMaTK();
            if (maTK == null)
                return RedirectToAction("Login", "TaiKhoan_65131483");

            var donHangs = db.DONHANGs
                .Include(d => d.CHITIETDONHANGs)
                .Where(d => d.MaTK == maTK && d.TrangThai != "TrongGioHang" && d.TrangThai != "Đã hủy")
                .OrderByDescending(d => d.NgayDat)
                .ToList();

            return View(donHangs);
        }
        public ActionResult ChiTietDonHang(int maDH)
        {
            var ctdh = db.CHITIETDONHANGs
                .Include(d => d.SACH)
                .Where(d => d.MaDH == maDH)
                .ToList();

            return View(ctdh);
        }
        public ActionResult HuyDon(int maDH)
        {
            var donHang = db.DONHANGs.Find(maDH);
            db.DONHANGs.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("DonHang", "TaiKhoan_65131483");
        }
        

    }
}