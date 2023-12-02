using FrontEnd.Models;

namespace FrontEnd.Services.Address
{
    public interface IAddressServices
    {
        Task<bool> Add(Guid id, AddAddressViewModel model);
        void Delete(Guid id);
        Task<AddressViewModel> GetAddressById(Guid id);
        Task<List<AddressViewModel>> GetUserAddress();
        Task<bool> Update(AddressViewModel model, Guid id);
    }
}
