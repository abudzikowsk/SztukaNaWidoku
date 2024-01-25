using Bursztynorama.Database.Enums;
using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;

namespace SztukaNaWidoku.Services;

public class ScrappingMNWService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://www.mnw.art.pl";

    //Muzeum Narodowe w Warszawie
    public async Task<List<Exhibition>> Scrap()
    {
        var html = await httpClient.GetStringAsync($"{baseUrl}/wystawy");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='news-content']//*[@class='title']");

        var exhibitions = new List<Exhibition>();
        if(nodes == null)
        {
            logger.LogWarning("Nodes not found.");
            return exhibitions;
        }
        
        foreach (var node in nodes)
        {
            var link = node.SelectSingleNode(".//a");
            if (link == null)
            {
                logger.LogWarning("Link to exhibition not found.");
                continue;
            }

            var exhibitionLink = link.Attributes["href"].Value;
            var exhibitionHtml = await httpClient.GetStringAsync($"{baseUrl}{exhibitionLink}");
    
            var exhibitionHtmlDocument = new HtmlDocument();
            exhibitionHtmlDocument.LoadHtml(exhibitionHtml);
    
            var titleNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode("//div[@class='desc-module']//*[@class='title']");
            if (titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
            
            var paragraphNodes = exhibitionHtmlDocument.DocumentNode.SelectNodes("//div[@class='desc-module']//p");
            if (paragraphNodes == null)
            {
                logger.LogWarning($"Paragraphs for {exhibitionLink} not found.");
                continue;
            }
            
            var dateNode = paragraphNodes.First();
            var descriptionNode = paragraphNodes.Skip(5).First();
            var imgNode = paragraphNodes.Skip(2).First().SelectSingleNode(".//img");

            var title = titleNode.InnerText;
            var date = dateNode.InnerText;
            var description = descriptionNode.InnerText;
            var imgLink = baseUrl + imgNode.Attributes["src"].Value;
            
            exhibitions.Add(new Exhibition
            {
                Title = title,
                Description = description,
                ImageLink = imgLink,
                Date = date,
                MuseoId = (int)Museos.MuzeumNarodoweWWarszawie,
                Link = $"{baseUrl}{exhibitionLink}"
            });
        }

        return exhibitions;
    }
}