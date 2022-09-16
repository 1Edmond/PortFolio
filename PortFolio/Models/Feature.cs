
namespace PortFolio.Models;

public class Feature
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    [DefaultValue(0)]
    public int Etat { get; set; }

    public DateTime DateAjout { get; set; }

    public List<ProjetFeature> ProjetFeatures { get; set; }



}
