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
                //Gán dữ liệu cho order
                Order objOrder = new Order();
                objOrder.Name = "DonHang-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                objOrder.UserId = int.Parse(Session["idUser"].ToString());
                objOrder.CreatedOnUtc = DateTime.Now;
                objOrder.Status = 1;
                objLTWebBanHangEntities.Orders.Add(objOrder);
                //Lưu thông tin dữ liệu vào bảng order
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

        public ActionResult Order(string Email, string Phone)
        {
            List<CartModel> cart = Session["cart"] as List<CartModel>;
            string sMsg = "<html><body><table border='1'><caption>Thông tin đặt hàng</caption><tr><th>STT</th><th>Tên hàng</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr>";
            int i = 0;
            double tongtien = 0;
            foreach (CartModel item in cart)
            {
                i++;
                sMsg += "<tr>";
                sMsg += "<td>" + i.ToString() + "</td>";
                sMsg += "<td>" + item.Product + "</td>";
                sMsg += "<td>" + item.Quantity.ToString() + "</td>";
                sMsg += "<td>" + item.Quantity.ToString() + "</td>";
                sMsg += "<td>" + String.Format("{0:#,###}", item.Quantity * item.Quantity) + "</td>";
                sMsg += "<tr>";
                tongtien += item.Quantity * item.Quantity;
            }
            //Gửi email cho khách hàng
            sMsg += "<tr><th colspan='5'>Tổng cộng: "
                + String.Format("{0:#,### vnđ}", tongtien) + "</th></tr></table>";
            MailMessage mail = new MailMessage("vuthanhtrung385@gmail.com", Email, "Thông tin đơn hàng", sMsg);
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("vuthanhtrung385@gmail.com", "rpponiqakrmtmvsn");
            mail.IsBodyHtml = true;
            client.Send(mail);
            return RedirectToAction("Index", "Home");
        }
    }
}