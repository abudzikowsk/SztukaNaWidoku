using HtmlAgilityPack;

namespace SztukaNaWidoku.Services;

public class ScrappingUJazdowskiService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://artmuseum.pl";

    public async Task Scrap()
    {
        var html = await httpClient.GetStringAsync(baseUrl + "/program/wystawy");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='item-box event-box masonry-item has-image']//a");

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
                
                var descriptionNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='event-tab-content']//div[@class='body max-w']//p");
                if (descriptionNode == null)
                {
                    logger.LogWarning($"Description in {exhibitionLink} not found.");
                    continue;
                }

                var title = titleNode.InnerText;
                var date = dateText;
                var imgUrl = baseUrl + imgNode.Attributes["src"].Value;
                var description = descriptionNode.InnerText;
            }
        }
    }
}