using Microsoft.EntityFrameworkCore;
using SztukaNaWidoku.Database;
using SztukaNaWidoku.Database.Entities;

namespace Bursztynorama.Database.Repositories;

public class ExhibitionRepository(ApplicationDbContext applicationDbContext)
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
    
    public async Task<Exhibition[]> GetAllByMuseo(int museoId)
    {
        return await _applicationDbContext.Exhibitions
            .Where(i => i.MuseoId == museoId)
            .ToArrayAsync();
    }
    
    public async Task Create(Exhibition exhibition)
    {
        _applicationDbContext.Exhibitions.Add(exhibition);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task Delete(Exhibition exhibition)
    {
        _applicationDbContext.Exhibitions.Remove(exhibition);
    }
}