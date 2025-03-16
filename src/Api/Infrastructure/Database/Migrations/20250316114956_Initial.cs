using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Wrpg.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Stats_Attributes_Level = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Strength = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Dexterity = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Intelligence = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Constitution = table.Column<int>(type: "integer", nullable: false),
                    Stats_Attributes_Spirit = table.Column<int>(type: "integer", nullable: false),
                    Stats_Resources_Health = table.Column<int>(type: "integer", nullable: false),
                    Stats_Resources_Energy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.InternalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Id",
                table: "Characters",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name",
                table: "Characters",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
