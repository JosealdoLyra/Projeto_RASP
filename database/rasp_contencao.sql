-- ============================================
-- TABELA RASP_CONTENCAO
-- Registra as ações de contenção/seleção por PN
-- (GM ou Provider), ao longo dos dias.
-- ============================================

create table rasp_contencao (
    id_rasp_contencao serial primary key,

    -- Ligação com o PN do RASP
    id_rasp_pn int not null references rasp_pn,

    -- Controle da seleção
    data_selecao date not null default current_date,
    quantidade_checada int not null,
    quantidade_rejeitada int,
    quantidade_retrabalhada int,
    nr_scrap varchar(30),
    data_lote_verificado date,

    -- Esforço de retrabalho
    horas_retrabalho numeric(6,2),

    -- Quem executou a seleção
    id_empresa_selecao_rasp int references empresa_selecao_rasp,
    id_usuario_execucao int references usuarios,

    -- Tipo de ação e observações
    tipo_acao varchar(30),
    observacao text,

    -- Rastreamento de atualização
    data_atualizacao timestamp not null default current_timestamp
);
