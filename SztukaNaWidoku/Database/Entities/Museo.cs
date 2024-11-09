using SztukaNaWidoku.Enums;

namespace SztukaNaWidoku.Database.Entities;

public class Museo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }

    public Cities MapToCity()
    {
        switch (City)
        {
            case "Warszawa":
                return Cities.Warszawa;
            case "Sopot":
            case "Gdańsk":
            case "Gdynia":
                return Cities.Trojmiasto;
            
            default:
                return Cities.Warszawa;
        }
    }
}