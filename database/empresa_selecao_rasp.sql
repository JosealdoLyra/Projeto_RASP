-- ======================================================================
-- TABELA: empresa_selecao_rasp
--
-- Finalidade:
--     Armazena as empresas responsáveis por executar ações de seleção,
--     contenção ou triagem de peças durante o processo do RASP.
--
-- Contexto:
--     - Pode incluir empresas terceiras (providers), equipes internas da GM,
--       ou outras entidades que realizam inspeções de qualidade.
--     - Usada principalmente na tabela rasp_contencao para registrar
--       quem executou a seleção em cada dia.
--
-- Exemplos de valores possíveis:
--     - GM
--     - Provider A
--     - Provider B
--     - Laboratório interno
--
-- Observação:
--     Campo "tipo_empresa" pode indicar:
--         'GM', 'Provider', 'Laboratório', 'Interna', etc.
--
-- ======================================================================

create table empresa_selecao_rasp (

    -- Identificador único da empresa
    id_empresa_selecao serial primary key,

    -- Nome completo da empresa (valor único)
    nome_empresa varchar(100) not null unique,

    -- Tipo de empresa executora da seleção (ex.: 'GM', 'Provider')
    tipo_empresa varchar(20) not null,

    -- Define se a empresa continua ativa para seleção no sistema
    ativo boolean not null default true
);

