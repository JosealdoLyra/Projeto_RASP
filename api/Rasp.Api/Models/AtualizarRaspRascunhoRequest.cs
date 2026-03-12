namespace Rasp.Api.Models
{
    public class AtualizarRaspRascunhoRequest
    {
        // null = não altera
        // > 0 = atualiza
        public int? IdFornecedorRasp { get; set; }

        // null = não altera
        // 0 = limpa o campo
        // > 0 = atualiza
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
        public int? IdIndiceOperacionalRasp { get; set; }

        // null = não altera
        // "" = limpa o campo
        // texto = atualiza
        public string? SppsNumero { get; set; }
        public string? Procedencia { get; set; }
        public string? DescricaoProblema { get; set; }
        public string? BpTexto { get; set; }
        public string? BpSerie { get; set; }
        public string? BreakpointTexto { get; set; }
        public string? BreakpointCodigo { get; set; }
        public string? ObservacaoGeral { get; set; }

        // null = não altera
        public DateTime? BpDatahora { get; set; }
        public DateTime? BreakpointDatahora { get; set; }

        // null = não altera
        public bool? IniciativaFornecedor { get; set; }
        public bool? SupplierAlert { get; set; }
        public bool? Reversao { get; set; }
        public bool? Safety { get; set; }
        public bool? EmitiuPrr { get; set; }
        public bool? AprovadoLg { get; set; }
        public bool? IsSupplierAlert { get; set; }
        public bool? IsSafety { get; set; }
        public bool? IsReversao { get; set; }
        public bool? GeraPrr { get; set; }
    }
}