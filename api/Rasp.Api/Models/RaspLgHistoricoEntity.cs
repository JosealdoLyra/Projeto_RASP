namespace Rasp.Api.Models;

public class RaspLgHistoricoEntity
{
    public int IdRaspLgHistorico { get; set; }

    public int IdRasp { get; set; }

    public int IdUsuarioLg { get; set; }

    public string Decisao { get; set; } = string.Empty;

    public string? Justificativa { get; set; }

    public DateTime DataHora { get; set; }
}
