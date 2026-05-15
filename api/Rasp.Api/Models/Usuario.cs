using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Rasp.Api.Models
{
    // =========================================================
    // 01. ENTIDADE USUÁRIO GM
    // =========================================================
    // Representa usuários internos GM que acessam o sistema RASP.
    //
    // Exemplos de perfis:
    // - ADMIN
    // - ANALISTA
    // - FT
    // - LG
    //
    // A autenticação será feita usando:
    // - GMIN
    // - SenhaHash
    //
    // A troca obrigatória de senha será controlada por:
    // - PrimeiroAcesso
    // =========================================================
    public class Usuario
    {
        // =====================================================
        // 01.01 IDENTIFICAÇÃO
        // =====================================================
        public int IdUsuario { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Sobrenome { get; set; }

        public string Gmin { get; set; } = string.Empty;


        // =====================================================
        // 01.02 DADOS CORPORATIVOS
        // =====================================================
        public string Email { get; set; } = string.Empty;

        public string Cargo { get; set; } = string.Empty;


        // =====================================================
        // 01.03 CONTROLE DE ACESSO
        // =====================================================
        public bool Ativo { get; set; }

        public bool Administrador { get; set; }

        public bool PrimeiroAcesso { get; set; }

        public int? IdPerfil { get; set; }

        public int? IdTurnoRasp { get; set; }


        // =====================================================
        // 01.04 SENHA
        // =====================================================
        [JsonIgnore]
        public string? SenhaHash { get; set; }


        // =====================================================
        // 01.05 AUDITORIA
        // =====================================================
        public DateTime DataCriacao { get; set; }

        public DateTime? UltimoLogin { get; set; }


        // =====================================================
        // 01.06 CAMPO AUXILIAR NÃO MAPEADO
        // =====================================================
        // Usado apenas para entrada temporária de senha, quando necessário.
        // Não é salvo no banco e não é exposto no JSON.
        [NotMapped]
        [JsonIgnore]
        public string Senha { get; set; } = string.Empty;
    }
}
