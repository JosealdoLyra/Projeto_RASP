-- ======================================================================
-- TABELA: major_rasp
--
-- Finalidade:
--     Armazena classificações relacionadas ao status MAJOR,
--     utilizado pela GM para indicar se um caso apresenta
--     gravidade elevada ou impacto crítico.
--
-- Contexto:
--     - É usada na tabela principal RASP para identificar se um caso
--       é considerado MAJOR ou não, seguindo critérios internos.
--
-- Exemplos comuns:
--     - "MAJOR"
--     - "NÃO MAJOR"
--     - "CRÍTICO"
--     - "ALTO IMPACTO"
--
-- Observação:
--     A tabela funciona como domínio, permitindo flexibilidade na
--     definição das categorias pelo time de qualidade.
--
-- ======================================================================

create table major_rasp (

    -- Identificador único da classificação "major"
    id_major_rasp serial primary key,

    -- Descrição da classificação (deve ser única)
    descricao varchar(30) not null unique,

    -- Ordem de exibição no sistema (combos, relatórios, filtros)
    ordem_exibicao smallint not null
);
