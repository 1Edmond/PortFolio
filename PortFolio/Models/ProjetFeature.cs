namespace PortFolio.Models;

[Table("ProjetFeature")]
public class ProjetFeature
{
    public int Id { get; set; }

    [ForeignKey(nameof(Feature))]
    public int FeatureId { get; set; }
    public Feature Feature { get; set; }

    [ForeignKey(nameof(Projet))]
    public int ProjetId { get; set; }

    public Projet Projet { get; set; }

   

}
