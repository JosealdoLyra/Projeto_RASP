namespace Rasp.Api.Models
{
    // Entidade de perfil de acesso do sistema RASP.
    //
    // Exemplos de perfis já usados no projeto:
    // - ADMIN
    // - ANALISTA
    // - FT
    // - LG
    //
    // Esses perfis são importantes para as regras de permissão
    // aplicadas nos endpoints da API.
    public class PerfilRasp
    {
        // Identificador interno do perfil
        public int IdPerfil { get; set; }

        // Nome do perfil
        public string Nome { get; set; } = string.Empty;
    }
}