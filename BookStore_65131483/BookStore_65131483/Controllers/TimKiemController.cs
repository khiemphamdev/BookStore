using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using BookStore_65131483.Models;
using System.Web.UI;
using System.Data.SqlTypes;
using System.Text;

namespace BookStore_65131483.Controllers
{
    public class TimKiemController : Controller
    {
        BookStore_65131483Entities db = new BookStore_65131483Entities();


        public ActionResult TimKiem(string keyword, int? page)
        {
            int pageSize = 6;            // Số sách mỗi trang
            int pageNumber = page ?? 1;   // Nếu không truyền page thì mặc định 1

            // Lấy dữ liệu sách
            var sach = db.SACHes.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                string searchKeyword = keyword.Trim().Normalize(NormalizationForm.FormC).ToLower();

                sach = sach.Where(s =>
                    s.TieuDe.ToLower().Contains(searchKeyword) ||
                    s.TACGIA.TenTacGia.ToLower().Contains(searchKeyword)
                    );
            }
            sach = sach.Where(s => s.IsActive == true);

            // Sắp xếp theo tên sách
            IPagedList<BookStore_65131483.Models.SACH> pagedSach =
       sach.OrderBy(s => s.NgayTao).ToPagedList(pageNumber, pageSize);

            return View(pagedSach);
        }

    }
}
