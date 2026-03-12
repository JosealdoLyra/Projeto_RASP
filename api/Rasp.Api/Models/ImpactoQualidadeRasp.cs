namespace Rasp.Api.Models
{
    public class ImpactoQualidadeRasp
    {
        public int IdImpactoQualidadeRasp { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public short OrdemExibicao { get; set; }
        public string? CodigoOpcao { get; set; }
    }
}