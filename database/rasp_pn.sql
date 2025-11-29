-- ============================================
-- TABELA RASP_PN
-- Relaciona cada PN envolvido no RASP
-- ============================================

create table rasp_pn (
    id_rasp_pn serial primary key,

    -- Relacionamento com a tabela principal
    id_rasp int not null references rasp(id_rasp),

    -- Peça envolvida
    pn varchar(30) not null,

    -- Quantidades
    quantidade_suspeita int,
    quantidade_checada int,
    quantidade_rejeitada int,

    -- Flag de contenção
    em_contencao boolean default false,

    -- Informações de fornecedor
    duns varchar(15),

    -- Ordem de exibição no front-end
    ordem_exibicao smallint not null
);

