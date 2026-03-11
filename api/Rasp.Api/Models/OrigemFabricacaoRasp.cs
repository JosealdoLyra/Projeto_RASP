namespace Rasp.Api.Models
{
    // Domínio de origem de fabricação usados no RASP.
    // Tabela auxiliar de consulta para preenchimento de combos no front.
    public class OrigemFabricacaoRasp
    {
        // Identificador interno da origem de fabricação
        public int IdOrigemFabricacaoRasp { get; set; }

        // Descrição da origem de fabricação
        public string Descricao { get; set; } = string.Empty;

        // Ordem de exibição no front
        public short OrdemExibicao { get; set; }
    }
}