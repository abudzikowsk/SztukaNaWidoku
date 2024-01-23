using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SztukaNaWidoku.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Museos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Museos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exhibitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MuseoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    ImageLink = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exhibitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exhibitions_Museos_MuseoId",
                        column: x => x.MuseoId,
                        principalTable: "Museos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Museos",
                columns: new[] { "Id", "City", "Name" },
                values: new object[,]
                {
                    { 1, "Warszawa", "Muzeum Narodowe w Warszawie" },
                    { 2, "Warszawa", "Muzeum Sztuki Nowoczesnej w Warszawie" },
                    { 3, "Sopot", "Państwowa Galeria Sztuki w Sopocie" },
                    { 4, "Warszawa", "Centrum Sztuki Współczesnej Zamek Ujazdowski" },
                    { 5, "Warszawa", "Zachęta Narodowa Galeria Sztuki" },
                    { 6, "Gdańsk", "Muzeum Narodowe w Gdańsku" },
                    { 7, "Gdynia", "Muzeum Miasta Gdyni" },
                    { 8, "Gdańsk", "Centrum Sztuki Współczesnej Łaźnia" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exhibitions_MuseoId",
                table: "Exhibitions",
                column: "MuseoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exhibitions");

            migrationBuilder.DropTable(
                name: "Museos");
        }
    }
}
