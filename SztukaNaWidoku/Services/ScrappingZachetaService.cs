using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;
using SztukaNaWidoku.Database.Enums;

namespace SztukaNaWidoku.Services;

public class ScrappingZachetaService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://zacheta.art.pl";

    //Zachęta Narodowa Galeria Sztuki
    public async Task<List<Exhibition>> Scrap()
    {
        var html = await httpClient.GetStringAsync(baseUrl + "/pl/wystawy");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//a[@class='list-item-link']");

        var exhibitions = new List<Exhibition>();
        if(nodes == null)
        {
            logger.LogWarning("Nodes not found.");
            return exhibitions;
        }
        
        foreach (var node in nodes)
        {
            var exhibitionLink = node.Attributes["href"].Value;
            var exhibitionHtml = await httpClient.GetStringAsync($"{baseUrl}{exhibitionLink}");
            var exhibitionHtmlDocument = new HtmlDocument();
            exhibitionHtmlDocument.LoadHtml(exhibitionHtml);
    
            var titleNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode("//strong[@class='exhibition-name']");
            if(titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
            
            var dateNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode("//span[@class='exhibition-date']");
            if(dateNode == null)
            {
                logger.LogWarning($"Date for {exhibitionLink} not found.");
                continue;
            }
            
            var imgNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode("//div[@class='full-height image-cover']");
            if(imgNode == null)
            {
                logger.LogWarning($"Image for {exhibitionLink} not found.");
                continue;
            }
            
            var paragraphNodes = exhibitionHtmlDocument.DocumentNode.SelectNodes("//div[@class='module-text']//p");
            if(paragraphNodes == null)
            {
                logger.LogWarning($"Paragraphs for {exhibitionLink} not found.");
                continue;
            }
            
            var descriptionNode = paragraphNodes.Skip(4).First();

            var title = titleNode.InnerText;
            var date = dateNode.InnerText;
            var imgLink = baseUrl + imgNode.SelectSingleNode("//picture//source").Attributes["srcset"].Value;
            var description = descriptionNode.InnerText;
            
            var exhibition = new Exhibition
            {
                MuseoId = (int)Museos.ZachetaNarodowaGaleriaSztuki,
                Link = $"{baseUrl}{exhibitionLink}",
                Title = title,
                Date = date,
                ImageLink = imgLink,
                Description = description
            };
            exhibitions.Add(exhibition);
        }

        return exhibitions;
    }
}