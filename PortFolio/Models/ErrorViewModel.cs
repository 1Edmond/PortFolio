namespace PortFolio.Models
{
    public class ErrorViewModel
    {

#pragma warning disable CS8632 // L'annotation pour les types référence Nullable doit être utilisée uniquement dans le code au sein d'un contexte d'annotations '#nullable'.
        public string? RequestId { get; set; }
#pragma warning restore CS8632 // L'annotation pour les types référence Nullable doit être utilisée uniquement dans le code au sein d'un contexte d'annotations '#nullable'.

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}