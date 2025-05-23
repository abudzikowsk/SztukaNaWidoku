using System.Text;
using SztukaNaWidoku.Database.Repositories;
using SztukaNaWidoku.Services;

namespace SztukaNaWidoku.Jobs;

public class GetExhibitionsDataJob(ExhibitionRepository exhibitionRepository,
    ScrappingMNWService scrappingMNWService, 
    ScrappingMSNService scrappingMSNService, 
    ScrappingPGSService scrappingPGSService,
    ScrappingUjazdowskiService scrappingUjazdowskiService,
    ScrappingZachetaService scrappingZachetaService,
    ScrappingCSWLazniaService scrappingCSWLazniaService,
    ScrappingMMGService scrappingMMGService,
    ScrappingMuzeumNarodoweGdansk scrappingMuzeumNarodoweGdansk)
{
    public async Task Run()
    {
        var exhibitions = await Task.WhenAll(
            scrappingMNWService.Scrap(),
            scrappingMSNService.Scrap(),
            scrappingPGSService.Scrap(),
            scrappingUjazdowskiService.Scrap(),
            scrappingZachetaService.Scrap(),
            scrappingCSWLazniaService.Scrap(),
            scrappingMMGService.Scrap(), 
            scrappingMuzeumNarodoweGdansk.Scrap()
        );

        await exhibitionRepository.CreateMany(exhibitions.SelectMany(i => i).ToList());
    }
}