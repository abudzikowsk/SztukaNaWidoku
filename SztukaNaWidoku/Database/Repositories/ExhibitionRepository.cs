using Microsoft.EntityFrameworkCore;
using SztukaNaWidoku.Database.Entities;

namespace SztukaNaWidoku.Database.Repositories;

public class ExhibitionRepository(ApplicationDbContext _applicationDbContext)
{
    public async Task<Exhibition[]> GetAll()
    {
        return await _applicationDbContext.Exhibitions.ToArrayAsync();
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