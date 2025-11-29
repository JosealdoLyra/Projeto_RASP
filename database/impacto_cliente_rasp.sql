-- ======================================================================
-- TABELA: impacto_cliente_rasp
--
-- Finalidade:
--     Armazena os tipos de impacto ao cliente relacionados a um RASP.
--     Essa classificação representa o grau de severidade ou consequência
--     percebida pelo cliente final (interno ou externo).
--
-- Contexto:
--     - Utilizada diretamente na tabela principal RASP.
--     - Permite organizar os tipos de impacto em uma ordem definida,
--       facilitando a exibição em formulários e relatórios.
--
-- Exemplos de impacto (podem variar conforme definição interna da GM):
--     - Estética
--     - Funcional
--     - Montagem
--     - Segurança / Safety
--     - Ergonômico
--     - Performance
--
-- Observação:
--     ordem_exibicao serve para controlar a posição da opção em listas.
--
-- ======================================================================

create table impacto_cliente_rasp (

    -- Identificador único do impacto ao cliente
    id_impacto_cliente  serial primary key,

    -- Descrição do impacto (única)
    descricao           varchar(50) not null unique,

    -- Ordem em que o valor será exibido no front-end
    ordem_exibicao      smallint not null
);