using System.ComponentModel.DataAnnotations.Schema;

namespace Rasp.Api.Models
{
    /// <summary>
    /// Entidade principal do processo RASP.
    ///
    /// Esta classe representa o registro central da reclamação e concentra:
    /// - identificação do RASP;
    /// - dados básicos do problema;
    /// - vínculos com fornecedor e tabelas auxiliares;
    /// - indicadores de impacto;
    /// - dados de BP / breakpoint;
    /// - responsáveis pelas etapas;
    /// - status, rascunho e completude.
    ///
    /// Observações importantes para o projeto:
    /// - IdAnalistaMt representa a autoria do RASP na fase MT;
    /// - IdStatusRasp controla a etapa atual do fluxo;
    /// - IsRascunho indica se o RASP ainda está em construção;
    /// - SppsNumero só passa a fazer sentido após evolução do fluxo;
    /// - ResumoOcorrencia representa o resumo curto exibido na abertura;
    /// - DescricaoProblema representa a descrição detalhada do problema.
    /// </summary>
    public class RaspEntity
    {
        // ==========================================================
        // IDENTIFICAÇÃO DO RASP
        // ==========================================================

        /// <summary>
        /// Identificador interno do RASP.
        /// </summary>
        public int IdRasp { get; set; }

        /// <summary>
        /// Número do RASP no formato 000X/YY.
        /// </summary>
        public string NumeroRasp { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação do registro.
        /// </summary>
        public DateOnly DataCriacao { get; set; }

        /// <summary>
        /// Hora de criação do registro.
        /// </summary>
        public TimeOnly HoraCriacao { get; set; }

        // ==========================================================
        // DADOS BÁSICOS DO RASP
        // ==========================================================

        /// <summary>
        /// Fornecedor vinculado ao RASP.
        /// </summary>
        public int IdFornecedorRasp { get; set; }

        /// <summary>
        /// Resumo curto da ocorrência.
        /// Campo próprio da abertura, separado da descrição detalhada.
        /// </summary>
        [Column("resumo_ocorrencia")]
        public string? ResumoOcorrencia { get; set; }

        /// <summary>
        /// Descrição principal e detalhada do problema levantado no RASP.
        /// </summary>
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
        // STATUS E FECHAMENTO
        // ==========================================================

        /// <summary>
        /// Status atual do RASP no fluxo.
        /// </summary>
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
