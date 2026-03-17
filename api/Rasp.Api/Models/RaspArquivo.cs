namespace Rasp.Api.Models
{
    // Entidade de arquivos anexados ao RASP.
    //
    // Armazena somente os metadados do arquivo:
    // - RASP relacionado
    // - tipo do arquivo
    // - descrição opcional
    // - caminho ou URL
    // - data de upload
    // - usuário que anexou
    public class RaspArquivo
    {
        // Identificador interno do arquivo
        public int IdArquivoRasp { get; set; }

        // RASP ao qual o arquivo pertence
        public int IdRasp { get; set; }

        // Tipo do arquivo (foto, vídeo, pdf, planilha, etc.)
        public string TipoArquivo { get; set; } = string.Empty;

        // Descrição opcional do arquivo
        public string? Descricao { get; set; }

        // Caminho ou URL do arquivo armazenado
        public string CaminhoArquivo { get; set; } = string.Empty;

        // Data do upload
        public DateOnly DataUpload { get; set; }

        // Usuário que realizou o upload
        public int IdUsuarioUpload { get; set; }
    }
}