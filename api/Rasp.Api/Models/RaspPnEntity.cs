namespace Rasp.Api.Models
{
    public class RaspPnEntity
    {
        public int IdRaspPn { get; set; }
        public int IdRasp { get; set; }
        public string Pn { get; set; } = string.Empty;
        public int QuantidadeSuspeita { get; set; }
        public int QuantidadeChecada { get; set; }
        public int QuantidadeRejeitada { get; set; }
        public bool EmContencao { get; set; }
        public string Duns { get; set; } = string.Empty;
        public short OrdemExibicao { get; set; }
    }
}