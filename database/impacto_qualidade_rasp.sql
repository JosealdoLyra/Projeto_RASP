create table impacto_qualidade_rasp(
    id_impacto_qualidade_rasp serial primary key,
    descricao varchar(1000) not null unique,
    ordem_exibicao smallint not null
);
