using Microsoft.AspNetCore.Mvc;
using SztukaNaWidoku.Database.Repositories;
using SztukaNaWidoku.Models;
using SztukaNaWidoku.Services;

namespace SztukaNaWidoku.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExhibitionsController(ExhibitionRepository exhibitionRepository) : ControllerBase
{
    [HttpGet]
    public async Task<List<ExhibitionModel>> GetAllExhibitionsData()
    {
        var data = await exhibitionRepository.GetAll();
        
        var mappedData = new List<ExhibitionModel>();
        
        foreach (var d in data)
        {
            mappedData.Add(new ExhibitionModel
            {
                Id = d.Id,
                Link = d.Link,
                Title = d.Title,
                Date = d.Date,
                ImageLink = d.ImageLink,
                Description = d.Description,
                MuseoName = d.Museo.Name
            });
        }
        return mappedData;
    }
}