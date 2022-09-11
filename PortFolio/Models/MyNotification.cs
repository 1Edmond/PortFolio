namespace PortFolio.Models;

[NotMapped]
public class MyNotification
{
    public string Libelle { get; set; }

    public string Description { get; set; }

    public string Type { get; set; } = "success";
}
