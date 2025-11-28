create table spps_classificacao_rasp (
    id_spps_classificacao_rasp serial primary key,
    descricao varchar(30) not null unique,
    ordem_exibicao smallint not null
);