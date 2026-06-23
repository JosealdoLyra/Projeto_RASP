using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rasp.Api.Models
{
    // =========================================================
    // SCRAP / REJEITADO
    // =========================================================
    [Table("scrap_table")]
    public class ScrapTable
    {
        [Key]
        [Column("id_scrap")]
        public int IdScrap { get; set; }

        [Column("numero_scrap")]
        public string NumeroScrap { get; set; } = string.Empty;

        [Column("id_rasp")]
        public int IdRasp { get; set; }

        [Column("id_rasp_pn")]
        public int IdRaspPn { get; set; }

        [Column("numero_rasp")]
        public string NumeroRasp { get; set; } = string.Empty;

        [Column("numero_spps")]
        public string? NumeroSpps { get; set; }

        [Column("id_fornecedor")]
        public int IdFornecedor { get; set; }

        [Column("fornecedor_nome")]
        public string FornecedorNome { get; set; } = string.Empty;

        [Column("fornecedor_duns")]
        public string FornecedorDuns { get; set; } = string.Empty;

        [Column("origem_peca")]
        public string OrigemPeca { get; set; } = string.Empty;

        [Column("pn")]
        public string Pn { get; set; } = string.Empty;

        [Column("nome_peca")]
        public string? NomePeca { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("tipo_destinacao")]
        public string TipoDestinacao { get; set; } = string.Empty;

        [Column("status_scrap")]
        public string StatusScrap { get; set; } = string.Empty;

        [Column("bruneta_numero")]
        public string? BrunetaNumero { get; set; }

        [Column("nota_fiscal_numero")]
        public string? NotaFiscalNumero { get; set; }

        [Column("baixa_estoque_realizada")]
        public bool BaixaEstoqueRealizada { get; set; }

        [Column("baixa_estoque_automatica")]
        public bool BaixaEstoqueAutomatica { get; set; }

        [Column("data_baixa_estoque", TypeName = "timestamp without time zone")]
        public DateTime? DataBaixaEstoque { get; set; }

        [Column("id_usuario_baixa")]
        public int? IdUsuarioBaixa { get; set; }

        [Column("observacao_baixa")]
        public string? ObservacaoBaixa { get; set; }

        [Column("data_criacao", TypeName = "timestamp without time zone")]
        public DateTime DataCriacao { get; set; }

        [Column("id_usuario_criacao")]
        public int? IdUsuarioCriacao { get; set; }

        [Column("observacao")]
        public string? Observacao { get; set; }

        [Column("id_usuario_mt")]
        public int? IdUsuarioMt { get; set; }

        [Column("data_envio_ft", TypeName = "timestamp without time zone")]
        public DateTime? DataEnvioFt { get; set; }

        [Column("id_usuario_ft")]
        public int? IdUsuarioFt { get; set; }

        [Column("data_aprovacao_ft", TypeName = "timestamp without time zone")]
        public DateTime? DataAprovacaoFt { get; set; }

        [Column("observacao_ft")]
        public string? ObservacaoFt { get; set; }

        [Column("data_envio_lg", TypeName = "timestamp without time zone")]
        public DateTime? DataEnvioLg { get; set; }

        [Column("id_usuario_lg")]
        public int? IdUsuarioLg { get; set; }

        [Column("data_aprovacao_lg", TypeName = "timestamp without time zone")]
        public DateTime? DataAprovacaoLg { get; set; }

        [Column("observacao_lg")]
        public string? ObservacaoLg { get; set; }

        [Column("data_bruneta", TypeName = "timestamp without time zone")]
        public DateTime? DataBruneta { get; set; }

        [Column("id_usuario_bruneta")]
        public int? IdUsuarioBruneta { get; set; }

        [Column("data_nota_fiscal", TypeName = "timestamp without time zone")]
        public DateTime? DataNotaFiscal { get; set; }

        [Column("id_usuario_nota_fiscal")]
        public int? IdUsuarioNotaFiscal { get; set; }

        [Column("bloqueado_edicao")]
        public bool BloqueadoEdicao { get; set; }

        [Column("estornado")]
        public bool Estornado { get; set; }

        [Column("data_estorno", TypeName = "timestamp without time zone")]
        public DateTime? DataEstorno { get; set; }

        [Column("id_usuario_estorno")]
        public int? IdUsuarioEstorno { get; set; }

        [Column("motivo_estorno")]
        public string? MotivoEstorno { get; set; }

    }
}
