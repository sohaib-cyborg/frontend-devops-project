using FrontEnd.Models;

namespace FrontEnd.Services.Products
{
    public interface IProductServices
    {
        Task<bool> Add(AddProductViewModel model);
        void Delete(Guid id);
        Task<ProductViewModel> FindById(Guid id);
        Task<List<ProductViewModel>> GetProducts();
        Task<bool> Update(Guid id, AddProductViewModel model);
    }
}
