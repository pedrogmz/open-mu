using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddJewelBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JewelBankEntry",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JewelId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelBankEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelBankEntry_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JewelBankEntry_ItemDefinition_JewelId",
                        column: x => x.JewelId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JewelBankEntry_AccountId",
                schema: "data",
                table: "JewelBankEntry",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelBankEntry_JewelId",
                schema: "data",
                table: "JewelBankEntry",
                column: "JewelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JewelBankEntry",
                schema: "data");
        }
    }
}
