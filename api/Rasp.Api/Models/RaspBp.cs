using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rasp.Api.Models
{
    [Table("rasp_bp")]
    public class RaspBp
    {
        [Key]
        [Column("id_rasp_bp")]
        public int IdRaspBp { get; set; }

        [Column("id_rasp")]
        public int IdRasp { get; set; }

        [Column("tipo_referencia_bp")]
        public string TipoReferenciaBp { get; set; } = string.Empty;

        [Column("data_bp")]
        public DateOnly DataBp { get; set; }

        [Column("hora_bp")]
        public TimeSpan HoraBp { get; set; }

        [Column("vin")]
        public string? Vin { get; set; }

        [Column("local_celula")]
        public string? LocalCelula { get; set; }

        [Column("como_identificado")]
        public string ComoIdentificado { get; set; } = string.Empty;

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }

        [Column("atualizado_em")]
        public DateTime AtualizadoEm { get; set; }
    }
}

