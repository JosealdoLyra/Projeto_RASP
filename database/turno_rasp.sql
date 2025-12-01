-- ======================================================================
-- TABELA: turno_rasp
--
-- Finalidade:
--     Armazena os turnos de produção ou operação associados ao processo
--     RASP. Permite identificar em qual turno a ocorrência aconteceu.
--
-- Exemplos comuns de turnos (dependendo da planta):
--     - 1º Turno
--     - 2º Turno
--     - 3º Turno
--     - Administrativo
--
-- Contexto:
--     - Usada diretamente na tabela principal RASP.
--     - Permite análises por turno (tendências, reincidência, etc.).
--
-- Observação:
--     ordem_exibicao organiza a ordem dos turnos no front-end.
-- ======================================================================

create table turno_rasp (

    -- Identificador único do turno
    id_turno_rasp serial primary key,

    -- Nome/descrição do turno (valor único)
    descricao varchar(20) not null unique,

    -- Controle da posição do turno na exibição
    ordem_exibicao smallint not null
);
