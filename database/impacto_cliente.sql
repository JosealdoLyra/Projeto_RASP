create table impacto_cliente (
    id_impacto_cliente  serial primary key,
    descricao           varchar(50) not null unique,
    ordem_exibicao      smallint not null
);







