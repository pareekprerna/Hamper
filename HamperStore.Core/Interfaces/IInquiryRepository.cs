using HamperStore.Core.Entities;

namespace HamperStore.Core.Interfaces
{
    public interface IInquiryRepository
    {
        Task<Inquiry> CreateAsync(Inquiry inquiry);
        Task<List<Inquiry>> GetAllAsync();
        Task<Inquiry?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, InquiryStatus status);
        Task<List<Inquiry>> GetByCustomerPhoneAsync(string phone);
    }
}
