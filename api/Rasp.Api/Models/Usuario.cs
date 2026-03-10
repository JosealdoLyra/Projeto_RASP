using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Rasp.Api.Models
{
    // Entidade de usuário do sistema RASP.
    //
    // Esta classe representa os usuários que acessam o sistema e participam
    // do fluxo do processo, como:
    // - ADMIN
    // - ANALISTA
    // - FT
    // - LG
    //
    // O vínculo com o perfil é feito por IdPerfil.
    public class Usuario
    {
        // Identificador interno do usuário
        public int IdUsuario { get; set; }

        // Nome do usuário
        public string Nome { get; set; } = string.Empty;

        // Identificação corporativa / matrícula / GMIN
        public string Gmin { get; set; } = string.Empty;

        // E-mail do usuário
        public string Email { get; set; } = string.Empty;

        // Cargo do usuário
        public string Cargo { get; set; } = string.Empty;

        // Indica se o usuário está ativo no sistema
        public bool Ativo { get; set; }

        // Perfil de acesso do usuário
        public int? IdPerfil { get; set; }

        // Campo auxiliar não mapeado no banco e não exposto no JSON.
        // Mantido fora do persistido para evitar gravação indevida e exposição na API.
        [NotMapped]
        [JsonIgnore]
        public string Senha { get; set; } = string.Empty;
    }
}