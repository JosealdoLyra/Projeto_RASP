-- ======================================================================
-- TABELA: rasp_aprovacao
--
-- Finalidade:
--     Guardar o HISTÓRICO de aprovações e decisões do RASP por nível
--     (FT, LG, etc.), permitindo rastrear:
--       - se aprovou ou não,
--       - se gerou SPPS,
--       - se gerou Supplier Alert,
--       - justificativa e data de cada decisão.
--
-- Cenários principais:
--     1) FT NÃO aprova → resultado = 'NAO_APROVADO', gera_spps = false,
--        gera_supplier_alert = false, justificativa em 'observacao'.
--
--     2) FT aprova → resultado = 'APROVADO', libera para LG.
--
--     3) LG NÃO aprova → resultado = 'NAO_APROVADO',
--        gera_spps = false, gera_supplier_alert = false.
--
--     4) LG aprova como Supplier Alert → resultado = 'APROVADO',
--        gera_spps = false, gera_supplier_alert = true,
--        justificativa obrigatória (validada pela aplicação).
--
-- Observação:
--     - Pode haver mais de um registro por RASP e por nível, em caso de
--       reavaliações; o histórico é preservado.
-- ======================================================================

create table rasp_aprovacao (
    -- Identificador único do registro de aprovação
    id_rasp_aprovacao serial primary key,

    -- RASP ao qual esta decisão está vinculada
    id_rasp int not null
        references rasp(id_rasp)
        on delete cascade,

    -- Nível da aprovação (quem está decidindo)
    nivel_aprovacao varchar(10) not null
        check (nivel_aprovacao in ('FT', 'LG', 'OUTRO')),

    -- Resultado da decisão neste nível
    -- APROVADO         → segue o fluxo
    -- NAO_APROVADO     → barra o fluxo naquele nível
    resultado varchar(20) not null
        check (resultado in ('APROVADO', 'NAO_APROVADO')),

    -- Indica se desta decisão surgiu um SPPS
    gera_spps boolean not null default false,

    -- Indica se desta decisão surgiu um Supplier Alert
    gera_supplier_alert boolean not null default false,

    -- Usuário que realizou a decisão (FT, LG, gerente, etc.)
    id_usuario int not null
        references usuarios(id_usuario),

    -- Data e hora em que a decisão foi tomada/registrada
    data_decisao timestamp not null default current_timestamp,

    -- Justificativa da decisão (principalmente quando não aprovado
    -- ou quando for Supplier Alert)
    observacao text
);
