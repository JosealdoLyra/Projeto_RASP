namespace Rasp.Api.Models
{
    public class RaspEntity
    {
        public int IdRasp { get; set; }
        public string NumeroRasp { get; set; } = string.Empty;
        public DateOnly DataCriacao { get; set; }
        public TimeOnly HoraCriacao { get; set; }
        public int IdFornecedorRasp { get; set; }
        public string DescricaoProblema { get; set; } = string.Empty;
        public int IdStatusRasp { get; set; }
    }
}