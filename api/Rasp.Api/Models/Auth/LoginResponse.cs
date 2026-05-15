namespace Rasp.Api.Models.Auth
{
    // =========================================================
    // 01. RESPONSE DE LOGIN
    // =========================================================
    public class LoginResponse
    {
        // =====================================================
        // 01.01 RESULTADO
        // =====================================================
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; } = string.Empty;


        // =====================================================
        // 01.02 DADOS DO USUÁRIO
        // =====================================================
        public string TipoUsuario { get; set; } = string.Empty;

        public int IdUsuario { get; set; }

        public string NomeCompleto { get; set; } = string.Empty;

        public string Gmid { get; set; } = string.Empty;

        public string Perfil { get; set; } = string.Empty;

        public bool Administrador { get; set; }

        public bool PrimeiroAcesso { get; set; }


        // =====================================================
        // 01.03 DIRECIONAMENTO
        // =====================================================
        public string TelaInicial { get; set; } = string.Empty;
    }
}
