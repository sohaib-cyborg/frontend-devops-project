using FrontEnd.Models;

namespace FrontEnd.Services
{
    public interface IAuthServices
    {
        Task<bool> create(RegistrationViewModel model);
        Task<UserViewModel> Details();
        Task<List<UserViewModel>> GetCustomers();
        Task<bool> Login(LoginRequestViewModel model);
        void Logout();
        Task<bool> Update(UserViewModel model);
    }
}
