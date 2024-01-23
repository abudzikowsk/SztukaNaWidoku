using Microsoft.EntityFrameworkCore;
using SztukaNaWidoku.Database.Entities;

namespace SztukaNaWidoku.Database.Repositories;

public class ExhibitionRepository(ApplicationDbContext applicationDbContext)
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
    
    public async Task<Exhibition[]> GetAllByMuseo(int museoId)
    {
        return await _applicationDbContext.Exhibitions
            .Where(i => i.MuseoId == museoId)
            .ToArrayAsync();
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