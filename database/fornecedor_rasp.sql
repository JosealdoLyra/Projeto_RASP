-- ======================================================================
-- TABELA: fornecedor_rasp
--
-- Finalidade:
--     Armazena informações dos fornecedores relacionados aos registros
--     de qualidade (RASP). Inclui dados como DUNS, nome e tipo de origem.
--
-- Contexto:
--     - Usada diretamente na tabela principal RASP.
--     - Relação 1:N: um fornecedor pode ter vários RASPs associados.
--
-- Campos principais:
--     duns  → identificador global do fornecedor
--     tipo_fornecedor → LOCAL ou IMPORTADO
--     ativo → controle de disponibilidade para seleção no sistema
--
-- ======================================================================

create table fornecedor_rasp (

    -- Identificador único do fornecedor
    id_fornecedor       serial primary key,

    -- Código DUNS (identificação mundial de fornecedores) — deve ser único
    duns                varchar(9) not null unique,

    -- Nome legal ou comercial do fornecedor
    nome                varchar(150) not null,

    -- Tipo do fornecedor: LOCAL ou IMPORTADO
    tipo_fornecedor     varchar(10) not null
        check (tipo_fornecedor in ('LOCAL', 'IMPORTADO')),

    -- Controle de disponibilidade no sistema
    ativo               boolean not null default true
);