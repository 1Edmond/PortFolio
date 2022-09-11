namespace PortFolio.Models;


[Table("Settings")]
public class Settings
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
    
    public string Value { get; set; }

    public int Etat { get; set; }
}
