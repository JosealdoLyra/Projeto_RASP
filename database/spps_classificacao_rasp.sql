-- ======================================================================
-- TABELA: spps_classificacao_rasp
--
-- Finalidade:
--     Armazena as classificações possíveis de um documento SPPS dentro
--     do processo RASP. Representa a conclusão ou categorização aplicada
--     ao SPPS após análise interna.
--
-- Diferença para *spps_status_rasp*:
--     - spps_classificacao_rasp → classificação final do SPPS
--     - spps_status_rasp        → estágio atual do SPPS no fluxo
--
-- Exemplos comuns de classificação:
--     - Procedente
--     - Não Procedente
--     - Rejeitado
--     - Aprovado pelo Fornecedor
--     - Revisado com Gerente
--
-- Observação:
--     ordem_exibicao controla a posição do item em combos e relatórios,
--     garantindo uma ordenação natural no front-end.
-- ======================================================================

create table spps_classificacao_rasp (

    -- Identificador único da classificação
    id_spps_classificacao_rasp serial primary key,

    -- Nome/descrição da classificação (deve ser única)
    descricao varchar(30) not null unique,

    -- Ordem de exibição no front-end
    ordem_exibicao smallint not null
);
