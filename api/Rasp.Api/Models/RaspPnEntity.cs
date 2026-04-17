using System.ComponentModel.DataAnnotations.Schema;

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
    //
    // Extensão do módulo de seleção:
    // - controla se o PN entrou em seleção
    // - controla o status da seleção
    // - registra data/hora de entrada e saída da seleção
    // - controla trava logística
    // - controla QHD
    public class RaspPnEntity
    {
        [Column("data_lote_inicial")]
        public DateTime? DataLoteInicial { get; set; }

        [Column("id_rasp_pn")]
        public int IdRaspPn { get; set; }

        [Column("id_rasp")]
        public int IdRasp { get; set; }

        [Column("pn")]
        public string Pn { get; set; } = string.Empty;

        [Column("quantidade_suspeita")]
        public int QuantidadeSuspeita { get; set; }

        [Column("quantidade_checada")]
        public int QuantidadeChecada { get; set; }

        [Column("quantidade_rejeitada")]
        public int QuantidadeRejeitada { get; set; }

        [Column("em_contencao")]
        public bool EmContencao { get; set; }

        [Column("duns")]
        public string Duns { get; set; } = string.Empty;

        [Column("ordem_exibicao")]
        public short OrdemExibicao { get; set; }

        // CONTROLE DE SELEÇÃO
        [Column("entrou_selecao")]
        public bool EntrouSelecao { get; set; }

        [Column("status_selecao")]
        public short StatusSelecao { get; set; }

        [Column("datahora_entrada_selecao")]
        public DateTime? DatahoraEntradaSelecao { get; set; }

        [Column("datahora_saida_selecao")]
        public DateTime? DatahoraSaidaSelecao { get; set; }

        // CONTROLE DE TRAVA
        [Column("trava_ativa")]
        public bool TravaAtiva { get; set; }

        [Column("datahora_solicitacao_trava")]
        public DateTime? DatahoraSolicitacaoTrava { get; set; }

        [Column("datahora_remocao_trava")]
        public DateTime? DatahoraRemocaoTrava { get; set; }

        // CONTROLE DE QHD
        [Column("qhd_ativo")]
        public bool QhdAtivo { get; set; }

        [Column("datahora_qhd")]
        public DateTime? DatahoraQhd { get; set; }
    }
}

