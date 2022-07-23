using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceObserver.Persistance.Migrations
{
    public partial class ChangePricestampEntityName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pricestamps",
                table: "Pricestamps");

            migrationBuilder.RenameTable(
                name: "Pricestamps",
                newName: "PriceStamps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceStamps",
                table: "PriceStamps",
                column: "Id");
        }
    }
}
