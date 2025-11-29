-- ======================================================================
-- TABELA: gm_aliado_rasp
--
-- Finalidade:
--     Armazena códigos ou siglas que representam os aliados GM
--     (responsáveis ou áreas internas) vinculados ao processo do RASP.
--
-- Contexto:
--     - Cada registro RASP pode ter um "GM aliado" vinculado.
--     - Usado principalmente para identificação rápida do responsável
--       ou grupo interno da GM em relatórios e telas do sistema.
--
-- Exemplos de valores possíveis (dependendo da definição interna):
--     - 'FT'
--     - 'LG'
--     - 'QE'
--     - 'MI'
--
-- Observação:
--     ordem_exibicao é usada para controlar a ordenação dos itens
--     no front-end e nos relatórios.
--
-- ======================================================================

create table gm_aliado_rasp (

    -- Identificador único do aliado GM
    id_gm_aliado_rasp serial primary key,

    -- Código / sigla do aliado GM (ex.: 'FT', 'LG', 'QE')
    descricao varchar(5) not null unique,

    -- Ordem de exibição no front-end (combobox, listagens, relatórios)
    ordem_exibicao smallint not null
);

