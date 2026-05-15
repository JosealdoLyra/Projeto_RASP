namespace Rasp.Api.Models.Auth
{
    // =========================================================
    // 01. REQUEST DE TROCA DE SENHA
    // =========================================================
    public class TrocarSenhaRequest
    {
        // =====================================================
        // 01.01 IDENTIFICAÇÃO
        // =====================================================
        public string Login { get; set; } = string.Empty;


        // =====================================================
        // 01.02 SENHAS
        // =====================================================
        public string SenhaAtual { get; set; } = string.Empty;

        public string NovaSenha { get; set; } = string.Empty;

        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}
