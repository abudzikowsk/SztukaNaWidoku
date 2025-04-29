using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SztukaNaWidoku.Migrations
{
    /// <inheritdoc />
    public partial class removedescriptionfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Exhibitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Exhibitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
