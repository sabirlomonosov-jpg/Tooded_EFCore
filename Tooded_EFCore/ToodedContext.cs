using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public class ToodedContext : DbContext
{
    public ToodedContext() { }
    public ToodedContext(DbContextOptions<ToodedContext> options) : base(options) { }
    public DbSet<Toode> Toodetabel => Set<Toode>();
    public DbSet<Kategooria> Kategooriatabel => Set<Kategooria>();
    public DbSet<Kasutaja> Kasutajad => Set<Kasutaja>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        optionsBuilder.UseSqlServer(
            Environment.GetEnvironmentVariable("TOODED_CONNECTION_STRING") ??
            @"Server=(localdb)\MSSQLLocalDB;Database=Tooded_DB;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kategooria>(entity =>
        {
            entity.ToTable("Kategooriatabel");
            entity.Property(x => x.Kategooria_nimetus).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Kirjeldus).HasMaxLength(500);
            entity.HasData(
                new Kategooria { Id = 1, Kategooria_nimetus = "Joogid" },
                new Kategooria { Id = 2, Kategooria_nimetus = "Puu- ja köögiviljad" },
                new Kategooria { Id = 3, Kategooria_nimetus = "Maiustused" });
        });

        modelBuilder.Entity<Toode>(entity =>
        {
            entity.ToTable("Toodetabel");
            entity.Property(x => x.Toodenimetus).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Pilt).HasMaxLength(260);
            entity.Property(x => x.Hind).HasPrecision(10, 2);
            entity.HasOne(x => x.Kategooria)
                .WithMany(x => x.TooDed)
                .HasForeignKey(x => x.KategooriaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Toodetabel_Kogus", "[Kogus] >= 0");
                t.HasCheckConstraint("CK_Toodetabel_Hind", "[Hind] >= 0");
            });
        });

        modelBuilder.Entity<Kasutaja>(entity =>
        {
            entity.ToTable("Kasutajad");
            entity.Property(x => x.Nimi).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.Property(x => x.ParoolHash).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Roll).IsRequired().HasMaxLength(20);
        });
    }
}
