namespace Rasp.Api.Models
{
    public class ContaCrSubcontaRasp
    {
        public int IdSubcontaCr { get; set; }
        public int IdContaCr { get; set; }
        public string CodigoSubconta { get; set; } = string.Empty;
        public string? BurdenCenter { get; set; }
        public string DescricaoSubconta { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }
}