-- ======================================================================
-- TABELA: impacto_qualidade_rasp
--
-- Finalidade:
--     Armazena os tipos de impacto relacionados à QUALIDADE interna,
--     utilizados no processo de classificação do RASP.
--
-- Diferença para impacto_cliente_rasp:
--     - impacto_cliente_rasp  → como o cliente final é afetado.
--     - impacto_qualidade_rasp → impacto técnico interno dentro da GM.
--
-- Exemplos de impacto de qualidade (podem variar conforme área):
--     - Reprovado no recebimento
--     - Não conformidade grave
--     - Afeta segurança do veículo
--     - Falha funcional repetitiva
--     - Não conformidade dimensional
--     - Desvio de processo
--
-- Observação:
--     O campo "descricao" é maior (1000) para comportar descrições longas
--     e completas usadas pelas equipes de qualidade.
--
-- ======================================================================

create table impacto_qualidade_rasp (

    -- Identificador único do impacto de qualidade
    id_impacto_qualidade_rasp serial primary key,

    -- Descrição detalhada do impacto (única)
    descricao varchar(1000) not null unique,

    -- Ordem de exibição no front-end
    ordem_exibicao smallint not null
);

