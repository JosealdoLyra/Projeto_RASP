-- ======================================================================
-- TABELA: classificacao_rasp
-- Finalidade:
--     Armazena os tipos de classificação do RASP utilizados pela equipe
--     de qualidade para organizar, filtrar e priorizar os registros.
--
-- Exemplos de classificação (dependendo do modelo definido):
--     - Procedente
--     - Não Procedente
--     - Em Análise
--     - Reincidência
--     - Defeito Crítico
--     - Etc.
--
-- Observação:
--     A tabela é auxiliar e usada apenas como domínio fixo (lista de opções).
-- ======================================================================

create table classificacao_rasp (

    -- Identificador único da classificação
    id_classificacao_rasp serial primary key,

    -- Nome/descrição da classificação (valor único)
    descricao varchar(30) not null unique,

    -- Define a ordem em que as classificações aparecerão no front-end
    ordem_exibicao smallint not null
);
