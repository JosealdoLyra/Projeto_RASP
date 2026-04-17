namespace Rasp.Api.Models
{
    public class OperadorSelecaoTerceiro
    {
        public int IdOperadorSelecaoTerceiro { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Empresa { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        public string? Observacao { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataInativacao { get; set; }
    }
}
