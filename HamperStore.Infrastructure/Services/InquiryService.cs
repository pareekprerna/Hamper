using HamperStore.Core.Entities;
using HamperStore.Core.Interfaces;
using HamperStore.Infrastructure.Data;
using HamperStore.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HamperStore.Infrastructure.Services
{
    public class InquiryService : IInquiryService
    {
        private readonly AppDbContext _context;

        public InquiryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> SubmitAsync(InquiryFormModel model)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == model.Phone);

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Email = model.Email,
                    PreferredCityId = model.CityId
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            else
            {
                customer.Name = model.Name;
                customer.Email = model.Email;
                customer.PreferredCityId = model.CityId;
            }

            // Generate unique random reference ID: INQ{$randomnumber} (6 digits)
            string referenceId;
            bool isUnique = false;
            var random = new Random();
            do
            {
                int randomNumber = random.Next(100000, 999999);
                referenceId = $"INQ{randomNumber}";
                var exists = await _context.Inquiries.AnyAsync(i => i.ReferenceId == referenceId);
                if (!exists)
                {
                    isUnique = true;
                }
            } while (!isUnique);

            var inquiry = new Inquiry
            {
                ReferenceId = referenceId,
                CustomerId = customer.Id,
                HamperId = model.HamperId,
                CityId = model.CityId,
                Occasion = model.Occasion,
                Budget = model.Budget,
                Message = model.Message,
                Status = InquiryStatus.New,
                CreatedAt = DateTime.UtcNow
            };

            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();
            return inquiry.ReferenceId;
        }
    }
}
