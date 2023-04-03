using LTWebBanDT.Context;
using LTWebBanDT.List;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static LTWebBanDT.List.Common;

namespace LTWebBanDT.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        LTWebBanHangEntities objLTWebBanHangEntities = new LTWebBanHangEntities();

        // GET: Admin/Product
        public ActionResult Index(string currentFilter, string SearchString, int? page) //khi nhấn button tìm kiếm sẽ gọi qua index của controller, searchstring là dữ liệu tìm kiếm,page dữ liệu khi người dùng nhấn vào trang
        {
            var lstProduct = new List<Product>(); //tạo danh sách sản phẩm
            if( SearchString != null) //nếu không tìm kiếm thì sẽ để page bằng 1
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;//nhấn tìm kiếm thì bên view sẽ trả về controller và biến currentfilter sẽ tìm kiếm với searching
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                //Lấy danh sách sản phẩm theo từ khóa tìm kiếm
                lstProduct = objLTWebBanHangEntities.Products.Where(n => n.Name.Contains(SearchString)).ToList();
            }
            else
            {
                //Lấy tất cả sản phẩm trong bảng Product
                lstProduct = objLTWebBanHangEntities.Products.ToList();
            }
            ViewBag.CurrentFilter = SearchString;
            //Số lượng item của 1 trang = 4
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            //Sắp xếp theo id sản phẩm, sản phẩm mới đưa lên đầu
            lstProduct = lstProduct.OrderByDescending(n => n.Id).ToList();
            return View(lstProduct.ToPagedList(pageNumber, pageSize)); //truyền number và size vào pagedlist
        }

        [HttpGet]
        public ActionResult Create()
        {
            this.LoadData();
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(Product objProduct) //lấy đc dữ liệu sản phẩm
        {
            this.LoadData(); //load dữ liệu
            if (ModelState.IsValid)
            {
                try
                {
                    if (objProduct.ImageUpload != null) //kiểm tra hình ảnh xem có up hình lên không
                    {
                        string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload.FileName);
                        //tên hình
                        string extension = Path.GetExtension(objProduct.ImageUpload.FileName);
                        //png
                        fileName = fileName + extension;
                        //tenhinh.png
                        objProduct.Avartar = fileName;
                        objProduct.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images"), fileName)); //lấy đc hình ảnh sau đó lưu vào đường dẫn
                    }
                    objProduct.CreatedOnUtc = DateTime.Now; //gán thời gian hiện tại
                    objLTWebBanHangEntities.Products.Add(objProduct); //thêm sản phẩm
                    objLTWebBanHangEntities.SaveChanges(); //lưu dữ liệu vào csdl

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            return View(objProduct);
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            var objProduct = objLTWebBanHangEntities.Products.Where(n => n.Id == id).FirstOrDefault();
            return View(objProduct);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objProduct = objLTWebBanHangEntities.Products.Where(n => n.Id == id).FirstOrDefault();
            return View(objProduct);
        }
        [HttpPost]
        public ActionResult Delete(Product objPro)
        {
            var objProduct = objLTWebBanHangEntities.Products.Where(n => n.Id == objPro.Id).FirstOrDefault();

            objLTWebBanHangEntities.Products.Remove(objProduct); //xóa sản phẩm khỏi bảng product
            objLTWebBanHangEntities.SaveChanges(); //lưu vào csdl
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var objProduct = objLTWebBanHangEntities.Products.Where(n => n.Id == id).FirstOrDefault();
            return View(objProduct);
        }
        [HttpPost]
        public ActionResult Edit( Product objProduct)
        {
            if (objProduct.ImageUpload != null) //hình ảnh null thì sẽ không hiển thị
            {
                string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload.FileName);
                string extension = Path.GetExtension(objProduct.ImageUpload.FileName);
                fileName = fileName + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                objProduct.Avartar = fileName;
                objProduct.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), fileName));
            }
            objLTWebBanHangEntities.Entry(objProduct).State = EntityState.Modified;//thay đổi dữ liệu và sau đó cập nhật vào bảng
            objLTWebBanHangEntities.SaveChanges(); //lưu vào csdl
            return RedirectToAction("Index");
        }

        void LoadData()
        {
            Common objCommon = new Common();
        //Lấy dữ liệu danh mục dưới DB
        var lstCat = objLTWebBanHangEntities.Categories.ToList();
        //Convert sang select list dạng value, text
        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        DataTable dtCategory = converter.ToDataTable(lstCat);
        ViewBag.ListCategory = objCommon.ToSelectList(dtCategory, "Id", "Name");

            //Lấy dữ liệu thương hiệu dưới DB
            var lstBrand = objLTWebBanHangEntities.Brands.ToList();
        DataTable dtBrand = converter.ToDataTable(lstBrand);
        //Convert sang select list dạng value, text
        ViewBag.ListBrand = objCommon.ToSelectList(dtBrand, "Id", "Name");

            //Loại sản phẩm
            List<ProductType> lstProductType = new List<ProductType>();
        ProductType objProductType = new ProductType();
        objProductType.Id = 01;
            objProductType.Name = "Giảm giá sốc";
            lstProductType.Add(objProductType);

            objProductType = new ProductType();
        objProductType.Id = 02;
            objProductType.Name = "Đề xuất";
            lstProductType.Add(objProductType);

            DataTable dtProductType = converter.ToDataTable(lstProductType);
        //Convert sang select list dạng value, text
        ViewBag.ProductType = objCommon.ToSelectList(dtProductType, "Id", "Name");
        }
    }
}