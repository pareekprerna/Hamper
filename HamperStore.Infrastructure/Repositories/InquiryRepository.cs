using HamperStore.Core.Entities;
using HamperStore.Core.Interfaces;
using HamperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HamperStore.Infrastructure.Repositories
{
    public class InquiryRepository : IInquiryRepository
    {
        private readonly AppDbContext _context;

        public InquiryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Inquiry> CreateAsync(Inquiry inquiry)
        {
            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();
            return inquiry;
        }

        public async Task<List<Inquiry>> GetAllAsync()
        {
            return await _context.Inquiries
                .Include(i => i.Customer)
                .Include(i => i.Hamper)
                .Include(i => i.City)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<Inquiry?> GetByIdAsync(int id)
        {
            return await _context.Inquiries
                .Include(i => i.Customer)
                .Include(i => i.Hamper)
                .Include(i => i.City)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task UpdateStatusAsync(int id, InquiryStatus status)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                inquiry.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Inquiry>> GetByCustomerPhoneAsync(string phone)
        {
            return await _context.Inquiries
                .Include(i => i.Customer)
                .Include(i => i.Hamper)
                .Include(i => i.City)
                .Where(i => i.Customer.Phone == phone)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
    }
}
