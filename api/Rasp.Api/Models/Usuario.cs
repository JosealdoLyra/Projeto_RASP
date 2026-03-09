using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Rasp.Api.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Gmin { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public bool Ativo { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string Senha { get; set; } = string.Empty;
        
    }
}