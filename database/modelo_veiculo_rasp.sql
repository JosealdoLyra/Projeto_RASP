-- ======================================================================
-- TABELA: modelo_veiculo_rasp
--
-- Finalidade:
--     Armazena os modelos de veículos associados aos registros RASP.
--     Cada RASP pode estar vinculado a um único modelo de veículo,
--     facilitando análises por plataforma, família, linha de montagem
--     e comportamento de falhas por produto.
--
-- Contexto:
--     - Utilizada diretamente na tabela principal RASP.
--     - Lista de domínio (tabela auxiliar) utilizada em combos,
--       filtros e relatórios.
--
-- Exemplos de valores (dependendo da planta):
--     - ONIX
--     - TRACKER
--     - S10
--     - MONTANA
--     - SPIN
--
-- Observação:
--     ordem_exibicao é usada para definir a ordem visual no front-end.
--
-- ======================================================================

create table modelo_veiculo_rasp (

    -- Identificador único do modelo do veículo
    id_modelo_veiculo_rasp serial primary key,

    -- Nome curto do modelo (valor único)
    descricao varchar(20) not null unique,

    -- Ordem de exibição no sistema (importantíssimo para combos/listas)
    ordem_exibicao smallint not null
);
