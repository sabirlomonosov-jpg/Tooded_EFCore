using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tooded_EFCore.Migrations;

[DbContext(typeof(ToodedContext))]
[Migration("20261023000100_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Kategooriatabel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Kategooria_nimetus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Kirjeldus = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Kategooriatabel", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Toodetabel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Toodenimetus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Kogus = table.Column<int>(type: "int", nullable: false),
                Hind = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                Pilt = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                KategooriaId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Toodetabel", x => x.Id);
                table.ForeignKey("FK_Toodetabel_Kategooriatabel_KategooriaId",
                    x => x.KategooriaId, "Kategooriatabel", "Id", onDelete: ReferentialAction.Restrict);
                table.CheckConstraint("CK_Toodetabel_Kogus", "[Kogus] >= 0");
                table.CheckConstraint("CK_Toodetabel_Hind", "[Hind] >= 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Toodetabel_KategooriaId", table: "Toodetabel", column: "KategooriaId");

        migrationBuilder.InsertData("Kategooriatabel", new[] { "Id", "Kategooria_nimetus" },
            new object[,]
            {
                { 1, "Joogid" },
                { 2, "Puu- ja köögiviljad" },
                { 3, "Maiustused" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Toodetabel");
        migrationBuilder.DropTable(name: "Kategooriatabel");
    }
}
