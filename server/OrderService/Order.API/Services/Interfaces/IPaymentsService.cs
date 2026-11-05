using Order.API.Dtos.Sepay;

namespace Order.API.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task CheckPayment(SepayWebhookRequestDto request);
        Task<byte[]> GenerateQR(Guid orderId);
    }
}
