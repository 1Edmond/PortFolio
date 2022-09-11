
namespace PortFolio.Models;

public class Feature
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    [DefaultValue(0)]
    public int Etat { get; set; }
    public Projet Projet { get; set; }

    [ForeignKey(nameof(Projet))]
    public int ProjetId { get; set; }



}
