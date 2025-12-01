-- ======================================================================
-- TABELA: rate_cr_ano_rasp
--
-- Finalidade:
--     Armazena os valores de RATE (custos/hora/minuto) utilizados nos
--     cálculos de Cost Recovery (CR) para cada ANO.
--
-- Contexto:
--     Esses valores servem de base para:
--       - Cálculo de downtime
--       - Cálculo de SPPS
--       - Cálculo de horas de facilities
--       - Cálculo de mão de obra GM
--       - Cálculo total de custo de contenção e retrabalho
--
-- Observações:
--     - A tabela é anual (um registro por ano).
--     - Permite atualizar valores anualmente sem afetar anos anteriores.
--     - Campo "ativo" controla qual ano está em uso no sistema.
--
-- Exemplos:
--     ano = 2024
--     valor_downtime_minuto = 153,20
--     valor_abertura_spps   = 512,00
--     valor_facilities_hora = 220,00
--     valor_mao_obra_gm_hora = 89,50
--
-- ======================================================================

create table rate_cr_ano_rasp (

    -- Identificador único do conjunto de rates por ano
    id_rate_cr_ano serial primary key,

    -- Ano ao qual estes valores pertencem (único)
    ano smallint not null unique,

    -- Valor de downtime por minuto
    valor_downtime_minuto numeric(12,2) not null,

    -- Custo para abertura de um SPPS
    valor_abertura_spps numeric(12,2) not null,

    -- Custo por hora de facilities
    valor_facilities_hora numeric(12,2) not null,

    -- Custo por hora de mão de obra GM
    valor_mao_obra_gm_hora numeric(12,2) not null,

    -- Controle de ativação deste conjunto de rates
    ativo boolean not null default true
);
