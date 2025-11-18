create table maior_impacto_rasp (
    id_maior_impacto_rasp serial primary key,
    descricao             varchar(1000) not null unique,
    ordem_exibicao        smallint      not null
);

