-- ======================================================================
-- TABELA: usuarios
--
-- Finalidade:
--     Armazena os dados dos usuários do sistema RASP.
--     Utilizada para controle de acesso, permissões (via usuario_perfil),
--     identificação de responsáveis, aprovadores e auditores.
--
-- Observações:
--     - O campo "gmin" funciona como identificador corporativo único.
--     - "senha_hash" será utilizada futuramente caso o sistema possua login próprio.
--     - "ativo" permite desativar o usuário sem excluir registros históricos.
--
-- Exemplos de cargos:
--     - Analista da Qualidade
--     - FT 1º Turno
--     - LG
--     - Engenheiro de Qualidade
--     - Gerente
--
-- ======================================================================

create table usuarios (

    -- Identificador único do usuário
    id_usuario  serial primary key,

    -- Nome completo do usuário
    nome        varchar(60) not null,

    -- GMIN corporativo (deve ser único)
    gmin        varchar(9) not null unique,

    -- E-mail corporativo ou principal
    email       varchar(150) not null,

    -- Cargo ou função (descrição livre)
    cargo       varchar(80) not null,    -- Ex.: Analista, FT 2º turno, LG, etc.

    -- Usuário ativo no sistema
    ativo       boolean not null default true,

    -- Hash da senha para autenticação futura (quando aplicável)
    senha_hash  varchar(200)
);
