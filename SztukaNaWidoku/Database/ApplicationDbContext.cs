using Microsoft.EntityFrameworkCore;
using SztukaNaWidoku.Database.Entities;

namespace SztukaNaWidoku.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Exhibition> Exhibitions { get; set; }
	public DbSet<Museo> Museos { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Museo>().HasData(
			new Museo
			{
				Id = 1,
				Name = "Muzeum Narodowe w Warszawie",
				City = "Warszawa"
			},
			new Museo
			{
				Id = 2,
				Name = "Muzeum Sztuki Nowoczesnej w Warszawie",
				City = "Warszawa"
			},
			new Museo
			{
				Id = 3,
				Name = "Państwowa Galeria Sztuki w Sopocie",
				City = "Sopot"
			},
			new Museo
			{
				Id = 4,
				Name = "Centrum Sztuki Współczesnej Zamek Ujazdowski",
				City = "Warszawa"
			},
			new Museo
			{
				Id = 5,
				Name = "Zachęta Narodowa Galeria Sztuki",
				City = "Warszawa"
			},
			new Museo
			{
				Id = 6,
				Name = "Muzeum Narodowe w Gdańsku",
				City = "Gdańsk"
			},
			new Museo
			{
				Id = 7,
				Name = "Muzeum Miasta Gdyni",
				City = "Gdynia"
			},
			new Museo
			{
				Id = 8,
				Name = "Centrum Sztuki Współczesnej Łaźnia",
				City = "Warszawa"
			}
		);
	}
}