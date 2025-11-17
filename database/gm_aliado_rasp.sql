create table gm_aliado_rasp(
    id_gm_aliado_rasp serial primary key,
    descricao varchar(5) not null unique,
    ordem_exibicao smallint not null
)

