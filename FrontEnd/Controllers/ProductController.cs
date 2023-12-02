using FrontEnd.Models;
using FrontEnd.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }
        public async Task<IActionResult> Index()
        {
             List<ProductViewModel> list = await _productServices.GetProducts();
            if(list == null)
            {
                return NotFound();
            }
            return View(list);
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Add(AddProductViewModel model)
        {
            if (await _productServices.Add(model))
            {
                return RedirectToAction("Index","Product");
            }
            return RedirectToAction("Index", "Product");
        }
        public async Task<IActionResult> Update(Guid id)
        {
            var model = await _productServices.FindById(id);
            AddProductViewModel prod = new AddProductViewModel()
            {
                name = model.name,
                description = model.description,
                category = model.category,
                price = model.price,
                quantity = model.quantity,
            };
            ViewBag.ProductId = id;
            return View(prod);
        }
        public async Task<IActionResult> Edit(Guid id,AddProductViewModel model)
        {
            if (await _productServices.Update(id,model))
            {
                return RedirectToAction("Index", "Product");
            }
            return RedirectToAction("Index", "Product");
        }
        public IActionResult Delete(Guid id)
        {
            _productServices.Delete(id);
            return RedirectToAction("Index", "Product");
        }
        public async Task<IActionResult> ShowProduct(Guid id)
        {
            var prod = await _productServices.FindById(id);
            if(prod != null)
            {
                return View(prod);
            }
            TempData["Failure"] = "Product Not Found Please Try Again!";
            return RedirectToAction("Index","Home");
        }
    }
}
