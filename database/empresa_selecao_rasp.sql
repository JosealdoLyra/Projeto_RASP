create table empresa_selecao_rasp (
    id_empresa_selecao serial primary key,
    nome_empresa varchar(100) not null unique,
    tipo_empresa varchar(20) not null,
    ativo boolean not null default true
);
