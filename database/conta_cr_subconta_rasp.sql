-- ======================================================================
-- TABELA: conta_cr_subconta_rasp
--
-- Finalidade:
--     Armazena as SUBCONTAS vinculadas às contas de CR (Cost Recovery)
--     utilizadas no processo RASP para fins contábeis, financeiros e de
--     rastreabilidade interna.
--
-- Contexto:
--     - Cada CONTA CR (tabela conta_cr_rasp) pode possuir várias SUBCONTAS.
--     - A subconta é necessária para detalhamento do custo da contenção,
--       mão de obra, serviços, inspeções ou qualquer movimentação financeira
--       associada ao RASP.
--
-- Exemplos de uso:
--     - Registrar subconta específica de inspeção manual.
--     - Subconta para serviço de terceiros (provider).
--     - Subconta para mão de obra interna GM.
--
-- Observação:
--     A relação é 1:N -> uma conta CR pode ter várias subcontas.
-- ======================================================================

create table conta_cr_subconta_rasp (

    -- Identificador único da subconta
    id_subconta_cr   serial primary key,

    -- Referência à conta CR principal (tabela conta_cr_rasp)
    id_conta_cr      integer not null
                       references conta_cr_rasp(id_conta_cr),

    -- Código da subconta utilizado na área financeira
    codigo_subconta  varchar(20) not null,

    -- Centro de custo reduzido (burden center) – campo opcional
    burden_center    char(2),

    -- Descrição detalhada da subconta
    descricao_subconta varchar(150),

    -- Define se a subconta está ativa para seleção no sistema
    ativo            boolean not null default true
);

