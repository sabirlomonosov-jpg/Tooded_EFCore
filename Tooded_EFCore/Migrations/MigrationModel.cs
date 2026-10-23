using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore.Migrations;

// Ühine mudeli kirjeldus migreerimise Designer-failide ja snapshot'i jaoks.
// Automaatseid migratsioone saab edaspidi lisada käsuga Add-Migration.
internal static class MigrationModel
{
    public static void Build(ModelBuilder modelBuilder, bool users)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.6")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity("Tooded_EFCore.Kategooria", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            b.Property<string>("Kategooria_nimetus").IsRequired().HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            b.Property<string>("Kirjeldus").HasMaxLength(500)
                .HasColumnType("nvarchar(500)");
            b.HasKey("Id");
            b.ToTable("Kategooriatabel");
            b.HasData(
                new { Id = 1, Kategooria_nimetus = "Joogid" },
                new { Id = 2, Kategooria_nimetus = "Puu- ja köögiviljad" },
                new { Id = 3, Kategooria_nimetus = "Maiustused" });
        });

        modelBuilder.Entity("Tooded_EFCore.Toode", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            b.Property<string>("Toodenimetus").IsRequired().HasMaxLength(100)
                .HasColumnType("nvarchar(100)");
            b.Property<int>("Kogus").HasColumnType("int");
            b.Property<decimal>("Hind").HasPrecision(10, 2).HasColumnType("decimal(10,2)");
            b.Property<string>("Pilt").HasMaxLength(260).HasColumnType("nvarchar(260)");
            b.Property<int>("KategooriaId").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("KategooriaId");
            b.ToTable("Toodetabel", t =>
            {
                t.HasCheckConstraint("CK_Toodetabel_Kogus", "[Kogus] >= 0");
                t.HasCheckConstraint("CK_Toodetabel_Hind", "[Hind] >= 0");
            });
        });

        modelBuilder.Entity("Tooded_EFCore.Toode", b =>
        {
            b.HasOne("Tooded_EFCore.Kategooria", "Kategooria")
                .WithMany("TooDed")
                .HasForeignKey("KategooriaId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            b.Navigation("Kategooria");
        });
        modelBuilder.Entity("Tooded_EFCore.Kategooria", b => b.Navigation("TooDed"));

        if (users)
        {
            modelBuilder.Entity("Tooded_EFCore.Kasutaja", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<string>("Nimi").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
                b.Property<string>("Email").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
                b.Property<string>("ParoolHash").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
                b.Property<string>("Roll").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.ToTable("Kasutajad");
            });
        }
    }
}
