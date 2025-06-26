using NguyenManhDuc.WebApp.Models.VNPay;

namespace NguyenManhDuc.WebApp.Services.VNPay
{
    public interface IVNPayService
    {
        Task<string> CreatePaymentAsync(VNPayInformationModel model, HttpContext context);
        Task<VNPayResponseModel> PaymentExecuteAsync(IQueryCollection collections);
    }
}
