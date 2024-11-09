using HtmlAgilityPack;
using SztukaNaWidoku.Database.Entities;
using SztukaNaWidoku.Database.Enums;

namespace SztukaNaWidoku.Services;

public class ScrappingMMGService(HttpClient httpClient, ILogger<ScrappingMMGService> logger)
{
    private const string baseUrl = "https://muzeumgdynia.pl";

    //Muzeum Miasta Gdyni
    public async Task<List<Exhibition>> Scrap()
    {
        var exhibitions = new List<Exhibition>();
        var html = await httpClient.GetStringAsync(baseUrl + "/wystawa/");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='posterthumb clipped-rect']//a");
        var dates = htmlDocument.DocumentNode.SelectNodes("//h2[@class='date']");

        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var exhibitionLink = node.Attributes["href"].Value;
            var exhibitionHtml = await httpClient.GetStringAsync(exhibitionLink);
            var exhibitionHtmlDocument = new HtmlDocument();
            exhibitionHtmlDocument.LoadHtml(exhibitionHtml);
    
            var titleNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//h1[@class='main']");
            if (titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
            
            var imgNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='featuredin clipped-rect']//img");
            if (imgNode == null)
            {
                logger.LogWarning($"Image for {exhibitionLink} not found.");
                continue;
            }
            
            var descriptionNodes = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='col']//p");
            if (descriptionNodes == null)
            {
                logger.LogWarning($"Description for {exhibitionLink} not found.");
                continue;
            }
    
            var title = titleNode.InnerText;
            var date = dates[i].InnerText;
            var imgUrl = imgNode.Attributes["src"].Value;
            var description = descriptionNodes.InnerText;
            
            exhibitions.Add(new Exhibition
            {
                Title = title,
                Description = description,
                ImageLink = imgUrl,
                Date = date,
                MuseoId = (int)Museos.MuzemMiastaGdyni,
                Link = exhibitionLink
            });
        }

        return exhibitions;
    }
}