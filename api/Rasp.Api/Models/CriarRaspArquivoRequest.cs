namespace Rasp.Api.Models
{
    public class CriarRaspArquivoRequest
    {
        public int IdRasp { get; set; }
        public string TipoArquivo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string CaminhoArquivo { get; set; } = string.Empty;
        public int IdUsuarioUpload { get; set; }
    }
}