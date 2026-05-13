using System.ComponentModel.DataAnnotations.Schema;

namespace Rasp.Api.Models
{
    [Table("rasp_historico_fluxo")]
    public class RaspHistoricoFluxoEntity
    {
        [Column("id_historico_fluxo")]
        public int IdHistoricoFluxo { get; set; }

        [Column("id_rasp")]
        public int IdRasp { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("acao")]
        public string Acao { get; set; } = string.Empty;

        [Column("status_anterior")]
        public int? StatusAnterior { get; set; }

        [Column("status_novo")]
        public int? StatusNovo { get; set; }

        [Column("observacao")]
        public string? Observacao { get; set; }

        [Column("data_hora")]
        public DateTime DataHora { get; set; }

        [Column("data_hora_anterior")]
        public DateTime? DataHoraAnterior { get; set; }

        [Column("tempo_fase_minutos")]
        public int? TempoFaseMinutos { get; set; }

        [Column("origem_evento")]
        public string? OrigemEvento { get; set; }

        [Column("tipo_complemento")]
        public string? TipoComplemento { get; set; }
    }
}
