create table classificacao_spps_rasp (
    id_classificacao_spps_rasp serial primary key,
    nome varchar(30) not null unique,
    descricao varchar(100),
    ordem_exibicao smallint not null
);
