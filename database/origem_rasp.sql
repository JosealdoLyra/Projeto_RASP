create table origem_rasp(
    id_origem_rasp serial primary key,
    descricao varchar(20) not null unique,
    ordem_exibicao smallint not null
);