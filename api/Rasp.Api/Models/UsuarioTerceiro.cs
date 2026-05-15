using System.Text.Json.Serialization;

namespace Rasp.Api.Models
{
    // =========================================================
    // 01. ENTIDADE USUÁRIO TERCEIRO
    // =========================================================
    // Representa usuários terceiros que acessam somente a área
    // operacional de seleção/contenção.
    //
    // Esses usuários NÃO acessam:
    // - Admin
    // - FT
    // - LG
    // - Dashboards administrativos
    //
    // O vínculo com a empresa é feito por:
    // - IdEmpresaSelecao
    // =========================================================
    public class UsuarioTerceiro
    {
        // =====================================================
        // 01.01 IDENTIFICAÇÃO
        // =====================================================
        public int IdUsuarioTerceiro { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Sobrenome { get; set; } = string.Empty;

        public string Gmid { get; set; } = string.Empty;


        // =====================================================
        // 01.02 EMPRESA
        // =====================================================
        public int IdEmpresaSelecao { get; set; }


        // =====================================================
        // 01.03 CONTROLE DE ACESSO
        // =====================================================
        public bool Ativo { get; set; }

        public bool PrimeiroAcesso { get; set; }


        // =====================================================
        // 01.04 SENHA
        // =====================================================
        [JsonIgnore]
        public string SenhaHash { get; set; } = string.Empty;


        // =====================================================
        // 01.05 AUDITORIA
        // =====================================================
        public DateTime DataCriacao { get; set; }
    }
}
