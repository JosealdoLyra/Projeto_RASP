-- ======================================================================
-- TABELA: origem_fabricacao_rasp
--
-- Finalidade:
--     Armazena as origens possíveis de fabricação da peça envolvida no RASP.
--     Essa informação é usada para identificar rapidamente onde a peça foi
--     produzida ou por qual processo passou antes da ocorrência.
--
-- Exemplos possíveis (dependendo da definição interna da GM):
--     - GM
--     - Fornecedor
--     - Retrabalho
--     - Reparo interno
--     - Reparo fornecedor
--
-- Contexto:
--     - Utilizada diretamente na tabela principal RASP.
--     - Permite padronizar a análise e auditoria dos casos.
--
-- Observação:
--     ordem_exibicao controla a posição da opção em listas no front-end.
--
-- ======================================================================

create table origem_fabricacao_rasp (

    -- Identificador único da origem de fabricação
    id_origem_fabricacao_rasp serial primary key,

    -- Descrição da origem (valor único)
    descricao varchar(30) not null unique,

    -- Ordem para exibição em menus e relatórios
    ordem_exibicao smallint not null
);
