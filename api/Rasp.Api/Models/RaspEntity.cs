namespace Rasp.Api.Models
{
    public class RaspEntity
    {
        public int IdRasp { get; set; }
        public string NumeroRasp { get; set; } = string.Empty;
        public DateOnly DataCriacao { get; set; }
        public TimeOnly HoraCriacao { get; set; }
        public int IdFornecedorRasp { get; set; }

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

        public string? SppsNumero { get; set; }
        public string? Procedencia { get; set; }

        public string DescricaoProblema { get; set; } = string.Empty;

        public bool? IniciativaFornecedor { get; set; }
        public bool? SupplierAlert { get; set; }
        public bool? Reversao { get; set; }
        public bool? Safety { get; set; }
        public bool? EmitiuPrr { get; set; }
        public bool? AprovadoLg { get; set; }

        public string? BpTexto { get; set; }
        public string? BpSerie { get; set; }
        public DateTime? BpDatahora { get; set; }

        public int? IdAnalista { get; set; }
        public int? IdAprovadorFt { get; set; }
        public int? IdAprovadorLg { get; set; }

        public int IdStatusRasp { get; set; }

        public short? AnoRasp { get; set; }
        public DateOnly? DataFechamento { get; set; }
        public int? IdAnalistaMt { get; set; }

        public string? BreakpointTexto { get; set; }
        public string? BreakpointCodigo { get; set; }
        public DateTime? BreakpointDatahora { get; set; }

        public bool? IsSupplierAlert { get; set; }
        public bool? IsSafety { get; set; }
        public bool? IsReversao { get; set; }
        public bool? GeraPrr { get; set; }

        public string? ObservacaoGeral { get; set; }

        public bool IsRascunho { get; set; }
        public short PercentualCompletude { get; set; }
        public int? IdIndiceOperacionalRasp { get; set; }
    }
}