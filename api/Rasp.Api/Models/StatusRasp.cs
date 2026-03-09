namespace Rasp.Api.Models
{
    public class StatusRasp
    {
        public int IdStatusRasp { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public short OrdemFluxo { get; set; }
    }
}