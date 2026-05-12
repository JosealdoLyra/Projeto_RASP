using System.ComponentModel.DataAnnotations.Schema;

namespace Rasp.Api.Models
{
    /// <summary>
    /// Entidade principal do processo RASP.
    /// </summary>
    public class RaspEntity
    {
        // ==========================================================
        // IDENTIFICAÇÃO DO RASP
        // ==========================================================
        public int IdRasp { get; set; }

        public string NumeroRasp { get; set; } = string.Empty;

        public DateOnly DataCriacao { get; set; }

        public TimeOnly HoraCriacao { get; set; }

        // ==========================================================
        // DADOS BÁSICOS DO RASP
        // ==========================================================
        public int IdFornecedorRasp { get; set; }

        [Column("resumo_ocorrencia")]
        public string? ResumoOcorrencia { get; set; }

        public string DescricaoProblema { get; set; } = string.Empty;

        // ==========================================================
        // VÍNCULOS COM TABELAS AUXILIARES / DOMÍNIOS DO PROCESSO
        // ==========================================================
        public int? IdModeloVeiculoRasp { get; set; }

        public int? IdSetorRasp { get; set; }

        public int? IdTurnoRasp { get; set; }

        public int? IdOrigemFabricacaoRasp { get; set; }

        public int? IdPilotoRasp { get; set; }

        public int? IdImpactoClienteRasp { get; set; }

        public int? IdImpactoQualidadeRasp { get; set; }

        public int? IdMaiorImpactoRasp { get; set; }

        public int? IdMajorRasp { get; set; }

        public int? IdSppsClassificacaoRasp { get; set; }

        public int? IdSppsStatusRasp { get; set; }

        public int? IdEmpresaSelecaoRasp { get; set; }

        public int? IdContaCrRasp { get; set; }

        public int? IdContaCrSubcontaRasp { get; set; }

        public int? IdGmAliadoRasp { get; set; }

        public int? IdPerfilRasp { get; set; }

        public int? IdIndiceOperacionalRasp { get; set; }

        // ==========================================================
        // NÚMERO DO SPPS E PROCEDÊNCIA
        // ==========================================================
        public string? SppsNumero { get; set; }

        public string? Procedencia { get; set; }

        // ==========================================================
        // INDICADORES / FLAGS DO PROCESSO
        // ==========================================================
        public bool? IniciativaFornecedor { get; set; }

        public bool? SupplierAlert { get; set; }

        public bool? Reversao { get; set; }

        public bool? Safety { get; set; }

        public bool? EmitiuPrr { get; set; }

        public bool? AprovadoLg { get; set; }

        // ==========================================================
        // INFORMAÇÕES RELACIONADAS AO BP
        // ==========================================================
        public string? BpTexto { get; set; }

        public string? BpSerie { get; set; }

        public DateTime? BpDatahora { get; set; }

        // ==========================================================
        // RESPONSÁVEIS PELAS ETAPAS DO FLUXO
        // ==========================================================
        public int? IdAnalista { get; set; }

        public int? IdAprovadorFt { get; set; }

        public int? IdAprovadorLg { get; set; }

        /// <summary>
        /// Autor do RASP na fase MT.
        /// </summary>
        public int? IdAnalistaMt { get; set; }

        // ==========================================================
        // CONTROLE DO FLUXO DE APROVAÇÃO FT / LG
        // ==========================================================

        public DateTime? DataEnvioFt { get; set; }

        public DateTime? DataAprovacaoFt { get; set; }

        public DateTime? DataEnvioLg { get; set; }

        public string? JustificativaFt { get; set; }

        public string? JustificativaLg { get; set; }

        [Column("observacao_ft")]
        public string? ObservacaoFt { get; set; }

        [Column("id_usuario_aprovacao_ft")]
        public int? IdUsuarioAprovacaoFt { get; set; }

        // ----------------------------------------------------------
        // Aprovação / decisão LG
        // ----------------------------------------------------------
        [Column("data_aprovacao_lg")]
        public DateTime? DataAprovacaoLg { get; set; }

        [Column("id_usuario_aprovacao_lg")]
        public int? IdUsuarioAprovacaoLg { get; set; }

        [Column("observacao_lg")]
        public string? ObservacaoLg { get; set; }





        // ==========================================================
        // STATUS E FECHAMENTO
        // ==========================================================
        public int IdStatusRasp { get; set; }

        public short? AnoRasp { get; set; }

        public DateOnly? DataFechamento { get; set; }

        // ==========================================================
        // INFORMAÇÕES RELACIONADAS AO BREAKPOINT
        // ==========================================================
        public string? BreakpointTexto { get; set; }

        public string? BreakpointCodigo { get; set; }

        public DateTime? BreakpointDatahora { get; set; }

        // ==========================================================
        // FLAGS ALTERNATIVAS / COMPLEMENTARES DO PROCESSO
        // ==========================================================
        public bool? IsSupplierAlert { get; set; }

        public bool? IsSafety { get; set; }

        public bool? IsReversao { get; set; }

        public bool? GeraPrr { get; set; }

        // ==========================================================
        // OBSERVAÇÕES E COMPLEMENTOS
        // ==========================================================
        public string? ObservacaoGeral { get; set; }

        public string? RdNumero { get; set; }

        public string? CampanhaNumero { get; set; }

        public string? NomeContato { get; set; }

        public DateOnly? DataContato { get; set; }

        // ==========================================================
        // CONTROLE DE RASCUNHO E COMPLETUDE
        // ==========================================================
        public bool IsRascunho { get; set; }

        public short PercentualCompletude { get; set; }
    }
}
