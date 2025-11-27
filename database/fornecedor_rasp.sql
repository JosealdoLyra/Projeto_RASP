create table fornecedor_rasp (
    id_fornecedor       serial primary key,
    duns                varchar(9)  not null unique,
    nome                varchar(150) not null,
    tipo_fornecedor     varchar(10)  not null
        check (tipo_fornecedor in ('LOCAL', 'IMPORTADO')),
    ativo               boolean      not null default true
);