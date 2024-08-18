using NourStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NourStore.Controllers
{
    public class ProductsController : Controller
    {
        private NourStoreProject6Entities1 db = new NourStoreProject6Entities1();

        //public ActionResult Shop()
        //{
        //    var products = db.Products.ToList(); // Retrieves all products from the database
        //    return View(products);
        //}
        public ActionResult Shop(string category, decimal? minPrice, decimal? maxPrice)
        {
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.CategoryName == category);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            var categories = db.Categories.Select(c => c.CategoryName).ToList();
            ViewBag.Categories = new SelectList(categories);

            return View(products.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

    }
}