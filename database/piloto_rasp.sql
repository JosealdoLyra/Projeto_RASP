create table piloto_rasp (
    id_piloto_rasp serial primary key,
    descricao varchar(5) not  null unique,
    ordem_exibicao smallint not null
)