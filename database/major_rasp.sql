create table major_rasp(
    id_major_rasp serial primary key,
    descricao varchar(30) not null unique,
    ordem_exibicao smallint not null
);