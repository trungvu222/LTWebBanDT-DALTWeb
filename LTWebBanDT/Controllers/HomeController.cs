using LTWebBanDT.Context;
using LTWebBanDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LTWebBanDT.Controllers
{
    public class HomeController : Controller
    {
        LTWebBanHangEntities objLTWebBanHangEntities = new LTWebBanHangEntities();//Chứa thông tin kết nối csdl để kết nối
        public ActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();

            objHomeModel.ListCategory = objLTWebBanHangEntities.Categories.ToList();
            objHomeModel.ListProduct = objLTWebBanHangEntities.Products.ToList();
            return View(objHomeModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            //kiem tra va luu vao db
            if (ModelState.IsValid)
            {
                var check = objLTWebBanHangEntities.Users.FirstOrDefault(s => s.Email == _user.Email);//kiểm tra xem form có hợp lệ không,sẽ lấy email dưới csdl lên
                if (check == null) //email chưa tồn tại thì sẽ cho đky mới
                {
                    _user.Password = GetMD5(_user.Password);//sau khi tạo mật khẩu sẽ ãm hóa thành một chuỗi,bảo mật thông tin người dùng khi đăng nhập hệ thống
                    objLTWebBanHangEntities.Configuration.ValidateOnSaveEnabled = false;//mã hóa mật khẩu md5
                    objLTWebBanHangEntities.Users.Add(_user);//thêm user vào model
                    objLTWebBanHangEntities.SaveChanges();//lưu vào csdl
                    return RedirectToAction("Index");//trở về trang chủ
                }
                else
                {
                    ViewBag.error = "Email already exists";//email đã tồn tại trong csdl và sẽ báo tạo lại
                    return View();
                }
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)//nhận thông tin nhập từ trang login
        {
            if (ModelState.IsValid)
            {


                var f_password = GetMD5(password);//mật khẩu sẽ được mã hóa thành một chuỗi
                var data = objLTWebBanHangEntities.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();//so sánh email và mật khẩu có tồn tại trong hệ thống không
                if (data.Count() > 0) //nếu tồn tại trong hệ thống
                {
                    //sẽ lưu dữ liệu vào session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().Id;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//sau khi đăng xuất sẽ xóa session vừa khởi tạo
            return RedirectToAction("Login");//trở về trang đăng nhập
        }

        //tạo mã hóa mật khẩu MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}