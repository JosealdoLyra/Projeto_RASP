namespace Rasp.Api.Models
{
    public class FornecedorRasp
    {
        public int IdFornecedor { get; set; }
        public string Duns { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string TipoFornecedor { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public int IdPais { get; set; }
    }
}