namespace Rasp.Api.Models
{
    public class SppsStatusRasp
    {
        public int IdSppsStatusRasp { get; set; }
        public string NomeStatus { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public short OrdemExibicao { get; set; }
        public bool EhFinal { get; set; }
        public bool ContaParaPrazo { get; set; }
    }
}