namespace Rasp.Api.Models
{
    // Entidade principal do processo RASP.
    //
    // Esta classe representa o registro central da reclamação e concentra:
    // - identificação do RASP
    // - dados básicos do problema
    // - vínculos com fornecedor e tabelas auxiliares
    // - indicadores de impacto
    // - dados de BP / breakpoint
    // - responsáveis pelas etapas
    // - status, rascunho e completude
    //
    // Observações importantes para o projeto:
    // - IdAnalistaMt representa a autoria do RASP na fase MT
    // - IdStatusRasp controla a etapa atual do fluxo
    // - IsRascunho indica se o RASP ainda está em construção
    // - SppsNumero só passa a fazer sentido após evolução do fluxo
    public class RaspEntity
    {
        // Identificador interno do RASP
        public int IdRasp { get; set; }

        // Número do RASP no formato 000X/YY
        public string NumeroRasp { get; set; } = string.Empty;

        // Data e hora de criação do registro
        public DateOnly DataCriacao { get; set; }
        public TimeOnly HoraCriacao { get; set; }

        // Fornecedor vinculado ao RASP
        public int IdFornecedorRasp { get; set; }

        // Vínculos com tabelas auxiliares / domínios do processo
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

        // Número do SPPS e procedência do documento
        public string? SppsNumero { get; set; }
        public string? Procedencia { get; set; }

        // Descrição principal do problema levantado no RASP
        public string DescricaoProblema { get; set; } = string.Empty;

        // Indicadores / flags do processo
        public bool? IniciativaFornecedor { get; set; }
        public bool? SupplierAlert { get; set; }
        public bool? Reversao { get; set; }
        public bool? Safety { get; set; }
        public bool? EmitiuPrr { get; set; }
        public bool? AprovadoLg { get; set; }

        // Informações relacionadas ao BP
        public string? BpTexto { get; set; }
        public string? BpSerie { get; set; }
        public DateTime? BpDatahora { get; set; }

        // Responsáveis pelas etapas do fluxo
        public int? IdAnalista { get; set; }
        public int? IdAprovadorFt { get; set; }
        public int? IdAprovadorLg { get; set; }

        // Status atual do RASP no fluxo
        public int IdStatusRasp { get; set; }

        // Informações complementares e de fechamento
        public short? AnoRasp { get; set; }
        public DateOnly? DataFechamento { get; set; }

        // Autor do RASP na fase MT
        public int? IdAnalistaMt { get; set; }

        // Informações relacionadas ao breakpoint
        public string? BreakpointTexto { get; set; }
        public string? BreakpointCodigo { get; set; }
        public DateTime? BreakpointDatahora { get; set; }

        // Flags alternativas / complementares do processo
        public bool? IsSupplierAlert { get; set; }
        public bool? IsSafety { get; set; }
        public bool? IsReversao { get; set; }
        public bool? GeraPrr { get; set; }

        // Observações gerais do registro
        public string? ObservacaoGeral { get; set; }

        // Controle de rascunho e nível de preenchimento
        public bool IsRascunho { get; set; }
        public short PercentualCompletude { get; set; }

        // Índice operacional vinculado ao RASP
        public int? IdIndiceOperacionalRasp { get; set; }

        public string? RdNumero { get; set; }
        public string? CampanhaNumero { get; set; }
        public string? NomeContato { get; set; }
        public DateOnly? DataContato { get; set; }
    }
}
