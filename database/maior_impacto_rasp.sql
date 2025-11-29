-- ======================================================================
-- TABELA: maior_impacto_rasp
--
-- Finalidade:
--     Armazena o MAIOR impacto identificado no caso, considerando a soma
--     ou a priorização entre impacto ao cliente, impacto à qualidade e
--     outros critérios aplicáveis ao processo do RASP.
--
-- Contexto:
--     - Utilizada para categorizar o caso de acordo com a gravidade ou
--       relevância do impacto.
--     - A descrição costuma ser mais longa, pois pode representar frases
--       completas ou critérios específicos definidos pela área de qualidade.
--
-- Exemplos de possíveis valores:
--     - "Impacto ao cliente crítico"
--     - "Falha de segurança"
--     - "Falha funcional"
--     - "Não conformidade grave"
--     - "Afeta integridade do veículo"
--
-- Observação:
--     ordem_exibicao permite controlar como os itens serão apresentados
--     no front-end (combo box, relatórios, filtros).
--
-- ======================================================================

create table maior_impacto_rasp (

    -- Identificador único do maior impacto
    id_maior_impacto_rasp serial primary key,

    -- Descrição detalhada (pode ser longa)
    descricao             varchar(1000) not null unique,

    -- Ordem de exibição no sistema
    ordem_exibicao        smallint not null
);


