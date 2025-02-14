using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Wrpg.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    IdentityProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdentityId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Stats_Attributes_Constitution = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Dexterity = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Intelligence = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Level = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Spirit = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Strength = table.Column<int>(type: "integer", nullable: false),
                    Stats_Resources_Energy = table.Column<int>(type: "integer", nullable: false),
                    Stats_Resources_Health = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_IdentityProvider_IdentityId",
                table: "Accounts",
                columns: new[] { "IdentityProvider", "IdentityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Nickname",
                table: "Accounts",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name",
                table: "Characters",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
