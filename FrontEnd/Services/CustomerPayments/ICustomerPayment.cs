using FrontEnd.Models;

namespace FrontEnd.Services.CustomerPayments
{
    public interface ICustomerPayment
    {
        Task<bool> AddMethod(AddPaymentMethodViewModel model);
        Task<bool> Delete(Guid id);
        Task<AddPaymentMethodViewModel> GetPaymentById(Guid id);
        Task<List<CustomerPaymentViewModel>> GetPayments();
        Task<bool> Update(AddPaymentMethodViewModel viewModel, Guid id);
    }
}
