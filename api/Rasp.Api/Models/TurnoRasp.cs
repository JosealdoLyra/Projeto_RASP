namespace Rasp.Api.Models
{
    public class TurnoRasp
    {
        public int IdTurnoRasp { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public short OrdemExibicao { get; set; }
    }
}