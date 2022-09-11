namespace PortFolio.Data;
public class PortFolioContext : DbContext
{
    public PortFolioContext (DbContextOptions<PortFolioContext> options)
        : base(options)
    {
    }

    public DbSet<Projet> Projets { get; set; } = default!;
    public DbSet<Feature> Features { get; set; } = default!;
    public DbSet<Ressource> Ressources { get; set; } = default!;
    public DbSet<Settings> Settings { get; set; } = default!;
    public DbSet<Categorie> Categories { get; set; } = default!;
}
