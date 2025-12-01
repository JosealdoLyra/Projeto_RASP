-- ======================================================================
-- TABELA: setor_rasp
--
-- Finalidade:
--     Armazena os setores da planta utilizados no processo RASP.
--     Cada setor representa um local físico ou área de responsabilidade
--     onde a ocorrência de qualidade foi identificada ou tratada.
--
-- Exemplos comuns de setores (dependendo da planta):
--     - Montagem
--     - Funilaria
--     - Pintura
--     - Motores
--     - QA / Qualidade
--     - Recebimento / Docks
--
-- Contexto:
--     - A tabela é usada diretamente pela tabela principal RASP.
--     - Os valores aparecem em filtros, relatórios e combos de seleção.
--
-- Observação:
--     ordem_exibicao define a ordem padrão de visualização no front-end
--     (muito útil para organizar setores em menus).
--
-- ======================================================================

create table setor_rasp (

    -- Identificador único do setor
    id_setor_rasp   serial primary key,

    -- Nome/descrição do setor (valor único)
    descricao       varchar(50) not null unique,

    -- Ordem de exibição no sistema
    ordem_exibicao  smallint not null
);

