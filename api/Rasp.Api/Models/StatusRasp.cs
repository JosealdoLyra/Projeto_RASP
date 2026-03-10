namespace Rasp.Api.Models
{
    // Entidade de domínio que representa os status do fluxo do RASP.
    //
    // Exemplos já usados no projeto:
    // - Em análise
    // - Em avaliação FT
    // - Em avaliação LG
    // - Concluído
    // - Cancelado
    //
    // A ordem lógica do processo é controlada por OrdemFluxo.
    public class StatusRasp
    {
        // Identificador interno do status
        public int IdStatusRasp { get; set; }

        // Descrição textual do status
        public string Descricao { get; set; } = string.Empty;

        // Ordem do status dentro do fluxo do processo
        public short OrdemFluxo { get; set; }
    }
}