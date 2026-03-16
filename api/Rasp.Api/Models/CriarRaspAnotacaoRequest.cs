namespace Rasp.Api.Models
{
    public class CriarRaspAnotacaoRequest
    {
        public int IdRasp { get; set; }
        public int IdUsuario { get; set; }
        public int IdPerfilRasp { get; set; }
        public string? TipoAnotacao { get; set; }
        public string TextoAnotacao { get; set; } = string.Empty;
    }
}