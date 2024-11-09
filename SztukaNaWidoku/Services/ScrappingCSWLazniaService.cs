using System.Text.RegularExpressions;
using SztukaNaWidoku.Database.Entities;
using HtmlAgilityPack;
using PuppeteerSharp;
using SztukaNaWidoku.Database.Enums;

namespace SztukaNaWidoku.Services;

public class ScrappingCSWLazniaService(HttpClient httpClient, ILogger<ScrappingMNWService> logger)
{
    private const string baseUrl = "https://www.laznia.pl";
    
    //Centrum Sztuki Współczesnej Łaźnia
    public async Task<List<Exhibition>> Scrap()
    {

// Setup Puppeteer to use a new browser instance
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Args = ["--no-sandbox"]
        });
// Create a new page and go to it
        var page = await browser.NewPageAsync();
        await page.GoToAsync($"{baseUrl}/wystawy/");

// Wait for the selector you're interested in to be loaded
        await page.WaitForSelectorAsync("#arts_wrapper");

// Extract the information from the page
        var html = await page.GetContentAsync();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var httpClient = new HttpClient();

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@id='arts_wrapper']//a");

        var exhibitions = new List<Exhibition>();
        
        if(nodes == null)
        {
            logger.LogWarning("Nodes not found.");
            return exhibitions;
        }
        foreach (var node in nodes)
        {
            var exhibitionLink = node.Attributes["href"].Value;
            await page.GoToAsync($"{baseUrl}{exhibitionLink}");
            await page.WaitForSelectorAsync(".photo.slick-slide.slick-current.slick-active");
            var exhibitionHtml = await page.GetContentAsync();
            var exhibitionHtmlDocument = new HtmlDocument();
            exhibitionHtmlDocument.LoadHtml(exhibitionHtml);
    
            var dates = htmlDocument.DocumentNode.SelectSingleNode(".//div[@class='date']");
            if(dates == null)
            {
                logger.LogWarning($"Date for {exhibitionLink} not found.");
                continue;
            }
            
            var titleNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='left clearfix']//h1");
            if(titleNode == null)
            {
                logger.LogWarning($"Title for {exhibitionLink} not found.");
                continue;
            }
            
            var imgNode = exhibitionHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='photo slick-slide slick-current slick-active']");
            if(imgNode == null)
            {
                logger.LogWarning($"Image for {exhibitionLink} not found.");
                continue;
            }
            
            var descriptionNodes = exhibitionHtmlDocument.DocumentNode.SelectNodes(".//div[@class='txt ck']//p");
            if(descriptionNodes == null)
            {
                logger.LogWarning($"Description for {exhibitionLink} not found.");
                continue;
            }
            
            var regex = new Regex(@"(\/up\/wystawy\/\w*\/\w*.(jpg|png))");
    
            var title = titleNode.InnerText;
            var date = dates.InnerText;
            var imgUrl = baseUrl + regex.Match(imgNode.Attributes["style"].Value).Value;
            var description = descriptionNodes.First().InnerText;
            
            exhibitions.Add(new Exhibition
            {
                Title = title,
                Description = description,
                ImageLink = imgUrl,
                Date = date,
                MuseoId = (int)Museos.CentrumSztukiWspółczesnejŁaznia,
                Link = $"{baseUrl}{exhibitionLink}"
            });
        }
        await browser.CloseAsync();
        
        return exhibitions;
    }
}