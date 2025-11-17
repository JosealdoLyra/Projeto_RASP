create table setor_rasp (
    id_setor_rasp   serial primary key,
    descricao       varchar(50) not null unique,
    ordem_exibicao  smallint not null
);
