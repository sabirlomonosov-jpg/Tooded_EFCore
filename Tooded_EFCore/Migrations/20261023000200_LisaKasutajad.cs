using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tooded_EFCore.Migrations;

[DbContext(typeof(ToodedContext))]
[Migration("20261023000200_LisaKasutajad")]
public partial class LisaKasutajad : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Kasutajad",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nimi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ParoolHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Roll = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Kasutajad", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Kasutajad_Email", table: "Kasutajad", column: "Email", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Kasutajad");
    }
}
