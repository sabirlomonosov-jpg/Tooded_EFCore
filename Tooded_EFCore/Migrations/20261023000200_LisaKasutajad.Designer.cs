using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore.Migrations;

public partial class LisaKasutajad
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder) =>
        MigrationModel.Build(modelBuilder, users: true);
}
