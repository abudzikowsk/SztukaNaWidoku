using Bursztynorama.Database.Enums;
using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;

namespace SztukaNaWidoku.Services;

public class ScrappingMuzeumNarodoweGdansk(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://www.mng.gda.pl";

    //Muzeum Narodowe w Gdańsku
    public async Task<List<Exhibition>> Scrap()
    {
        var html = await httpClient.GetStringAsync(baseUrl + "/wystawy-czasowe/");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='col-md-4 col-sm-4']");

        var exhibitions = new List<Exhibition>();
        if(nodes == null)
        {
            logger.LogWarning("Nodes not found.");
            return exhibitions;
        }
        
        foreach (var node in nodes)
        {
            var exhibitionLink = node.SelectSingleNode(".//a[@class='mec-color-hover']").Attributes["href"];
            if (exhibitionLink == null)
            {
                logger.LogWarning("Exhibition link not found.");
                continue;
            }
    
            var titleNode = node.SelectSingleNode(".//a[@class='mec-color-hover']");
            if(titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
            
            var startDateNode = node.SelectSingleNode(".//span[@class='mec-start-date-label']");
            if(startDateNode == null)
            {
                logger.LogWarning($"Date for {exhibitionLink} not found.");
                continue;
            }
            
            var endDateNode = node.SelectSingleNode(".//span[@class='mec-end-date-label']");
            if(endDateNode == null)
            {
                logger.LogWarning($"Date for {exhibitionLink} not found.");
                continue;
            }
            
            var imgNode = node.SelectSingleNode(".//img[@class='attachment-medium size-medium wp-post-image']");
            if(imgNode == null)
            {
                logger.LogWarning($"Image for {exhibitionLink} not found.");
                continue;
            }
            
            var paragraphNodes = node.SelectSingleNode(".//p[@class='mec-grid-event-location']");
            if(paragraphNodes == null)
            {
                logger.LogWarning($"Paragraphs for {exhibitionLink} not found.");
                continue;
            }
            
            var title = titleNode.InnerText;
            var date = startDateNode.InnerText + endDateNode.InnerText;
            var imgLink = imgNode.Attributes["src"].Value;
            var description = paragraphNodes.InnerText;
            
            var exhibition = new Exhibition
            {
                MuseoId = (int)Museos.MuzeumNarodoweWGdańsku,
                Link = exhibitionLink.Value,
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