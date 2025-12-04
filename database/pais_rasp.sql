-- ======================================================================
-- TABELA: pais_rasp
--
-- Finalidade:
--     Armazena os países utilizados para classificação de fornecedores
--     (local, importado por país, relatórios em Power BI, etc.).
--
-- Contexto:
--     - Referenciada pela tabela fornecedor_rasp (campo id_pais).
--     - Permite análises por país de origem do fornecedor.
--
-- Campos principais:
--     nome_pais → nome descritivo (Brasil, Estados Unidos, México...)
--     sigla     → código curto do país (BRA, USA, MEX...)
-- ======================================================================

create table pais_rasp (

    -- Identificador único do país
    id_pais      serial primary key,

    -- Nome completo do país
    nome_pais    varchar(80) not null,

    -- Sigla curta do país (ex: BRA, USA, MEX)
    sigla        varchar(3) not null,

    -- Controle de disponibilidade no sistema
    ativo        boolean not null default true
);
