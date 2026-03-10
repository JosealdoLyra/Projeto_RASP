namespace Rasp.Api.Models
{
    // Entidade de vínculo entre um RASP e os PNs envolvidos.
    //
    // Esta tabela registra:
    // - qual PN está associado ao RASP
    // - quantidades suspeita, checada e rejeitada
    // - se o item está em contenção
    // - DUNS relacionado
    // - ordem de exibição no processo
    //
    // Regras de negócio aplicadas na API:
    // - o RASP deve existir
    // - o PN deve existir no cadastro mestre
    // - o DUNS deve existir no cadastro de fornecedor
    // - quantidades não podem ser negativas
    // - não permitir repetição do mesmo PN no mesmo RASP
    public class RaspPnEntity
    {
        // Identificador interno do vínculo RASP x PN
        public int IdRaspPn { get; set; }

        // RASP ao qual o PN está vinculado
        public int IdRasp { get; set; }

        // Código do PN vinculado ao RASP
        public string Pn { get; set; } = string.Empty;

        // Quantidade inicialmente suspeita
        public int QuantidadeSuspeita { get; set; }

        // Quantidade efetivamente checada
        public int QuantidadeChecada { get; set; }

        // Quantidade rejeitada após análise
        public int QuantidadeRejeitada { get; set; }

        // Indica se o item está em contenção
        public bool EmContencao { get; set; }

        // DUNS relacionado ao item / fornecedor
        public string Duns { get; set; } = string.Empty;

        // Ordem de exibição do item dentro do RASP
        public short OrdemExibicao { get; set; }
    }
}