using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore.Migrations;

public partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder) =>
        MigrationModel.Build(modelBuilder, users: false);
}
