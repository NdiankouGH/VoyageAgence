using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoyageAgence.Models;

namespace VoyageAgence.Data;

public class ApplicationDbContext : IdentityDbContext<Utilisateur>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Gestionnaire> Gestionnaires { get; set; }
    public DbSet<Administrateur> Administrateurs { get; set; }
    public DbSet<Agence> Agences { get; set; }
    public DbSet<Offre> Offres { get; set; }
    public DbSet<Voyage> Voyages { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Chauffeur> Chauffeurs { get; set; }
    public DbSet<Flotte> Flottes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important pour Identity

        // 🔹 Définition de l'héritage TPH (Table Per Hierarchy)
        modelBuilder.Entity<Utilisateur>()
            .HasDiscriminator<string>("TypeUtilisateur")
            .HasValue<Administrateur>("Administrateur")
            .HasValue<Gestionnaire>("Gestionnaire")
            .HasValue<Client>("Client");

        // 🔹 Relation Agence ↔ Gestionnaire
        modelBuilder.Entity<Agence>()
            .HasOne(a => a.Gestionnaire)
            .WithMany(g => g.Agences)
            .HasForeignKey(a => a.GestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Relation Reservation ↔ Client
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Client)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);


    }

    public DbSet<VoyageAgence.Models.Annonce> Annonce { get; set; } = default!;
}
