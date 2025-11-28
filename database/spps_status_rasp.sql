create table spps_status_rasp (
    id_spps_status_rasp serial primary key,
    nome_status varchar(40) not null unique,
    ativo boolean not null default true,
    ordem_exibicao smallint not null
);
