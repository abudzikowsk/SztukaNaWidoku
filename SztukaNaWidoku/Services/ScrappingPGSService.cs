using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;
using SztukaNaWidoku.Database.Enums;

namespace SztukaNaWidoku.Services;

public class ScrappingPGSService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://pgs.pl";
    
    //Państwowa Galeria Sztuki w Sopocie
    public async Task<List<Exhibition>> Scrap()
    {
        var html = await httpClient.GetStringAsync(baseUrl);

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='elementor-element elementor-element-1196ac5 elementor-column elementor-col-100 elementor-top-column']");
        HtmlNodeCollection? links = null;
        if(nodes == null)
        {
            logger.LogWarning("Nodes not found.");
            return new List<Exhibition>();
        }
        
        foreach (var singleNode in nodes)
        {
            var titleNode = singleNode.SelectSingleNode(".//h3[@class='elementor-heading-title elementor-size-default']");
            if (titleNode == null)
            {
                logger.LogWarning("Title for exhibitions not found.");
                continue;
            }
            
            if (titleNode.InnerText.ToLowerInvariant().Contains("wystawy"))
            {
                links = singleNode.SelectNodes(".//div[@class='jet-smart-listing__post-thumbnail post-thumbnail-simple']//a");
            }
        }
        var exhibitions = new List<Exhibition>();

        if(links == null)
        {
            logger.LogWarning("Links to exhibitions not found.");
            return exhibitions;
        }
        
        foreach (var link in links)
        {
            var exhibitionLink = link.Attributes["href"].Value;
            var exhibitionHtml = await httpClient.GetStringAsync(exhibitionLink);
            var exhibitionHtmlDocument = new HtmlDocument();
            exhibitionHtmlDocument.LoadHtml(exhibitionHtml);
    
            var titleNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//header[@class='entry-header']//h1");
            if(titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
        
            var dateNode = exhibitionHtmlDocument.DocumentNode.SelectNodes(".//div[@class='entry-content']//p");
            if(dateNode == null)
            {
                logger.LogWarning($"Date for {exhibitionLink} not found.");
                continue;
            }
            
            var imgNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='entry-content']//p//img");
            if(imgNode == null)
            {
                logger.LogWarning($"Image for {exhibitionLink} not found.");
                continue;
            }

            var title = titleNode.InnerText;
            var date = dateNode.Skip(2).First().InnerText;
            var imgLink = imgNode.Attributes["src"].Value;
            
            exhibitions.Add(new Exhibition
            {
                Title = title,
                ImageLink = imgLink,
                Date = date,
                MuseoId = (int)Museos.PanstwowaGaleriaSztukiWSopocie,
                Link = exhibitionLink
            });
        }

        return exhibitions;
    }
}