using BCrypt.Net;

namespace Rasp.Api.Services
{
    // =========================================================
    // 01. SERVIÇO DE SENHAS
    // =========================================================
    // Responsável por:
    // - gerar hash seguro
    // - validar senha
    //
    // BCrypt é utilizado porque:
    // - é seguro
    // - padrão moderno
    // - resistente a força bruta
    // =========================================================
    public static class SenhaService
    {
        // =====================================================
        // 01.01 GERAR HASH
        // =====================================================
        public static string GerarHash(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }


      
        // =====================================================
        // 01.02 VALIDAR SENHA
        // =====================================================
        public static bool ValidarSenha(
            string senhaDigitada,
            string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(
                senhaDigitada,
                senhaHash
            );
        }
    }
}
