using LTWebBanDT.Context;
using LTWebBanDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWebBanDT.Controllers
{
    public class CartController : Controller
    {
        LTWebBanHangEntities objLTWebBanHangEntities = new LTWebBanHangEntities();
        // GET: Cart
        public ActionResult Index()
        {
            return View((List<CartModel>)Session["cart"]); //lấy lại session truyền xuống csdl rồi load dữ liệu đặt hàng
        }

        public ActionResult AddToCart(int id, int quantity) //Nhận đc 2 giá trị khi lấy từ Id ajax truyền về controller
        {
            if (Session["cart"] == null) //kiểm tra giỏ hàng xem có bằng null không
            {
                List<CartModel> cart = new List<CartModel>();
                cart.Add(new CartModel { Product = objLTWebBanHangEntities.Products.Find(id), Quantity = quantity }); //dựa vào Id xuống csdl lấy sản phẩm lên theo id sau đó add vào danh sách cartModel
                Session["cart"] = cart; //Lưu cart vào session
                Session["count"] = 1; //tạo session cao hiển thị số lượng
            }
            else
            {
                List<CartModel> cart = (List<CartModel>)Session["cart"];
                //Kiểm tra sản phẩm có tồn tại trong giỏ hàng chưa
                int index = isExist(id);
                if (index != -1)
                {
                    //Nếu sản phẩm tồn tại trong giỏ hàng thì cộng thêm số lượng
                    cart[index].Quantity += quantity;
                }
                else
                {
                    //Nếu không tồn tại thì thêm sản phẩm vào giỏ hàng
                    cart.Add(new CartModel { Product = objLTWebBanHangEntities.Products.Find(id), Quantity = quantity });
                    //Tính lại số sản phẩm trong giỏ hàng
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
                Session["cart"] = cart; //lưu vào session cart
            }
            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }

        private int isExist(int id)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.Id.Equals(id))
                    return i;
            return -1;
        }

        //Xóa sản phẩm khỏi giỏ hàng theo id
        public ActionResult Remove(int Id)
        {
            List<CartModel> li = (List<CartModel>)Session["cart"]; //Lấy giá trị session sau đó gán vào li
            li.RemoveAll(x => x.Product.Id == Id); //sau đó xóa sản phẩm trong cart
            Session["cart"] = li; //truyền session cart vào li để lấy dữ liệu xóa
            Session["count"] = Convert.ToInt32(Session["count"]) - 1; //trừ vào count theo số lượng xóa
            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }
    }
}