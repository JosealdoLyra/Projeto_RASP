namespace Rasp.Api.Models
{
    // Entidade do cadastro mestre de PN.
    //
    // Regras de negócio aplicadas na API:
    // - CodigoPn deve ter exatamente 8 dígitos
    // - CodigoPn deve conter somente números
    // - NomePeca é obrigatório
    // - Não permitir duplicidade de CodigoPn
    public class PnRasp
    {
        // Identificador interno do PN
        public int IdPn { get; set; }

        // Código do PN
        public string CodigoPn { get; set; } = string.Empty;

        // Nome/descrição da peça
        public string NomePeca { get; set; } = string.Empty;
    }
}