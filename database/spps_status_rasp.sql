-- ======================================================================
-- TABELA: spps_status_rasp
--
-- Finalidade:
--     Armazena os STATUS possíveis de um SPPS dentro do processo RASP.
--     Diferente de "classificação", o STATUS representa o estágio ATUAL
--     do SPPS no fluxo de análise, aprovação e decisão.
--
-- Diferença entre status e classificação:
--     - STATUS  → etapa atual do SPPS (fluxo)
--     - CLASSIFICAÇÃO → decisão final sobre o SPPS
--
-- Exemplos de STATUS comuns:
--     - Em Análise
--     - Em Revisão Interna
--     - Escalonado – 1º N
--     - Escalonado – 2º N
--     - Decisão Gerencial – Escalation
--     - Fechado
--
-- Observação:
--     - "ativo" permite desativar um status sem perder histórico.
--     - "ordem_exibicao" controla ordenação em combos e relatórios.
-- ======================================================================

create table spps_status_rasp (

    -- Identificador único do status
    id_spps_status_rasp serial primary key,

    -- Nome do status (único)
    nome_status varchar(40) not null unique,

    -- Controle para ativar/desativar o status no sistema
    ativo boolean not null default true,

    -- Ordem visual no front-end
    ordem_exibicao smallint not null
);

