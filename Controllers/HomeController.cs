using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using products_categories.Models;
using Microsoft.EntityFrameworkCore;

namespace products_categories.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ____________________
        // add product and view all product
        // __________________
        public IActionResult Index()
        {
            ViewBag.allProducts = _context.Products.OrderBy(p => p.Name).ToList();
            return View();
        }


        [HttpPost("addProduct")]
        public IActionResult addProduct(Product newProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newProduct);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.allProducts = _context.Products.OrderBy(p => p.Name).ToList();
                return View("Index");
            }
        }

        // ____________________
        // add category and view all categories
        // __________________
        [HttpGet("categories")]
        public IActionResult Categories()
        {
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View();
        }
        [HttpPost("addCategory")]
        public IActionResult addCategory(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newCategory);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            else
            {
                ViewBag.AllCategories = _context.Categories.OrderBy(p => p.Name).ToList();
                return View("Categories");
            }
        }

        // ____________________
        // render view of just one product and add category to product
        // __________________
        [HttpGet("product/{productId}")]
        public IActionResult OneProduct(int productId)
        {
            Product one = _context.Products.Include(p => p.ProductList).ThenInclude(ti => ti.Category).FirstOrDefault(fd => fd.ProductId == productId);
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View(one);
        }

        [HttpPost("addAssociation")]
        public IActionResult addAssociation(Association newAssociation)
        {
            _context.Add(newAssociation);
            _context.SaveChanges();
            return Redirect($"/product/{newAssociation.ProductId}");
        }
        // ____________________
        // render view of just one category and add product to category
        // __________________
        [HttpGet("category/{categoryid}")]
        public IActionResult OneCategory(int categoryId)
        {
            Category one = _context.Categories.Include(c => c.CategoryList).ThenInclude(ti => ti.Product).FirstOrDefault(fd => fd.CategoryId == categoryId);
            ViewBag.allProducts = _context.Products.OrderBy(s => s.Name).ToList();
            return View(one);
        }
        [HttpPost("addtoCategory")]
        public IActionResult addtoCategory(Association newAssociation)
        {
            _context.Add(newAssociation);
            _context.SaveChanges();
            return Redirect($"/category/{newAssociation.CategoryId}");
        }

        // ____________________
        // Delete routes
        // __________________
        [HttpGet("product/delete/{productId}")]
        public IActionResult deleteProduct(int productId)
        {
            Product pd = _context.Products.SingleOrDefault(s => s.ProductId == productId);
            _context.Products.Remove(pd);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("category/delete/{categoryId}")]
        public IActionResult deleteCategory(int categoryId)
        {
            Category cd = _context.Categories.SingleOrDefault(s => s.CategoryId == categoryId);
            _context.Categories.Remove(cd);
            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        [HttpGet("delete/asso/{cid}/{pid}")]
        public IActionResult DeletefromAss(int cid, int pid)
        {
            Association asso = _context.Associations.SingleOrDefault(d => d.CategoryId == cid && d.ProductId == pid);
            _context.Associations.Remove(asso);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
