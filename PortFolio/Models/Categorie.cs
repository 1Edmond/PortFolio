namespace PortFolio.Models;

[Table("Categories")]
public class Categorie
{

    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage ="Vous devez saisir le champ libelle")]
    public string Libelle { get; set; }

    public string Description { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DateAjout { get; set; }

    [DefaultValue(0)]
    public int Etat { get; set; }

}
