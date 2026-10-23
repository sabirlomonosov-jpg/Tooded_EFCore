using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Tooded_EFCore.Migrations;

[DbContext(typeof(ToodedContext))]
public partial class ToodedContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder) =>
        MigrationModel.Build(modelBuilder, users: true);
}
