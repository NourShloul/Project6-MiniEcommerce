using NourStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NourStore.Controllers
{
    public class CartController : Controller
    {
        private NourStoreProject6Entities1 db = new NourStoreProject6Entities1();

        public ActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        public ActionResult AddToCart(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(c => c.ProductID == id);

                if (item != null)
                {
                    item.Quantity++;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                SetCart(cart);
            }

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductID == id);

            if (item != null)
            {
                cart.Remove(item);
                SetCart(cart);
            }

            return RedirectToAction("Index");
        }

        private List<CartItem> GetCart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        private void SetCart(List<CartItem> cart)
        {
            Session["Cart"] = cart;
        }
    }

    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}