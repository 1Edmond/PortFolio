namespace PortFolio.Models
{
    public class ErrorViewModel
    {

#pragma warning disable CS8632 // L'annotation pour les types r�f�rence Nullable doit �tre utilis�e uniquement dans le code au sein d'un contexte d'annotations '#nullable'.
        public string? RequestId { get; set; }
#pragma warning restore CS8632 // L'annotation pour les types r�f�rence Nullable doit �tre utilis�e uniquement dans le code au sein d'un contexte d'annotations '#nullable'.

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}