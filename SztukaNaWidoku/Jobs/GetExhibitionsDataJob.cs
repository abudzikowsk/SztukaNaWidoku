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
        var exhibitionsFlatten = exhibitions.SelectMany(i => i).ToList();
        foreach (var exhibition in exhibitionsFlatten)
        {
            var counter = 0;
            var stringBuilder = new StringBuilder();
            foreach (var character in exhibition.Description)
            {
                stringBuilder.Append(character);
                if (character == '.')
                {
                    counter++;
                }

                if (counter == 4)
                {
                    break;
                }
            }

            exhibition.Description = stringBuilder.ToString();
        }

        await exhibitionRepository.CreateMany(exhibitions.SelectMany(i => i).ToList());
    }
}