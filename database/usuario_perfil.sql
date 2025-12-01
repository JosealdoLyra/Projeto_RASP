-- ======================================================================
-- TABELA: usuario_perfil
--
-- Finalidade:
--     Tabela de relacionamento N:N entre usuários e perfis do sistema RASP.
--     Um usuário pode ter vários perfis (ex.: ANALISTA + FT).
--     Um perfil pode ser atribuído a vários usuários.
--
-- Regras:
--     - A PK é composta por (id_usuario, id_perfil)
--     - Se o usuário for apagado, todos os vínculos são removidos
--       automaticamente (ON DELETE CASCADE).
--     - Perfis não são removidos automaticamente (protege integridade).
--
-- Exemplos de uso:
--     Usuario 12 -> ADMIN, ANALISTA
--     Usuario 45 -> LG, GERENTE
--
-- ======================================================================

create table usuario_perfil (

    -- Usuário associado ao perfil
    id_usuario int not null
        references usuarios(id_usuario)
        on delete cascade,   -- se o usuário for removido, remove os vínculos

    -- Perfil do usuário
    id_perfil int not null
        references perfil_rasp(id_perfil),

    -- Chave primária composta
    primary key (id_usuario, id_perfil)
);
