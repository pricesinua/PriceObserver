using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceObserver.Persistance.Migrations
{
    public partial class ReplaceProductIdWithProductEan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PriceStamps");

            migrationBuilder.AddColumn<string>(
                name: "ProductEan",
                table: "PriceStamps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductEan",
                table: "PriceStamps");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PriceStamps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
