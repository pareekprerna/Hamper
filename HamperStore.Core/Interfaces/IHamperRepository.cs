using HamperStore.Core.Entities;

namespace HamperStore.Core.Interfaces
{
    public interface IHamperRepository
    {
        Task<List<Hamper>> GetAllAsync(int? cityId = null, int? categoryId = null, bool includeInactive = false);
        Task<Hamper?> GetBySlugAsync(string slug);
        Task<Hamper?> GetByIdAsync(int id);
        Task<List<Category>> GetCategoriesAsync();
        Task<List<City>> GetCitiesAsync();
        Task AddHamperAsync(Hamper hamper);
        Task UpdateHamperAsync(Hamper hamper);
        Task AddCategoryAsync(Category category);
    }
}
