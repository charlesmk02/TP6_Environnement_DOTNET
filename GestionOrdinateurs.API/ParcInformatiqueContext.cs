using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Shared;

public class ParcInformatiqueContext : DbContext
{
    public DbSet<Ordinateur> Ordinateurs { get; set; } = null!;
    public DbSet<Reparation> Reparations { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");  
        }
    }
    // TP4.étape3 : REFACTORING --> Configurer les relations entre les entités Ordinateur et Reparation
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Clé primaire
        modelBuilder.Entity<Ordinateur>().HasKey(o => o.Id);

        // caractéristiques d'un ordi
        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        // Relation 1-N : un ordinateur a plusieurs réparations
        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur);

        // TP 5.2 : Le Login doit être unique en base de données.
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
    }
}