namespace PortFolio.Models;
public class Projet
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DateAjout { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DateFin { get; set; }

    [DefaultValue(0)]
    public int Etat { get; set; } = 0;

    public Categorie Categorie { get; set; }

    [ForeignKey(nameof(Categorie))]
    public int CategorieId { get; set; }

    public string Lien { get; set; } = string.Empty;
    public List<Feature> Features  { get; set; }
    public List<Ressource> Ressources { get; set; }


}
