namespace Rasp.Api.Models
{
    public class EmpresaSelecaoRasp
    {
        public int IdEmpresaSelecao { get; set; }
        public string NomeEmpresa { get; set; } = string.Empty;
        public string? TipoEmpresa { get; set; }
        public bool Ativo { get; set; }
    }
}