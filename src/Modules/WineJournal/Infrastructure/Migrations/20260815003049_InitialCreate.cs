using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WineTracker.WineJournal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Producer = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Vintage = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Region = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    IdentityKey = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wine_consumptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ReorderIntent = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wine_consumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wine_consumptions_wines_WineId",
                        column: x => x.WineId,
                        principalTable: "wines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wine_consumptions_ConsumedOn_CreatedAt",
                table: "wine_consumptions",
                columns: new[] { "ConsumedOn", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_wine_consumptions_WineId",
                table: "wine_consumptions",
                column: "WineId");

            migrationBuilder.CreateIndex(
                name: "IX_wines_IdentityKey",
                table: "wines",
                column: "IdentityKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wine_consumptions");

            migrationBuilder.DropTable(
                name: "wines");
        }
    }
}
