using UserManagement.API.Dtos;
using Domain.Entities;
using UserManagement.API.Dtos.Notification;
namespace UserManagement.API.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<IEnumerable<VoucherDto>> GetAllVouchers();
        Task<bool> CreateVoucher(CreateNewVoucherDto createVoucherDto);
        Task<bool> DeleteVoucher(Guid id);
        Task<bool> UpdateVoucher(Guid id,UpdateVoucherDto updateVoucherDto);
       Task<IEnumerable<VoucherUsagesDto>> GetAllVoucherUseds();
        //user 
        Task<IEnumerable<VoucherUserDto>> GetUserVouchers(Guid userId); 

    }
}
