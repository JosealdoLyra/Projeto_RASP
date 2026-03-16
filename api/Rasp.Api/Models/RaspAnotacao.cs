namespace Rasp.Api.Models
{
    // Entidade de anotações do RASP.
    //
    // Esta tabela registra:
    // - o RASP relacionado
    // - o usuário que fez a anotação
    // - o perfil no momento do registro
    // - data e hora da anotação
    // - tipo da anotação
    // - texto da anotação
    //
    // Regras de negócio do projeto:
    // - toda anotação pertence a um RASP existente
    // - toda anotação deve registrar o usuário autor
    // - toda anotação deve registrar o perfil do momento
    // - o texto da anotação é obrigatório
    public class RaspAnotacao
    {
        // Identificador interno da anotação
        public int IdRaspAnotacao { get; set; }

        // RASP ao qual a anotação pertence
        public int IdRasp { get; set; }

        // Usuário que registrou a anotação
        public int IdUsuario { get; set; }

        // Perfil do usuário no momento da anotação
        public int IdPerfilRasp { get; set; }

        // Data e hora em que a anotação foi registrada
        public DateTime DataHora { get; set; }

        // Tipo da anotação (Complemento, Correção, Observação, etc.)
        public string TipoAnotacao { get; set; } = string.Empty;

        // Conteúdo da anotação
        public string TextoAnotacao { get; set; } = string.Empty;
    }
}