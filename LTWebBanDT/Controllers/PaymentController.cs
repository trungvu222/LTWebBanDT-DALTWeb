using LTWebBanDT.Context;
using LTWebBanDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LTWebBanDT.Controllers
{
    public class PaymentController : Controller
    {
        LTWebBanHangEntities objLTWebBanHangEntities = new LTWebBanHangEntities();
        // GET: Payment
        public ActionResult Index()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Home"); 
            }
            else
            {
                //Lấy thông tin từ giỏ hàng từ biến session
                var lstCart = (List<CartModel>)Session["cart"];
                //Gán dữ liệu cho bảng order
                Order objOrder = new Order();
                objOrder.Name = "DonHang-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                objOrder.UserId = int.Parse(Session["idUser"].ToString()); 
                objOrder.CreatedOnUtc = DateTime.Now;
                objOrder.Status = 1;
                objLTWebBanHangEntities.Orders.Add(objOrder);
                objLTWebBanHangEntities.SaveChanges();

                //Lấy OrderId vừa mới tạo để lưu vào bảng OrderDetail
                int intOrderId = objOrder.Id;

                List<OrderDetail> lstOrderDetail = new List<OrderDetail>(); 

                foreach (var item in lstCart) 
                {
                    OrderDetail obj = new OrderDetail();
                    obj.Quantity = item.Quantity;
                    obj.OrderId = intOrderId;
                    obj.ProductId = item.Product.Id;
                    lstOrderDetail.Add(obj);
                }
                objLTWebBanHangEntities.OrderDetails.AddRange(lstOrderDetail);
                objLTWebBanHangEntities.SaveChanges();
            }
            return View();
        }
    }
}