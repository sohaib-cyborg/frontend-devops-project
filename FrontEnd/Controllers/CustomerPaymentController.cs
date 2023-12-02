using FrontEnd.Models;
using FrontEnd.Services.CustomerPayments;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class CustomerPaymentController : Controller
    {
        private readonly ICustomerPayment _customerPayment;

        public CustomerPaymentController(ICustomerPayment customerPayment)
        {
            _customerPayment = customerPayment;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _customerPayment.GetPayments();
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
        public async Task<IActionResult> Add(AddPaymentMethodViewModel model)
        {
            if (ModelState.IsValid) {
                if (await _customerPayment.AddMethod(model))
                {
                        TempData["Success"] = "Payment Method Added Successfully!";
                        return RedirectToAction("Index","Cart");
                    
                }
                TempData["Failure"] = "Payment method addition failed!";
                return RedirectToAction("Index","CustomerPayment");
            }
            return NotFound();
        }
        public async Task<IActionResult> Edit(Guid id) 
        {
            var model =await _customerPayment.GetPaymentById(id);
            ViewBag.PaymentMethodId = id;
            return View(model);
        }
        public async Task<IActionResult> Update(Guid id, AddPaymentMethodViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if(await _customerPayment.Update(viewModel, id))
                {
                    return RedirectToAction("Index", "CustomerPayment");
                }
                return NotFound();
            }
            return NotFound();
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            if(await _customerPayment.Delete(id))
            {
                return RedirectToAction("Index", "CustomerPayment");
            }
            return NotFound(ModelState);
        }
    }
}
