using Microsoft.EntityFrameworkCore;
using SztukaNaWidoku.Database.Entities;
using SztukaNaWidoku.Enums;

namespace SztukaNaWidoku.Database.Repositories;

public class ExhibitionRepository(ApplicationDbContext _applicationDbContext)
{
    public async Task<Exhibition[]> GetAllByCityId(Cities? city)
    {
        var exhibitions = await _applicationDbContext.Exhibitions.Include(m => m.Museo)
            .ToArrayAsync();

        return city != null ? exhibitions.Where(x => x.Museo.MapToCity() == city).ToArray() : exhibitions;
    }
    
    public async Task CreateMany(List<Exhibition> exhibitions)
    {
        _applicationDbContext.Exhibitions.AddRange(exhibitions);
        await _applicationDbContext.SaveChangesAsync(); 
    }

    public async Task DeleteAll()
    {
        var exhibitions = await _applicationDbContext.Exhibitions.ToArrayAsync();
        _applicationDbContext.Exhibitions.RemoveRange(exhibitions);
        await _applicationDbContext.SaveChangesAsync();
    }   
}