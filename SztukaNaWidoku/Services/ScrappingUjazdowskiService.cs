using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;
using SztukaNaWidoku.Database.Enums;

namespace SztukaNaWidoku.Services;

public class ScrappingUjazdowskiService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://u-jazdowski.pl";

    //Centrum Sztuki Współczesnej Zamek Ujazdowski
    public async Task<List<Exhibition>> Scrap()
    {
        var html = await httpClient.GetStringAsync(baseUrl + "/program/wystawy");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='item-box event-box masonry-item has-image']//a");

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
    
            var dateNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='event-content-header']//div[@class='body mb--3']");
            if (dateNode == null)
            {
                logger.LogWarning($"Date in {exhibitionLink} not found.");
                continue;
            }
            
            var dateText= dateNode.InnerText;
            var dateTextSplitted = dateText.Split("-");
            var dateEndText = dateTextSplitted.Length > 1 ? dateTextSplitted[1] : dateTextSplitted[0];
            if (DateTime.TryParse(dateEndText, out var dateParsed))
            {
                if(dateParsed < DateTime.Now)
                {
                    continue;
                }
        
                var titleNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//h2[@class='attr-value-title title-attr fs-54 fw-black mt-0 mb-10']");
                if (titleNode == null)
                {
                    logger.LogWarning($"Title in {exhibitionLink} not found.");
                    continue;
                }
                
                var imgNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='item-image-lg']//picture//img");
                if(imgNode == null)
                {
                    logger.LogWarning($"Image in {exhibitionLink} not found.");
                    continue;
                }

                var title = titleNode.InnerText;
                var date = dateText;
                var imgLink = baseUrl + imgNode.Attributes["src"].Value;
                
                exhibitions.Add(new Exhibition
                {
                    Title = title,
                    ImageLink = imgLink,
                    Date = date,
                    Link = $"{baseUrl}{exhibitionLink}",
                    MuseoId = (int)Museos.CentrumSztukiWspółczesnejZamekUjazdowski
                });
            }
        }

        return exhibitions;
    }
}