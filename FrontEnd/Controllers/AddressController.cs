using FrontEnd.Models;
using FrontEnd.Services.Address;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressServices _addressServices;

        public AddressController(IAddressServices addressServices)
        {
            _addressServices = addressServices;
        }
        public async Task<IActionResult> Index()
        {
            List<AddressViewModel> list = await _addressServices.GetUserAddress();
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Add(Guid id,AddAddressViewModel model)
        {
            if(await _addressServices.Add(id,model))
            {
                
                    TempData["Success"] = "Address Added Successfully!";
                    return RedirectToAction("Index", "Address");
                
            }
            TempData["Failure"] = "Failed to Add Address";
            return RedirectToAction("Index", "Address");
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            AddressViewModel model = await _addressServices.GetAddressById(id);
            if(model == null)
            {
                return NotFound();
            }
            ViewBag.AddressId = id;
            return View(model);
        }
        public async Task<IActionResult> Update(Guid id,AddressViewModel model)
        {
            if(await _addressServices.Update(model, id))
            {
                return RedirectToAction("Index", "Address");
            }
            return RedirectToAction("Index", "Address");
        }
        public IActionResult Delete(Guid id)
        {
            _addressServices.Delete(id);
            return RedirectToAction("Index", "Address");
        }
    }
}
