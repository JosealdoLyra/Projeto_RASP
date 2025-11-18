create table origem_fabricacao_rasp (
    id_origem_fabricacao_rasp serial primary key,
    descricao varchar(30) not null unique,
    ordem_exibicao smallint not null
);