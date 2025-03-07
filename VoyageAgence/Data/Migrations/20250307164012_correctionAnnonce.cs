using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoyageAgence.Data.Migrations
{
    /// <inheritdoc />
    public partial class correctionAnnonce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeureArrivee",
                table: "Annonce");

            migrationBuilder.DropColumn(
                name: "HeureDepart",
                table: "Annonce");

            migrationBuilder.DropColumn(
                name: "TypeOffre",
                table: "Annonce");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "HeureArrivee",
                table: "Annonce",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HeureDepart",
                table: "Annonce",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "TypeOffre",
                table: "Annonce",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
