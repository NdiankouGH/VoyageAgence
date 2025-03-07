using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoyageAgence.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministrateurId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumPasseport",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumTel",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeUtilisateur",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Agences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adresse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    RCCM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ninea = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<int>(type: "int", nullable: false),
                    GestionnaireId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agences_AspNetUsers_GestionnaireId",
                        column: x => x.GestionnaireId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chauffeurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstDisponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chauffeurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flottes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstDisponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flottes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Offres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    AgenceId = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GestionnaireId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offres_Agences_AgenceId",
                        column: x => x.AgenceId,
                        principalTable: "Agences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offres_AspNetUsers_GestionnaireId",
                        column: x => x.GestionnaireId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Voyages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateDepart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRetour = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prix = table.Column<float>(type: "real", nullable: false),
                    ChauffeurId = table.Column<int>(type: "int", nullable: false),
                    FlotteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voyages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voyages_Chauffeurs_ChauffeurId",
                        column: x => x.ChauffeurId,
                        principalTable: "Chauffeurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Voyages_Flottes_FlotteId",
                        column: x => x.FlotteId,
                        principalTable: "Flottes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoyageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Voyages_VoyageId",
                        column: x => x.VoyageId,
                        principalTable: "Voyages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdministrateurId",
                table: "AspNetUsers",
                column: "AdministrateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Agences_GestionnaireId",
                table: "Agences",
                column: "GestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Offres_AgenceId",
                table: "Offres",
                column: "AgenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Offres_GestionnaireId",
                table: "Offres",
                column: "GestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ClientId",
                table: "Reservations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_VoyageId",
                table: "Reservations",
                column: "VoyageId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_ChauffeurId",
                table: "Voyages",
                column: "ChauffeurId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_FlotteId",
                table: "Voyages",
                column: "FlotteId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_AdministrateurId",
                table: "AspNetUsers",
                column: "AdministrateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_AdministrateurId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Offres");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Agences");

            migrationBuilder.DropTable(
                name: "Voyages");

            migrationBuilder.DropTable(
                name: "Chauffeurs");

            migrationBuilder.DropTable(
                name: "Flottes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AdministrateurId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdministrateurId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumPasseport",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumTel",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TypeUtilisateur",
                table: "AspNetUsers");
        }
    }
}
