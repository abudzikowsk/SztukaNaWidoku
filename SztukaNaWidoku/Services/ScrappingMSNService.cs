using Bursztynorama.Database.Enums;
using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;

namespace SztukaNaWidoku.Services;

public class ScrappingMSNService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://artmuseum.pl";
    
    //Muzeum Sztuki Nowoczesnej w Warszawie
    public async Task<List<Exhibition>> Scrap()
    {
        var html = await httpClient.GetStringAsync(baseUrl + "/pl/wystawy");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='box']//a");

        var exhibitions = new List<Exhibition>();
        if(nodes == null)
        {
            logger.LogWarning("Nodes not found.");
            return exhibitions;
        }
        
        foreach (var node in nodes)
        {
            var isActiveNode = node.SelectSingleNode(".//div//span");
            if(isActiveNode == null)
            {
                logger.LogWarning("Link to exhibition not found.");
                continue;
            }
    
            if(isActiveNode.InnerText.ToLowerInvariant() != "aktualna wystawa")
            {
                continue;
            }
            
            var exhibitionLink = node.Attributes["href"].Value;
            var exhibitionHtml = await httpClient.GetStringAsync(baseUrl + node.Attributes["href"].Value);
            var exhibitionHtmlDocument = new HtmlDocument();
            exhibitionHtmlDocument.LoadHtml(exhibitionHtml);
    
            var titleNode = node.SelectSingleNode(".//div[@class='content']//h3");
            if(titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
            
            var dateNode = node.SelectSingleNode(".//div[@class='content']//time");
            var imgNode = node.SelectSingleNode(".//img");
            if(imgNode == null)
            {
                logger.LogWarning($"Image for {exhibitionLink} not found.");
                continue;
            }
            
            var descriptionNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode("//p[@class='lead']");
            if (descriptionNode == null)
            {
                logger.LogWarning($"Description for {exhibitionLink} not found.");
                continue;
            }

            var title = titleNode.InnerText;
            var date = dateNode.InnerText;
            var imgLink = baseUrl + imgNode.Attributes["src"].Value;
            var description = descriptionNode.InnerText;
            
            exhibitions.Add(new Exhibition
            {
                Title = title,
                Description = description,
                ImageLink = imgLink,
                Date = date,
                MuseoId = (int)Museos.MuzeumSztukiNowoczesnejWWarszawie,
                Link = $"{baseUrl}{exhibitionLink}"
            });
        }
        
        return exhibitions;
    }
}