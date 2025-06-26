using NguyenManhDuc.WebApp.Models.MoMo;

namespace NguyenManhDuc.WebApp.Services.MoMo
{
    public interface IMoMoService
    {
        Task<MoMoResponseModel> CreatePaymentAsync(MoMoInformationModel model);
        MoMoInformationModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
