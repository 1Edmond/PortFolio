namespace PortFolio.Models;

[Table("Ressources")]
public class Ressource
{
    [Key]
    public int Id { get; set; }

    public string Libelle { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Lien { get; set; } = string.Empty;
    [DefaultValue(0)]
    public int Etat { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DateAjout { get; set; }
    [ForeignKey("Projet")]
    public int ProjetId { get; set; }
    public Projet Projet { get; set; }


}
