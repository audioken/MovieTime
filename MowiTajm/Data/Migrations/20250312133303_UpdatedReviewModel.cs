using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MowiTajm.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReviewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieTitle",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieTitle",
                table: "Reviews");
        }
    }
}
