-- ============================================
-- TABELA PRINCIPAL DO SISTEMA RASP
-- Armazena todas as informações centrais do registro
-- ============================================

create table rasp (
    id_rasp serial primary key,

    -- Identificação do RASP
    numero_rasp varchar(20) not null unique,    -- Ex.: 0001/25
    data_criacao date not null default current_date,
    hora_criacao time not null default current_time,

    -- Ligações com tabelas auxiliares
    id_fornecedor_rasp int not null references fornecedor_rasp,
    id_modelo_veiculo_rasp int references modelo_veiculo_rasp,
    id_setor_rasp int references setor_rasp,
    id_turno_rasp int references turno_rasp,
    id_origem_fabricacao_rasp int references origem_fabricacao_rasp,
    id_piloto_rasp int references piloto_rasp,

    id_impacto_cliente_rasp int references impacto_cliente_rasp,
    id_impacto_qualidade_rasp int references impacto_qualidade_rasp,
    id_maior_impacto_rasp int references maior_impacto_rasp,
    id_major_rasp int references major_rasp,
    id_spps_classificacao_rasp int references spps_classificacao_rasp,
    id_spps_status_rasp int references spps_status_rasp,
    id_empresa_selecao_rasp int references empresa_selecao_rasp,
    id_conta_cr_rasp int references conta_cr_rasp,
    id_conta_cr_subconta_rasp int references conta_cr_subconta_rasp,
    id_gm_aliado_rasp int references gm_aliado_rasp,
    id_perfil_rasp int references perfil_rasp,

    -- Informações do problema
    spps_numero varchar(30),
    procedencia varchar(20),                                 -- 'Em análise', 'Concluído', etc.
    descricao_problema text not null,

    -- Flags de qualidade e segurança
    iniciativa_fornecedor boolean default false,
    supplier_alert boolean default false,
    reversao boolean default false,
    safety boolean default false,
    emitiu_prr boolean default false,
    aprovado_lg boolean default false,

    -- Breaking Point (se aplicável)
    bp_texto text,
    bp_serie varchar(17),
    bp_datahora timestamp,

    -- Usuários envolvidos no processo
    id_analista int references usuarios,
    id_aprovador_ft int references usuarios,
    id_aprovador_lg int references usuarios
);
