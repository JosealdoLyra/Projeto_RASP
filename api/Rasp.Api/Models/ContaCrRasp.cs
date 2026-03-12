namespace Rasp.Api.Models
{
    public class ContaCrRasp
    {
        public int IdContaCr { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Observacao { get; set; }
    }
}