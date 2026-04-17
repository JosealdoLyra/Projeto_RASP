namespace Rasp.Api.Models.Requests;

public class CriarRaspContencaoRequest
{
    public int IdRaspPn { get; set; }

    public DateOnly DataLoteVerificada { get; set; }

    public string? Lote { get; set; }

    public int QuantidadeVerificada { get; set; }

    public int QuantidadeRejeitada { get; set; }

    public string? Observacao { get; set; }

    public int IdTurnoRasp { get; set; }

    public int? IdOperadorSelecaoTerceiro { get; set; }

    public int? IdUsuarioInterno { get; set; }

    public short OrigemRegistro { get; set; }

    // NOVOS CAMPOS
    public DateTime? DataHoraInicioAtividade { get; set; }

    public DateTime? DataHoraFimAtividade { get; set; }
}




