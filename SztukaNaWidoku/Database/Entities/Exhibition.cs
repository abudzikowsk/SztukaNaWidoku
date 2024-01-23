namespace SztukaNaWidoku.Database.Entities;

public class Exhibition
{
    public int Id { get; set; }
    public int MuseoId { get; set; }
    public string Link { get; set; }
    public string Title { get; set; }
    public string Date { get; set; }
    public string ImageLink { get; set; }
    public string Description { get; set; }
    
    public Museo Museo { get; set; }
}