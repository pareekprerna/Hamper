using HamperStore.Core.Entities;
using HamperStore.Core.Interfaces;
using HamperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HamperStore.Infrastructure.Repositories
{
    public class HamperRepository : IHamperRepository
    {
        private readonly AppDbContext _context;

        public HamperRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hamper>> GetAllAsync(int? cityId = null, int? categoryId = null, bool includeInactive = false)
        {
            var query = _context.Hampers
                .Include(h => h.Category)
                .Include(h => h.Images)
                .Include(h => h.AvailableCities)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(h => h.IsActive);
            }

            if (cityId.HasValue)
                query = query.Where(h => h.AvailableCities.Any(c => c.Id == cityId));

            if (categoryId.HasValue)
                query = query.Where(h => h.CategoryId == categoryId);

            return await query.OrderBy(h => h.Name).ToListAsync();
        }

        public async Task<Hamper?> GetBySlugAsync(string slug)
        {
            return await _context.Hampers
                .Include(h => h.Category)
                .Include(h => h.Images)
                .Include(h => h.Items)
                .Include(h => h.AvailableCities)
                .FirstOrDefaultAsync(h => h.Slug == slug && h.IsActive);
        }

        public async Task<Hamper?> GetByIdAsync(int id)
        {
            return await _context.Hampers
                .Include(h => h.Category)
                .Include(h => h.Images)
                .Include(h => h.Items)
                .Include(h => h.AvailableCities)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<List<City>> GetCitiesAsync()
        {
            return await _context.Cities.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        }

        public async Task AddHamperAsync(Hamper hamper)
        {
            _context.Hampers.Add(hamper);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHamperAsync(Hamper hamper)
        {
            var existing = await _context.Hampers
                .Include(h => h.Items)
                .Include(h => h.AvailableCities)
                .Include(h => h.Images)
                .FirstOrDefaultAsync(h => h.Id == hamper.Id);

            if (existing != null)
            {
                // If they are the same instance, EF Core is already tracking all changes.
                // We just save changes and return to avoid deleting and losing collections.
                if (ReferenceEquals(existing, hamper))
                {
                    await _context.SaveChangesAsync();
                    return;
                }

                // Update scalar properties
                _context.Entry(existing).CurrentValues.SetValues(hamper);

                // Update standard items
                _context.HamperItems.RemoveRange(existing.Items);
                existing.Items = hamper.Items;

                // Update city connections
                existing.AvailableCities.Clear();
                foreach (var city in hamper.AvailableCities)
                {
                    var trackedCity = await _context.Cities.FindAsync(city.Id);
                    if (trackedCity != null)
                    {
                        existing.AvailableCities.Add(trackedCity);
                    }
                }

                // Update images
                if (hamper.Images != null && hamper.Images.Any())
                {
                    _context.HamperImages.RemoveRange(existing.Images);
                    existing.Images = hamper.Images;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
    }
}
