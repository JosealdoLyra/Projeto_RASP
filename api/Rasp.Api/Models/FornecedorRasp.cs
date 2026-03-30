namespace Rasp.Api.Models
{
    // Entidade de fornecedor usada no processo RASP.
    //
    // Regras de negócio aplicadas na API
    // - DUNS deve ter 9 dígitos numéricos
    // - Nome é obrigatório
    // - TipoFornecedor deve ser LOCAL ou IMPORTADO
    // - IdPais é opcional no momento pode se null
    public class FornecedorRasp
    {
        // Identificador interno do fornecedor
        public int IdFornecedor { get; set; }

        // DUNS do fornecedor
        public string Duns { get; set; } = string.Empty;

        // Nome do fornecedor
        public string Nome { get; set; } = string.Empty;

        // Tipo do fornecedor: LOCAL ou IMPORTADO
        public string TipoFornecedor { get; set; } = string.Empty;

        // Indica se o fornecedor está ativo no cadastro
        public bool Ativo { get; set; }

        // País vinculado ao fornecedor
        public int? IdPais { get; set; }
    }
}