namespace PortFolio.Data;
public class PortFolioContext : DbContext
{
    public PortFolioContext (DbContextOptions<PortFolioContext> options)
        : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseNpgsql(@"Host=ec2-23-20-140-229.compute-1.amazonaws.com;Username=ruzpbayvzivepc;Password=9a9b575d2d22c623fcb3be1bec620f8d570706b9e2d775903290447b9343dfbb;Database=dddarqseaeo494");


    public DbSet<Projet> Projets { get; set; } = default!;
    public DbSet<Feature> Features { get; set; } = default!;
    public DbSet<Ressource> Ressources { get; set; } = default!;
    public DbSet<Settings> Settings { get; set; } = default!;
    public DbSet<Categorie> Categories { get; set; } = default!;
    public DbSet<ProjetFeature> ProjetFeatures { get; set; } = default!;
    public DbSet<Skill> Skills { get; set; } = default!;

}
