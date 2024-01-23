using SztukaNaWidoku.Database.Repositories;
using SztukaNaWidoku.Services;

namespace SztukaNaWidoku.Jobs;

public class DeleteAllExhibitionsDataJob(ExhibitionRepository exhibitionRepository)
{
    public async Task Run()
    {
        await exhibitionRepository.DeleteAll();
    }
}