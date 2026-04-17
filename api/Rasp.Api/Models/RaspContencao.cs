using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rasp.Api.Models
{
    [Table("rasp_contencao")]
    public class RaspContencao
    {
        [Key]
        [Column("id_rasp_contencao")]
        public int IdRaspContencao { get; set; }

        [Column("id_rasp_pn")]
        public int IdRaspPn { get; set; }

        [Column("data_selecao")]
        public DateOnly DataSelecao { get; set; }

        [Column("quantidade_checada")]
        public int QuantidadeChecada { get; set; }

        [Column("quantidade_rejeitada")]
        public int? QuantidadeRejeitada { get; set; }

        [Column("quantidade_retrabalhada")]
        public int? QuantidadeRetrabalhada { get; set; }

        [Column("nr_scrap")]
        public string? NrScrap { get; set; }

        [Column("data_lote_verificado")]
        public DateOnly? DataLoteVerificada { get; set; }

        [Column("horas_retrabalho")]
        public decimal? HorasRetrabalho { get; set; }

        [Column("id_empresa_selecao_rasp")]
        public int? IdEmpresaSelecaoRasp { get; set; }

        [Column("id_usuario_execucao")]
        public int? IdUsuarioExecucao { get; set; }

        [Column("tipo_acao")]
        public string? TipoAcao { get; set; }

        [Column("observacao")]
        public string? Observacao { get; set; }

        [Column("data_atualizacao")]
        public DateTime DataAtualizacao { get; set; }

        [Column("quantidade_verificada")]
        public int? QuantidadeVerificada { get; set; }

        [Column("quantidade_ok")]
        public int? QuantidadeOk { get; set; }

        [Column("id_operador_selecao_terceiro")]
        public int? IdOperadorSelecaoTerceiro { get; set; }

        [Column("origem_registro")]
        public short? OrigemRegistro { get; set; }

        [Column("id_turno_rasp")]
        public int? IdTurnoRasp { get; set; }

        [Column("datahora_inicio_atividade")]
        public DateTime? DataHoraInicioAtividade { get; set; }

        [Column("datahora_fim_atividade")]
        public DateTime? DataHoraFimAtividade { get; set; }
    }
}




