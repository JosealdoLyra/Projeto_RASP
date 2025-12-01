-- ======================================================================
-- TABELA: perfil_rasp
--
-- Finalidade:
--     Armazena os perfis/funções dos usuários dentro do processo RASP.
--     Os perfis determinam permissões, responsabilidades e papéis
--     no fluxo do sistema (como criação, análise, aprovação, etc.).
--
-- Exemplos comuns de perfis (dependendo da planta):
--     - ADMIN
--     - ANALISTA
--     - FT
--     - LG
--     - GERENTE
--     - SA  (Supplier Alert / Segurança)
--     - EQF (Equipe de Qualidade da Fábrica)
--
-- Observação:
--     Esta tabela pode ser usada:
--       - Para mapear perfis de usuários
--       - Para controle de permissões no front-end
--       - Para auditorias
--
-- ======================================================================

create table perfil_rasp (

    -- Identificador único do perfil
    id_perfil serial primary key,

    -- Nome do perfil (valor único)
    nome varchar(30) not null unique,  -- Ex.: ADMIN, FT, LG, ANALISTA, etc.

    -- Descrição detalhada do perfil
    descricao varchar(150)
);
