create table rate_cr_ano_rasp (
    id_rate_cr_ano        serial primary key,
    ano                   smallint not null unique,
    valor_downtime_minuto numeric(12,2) not null,
    valor_abertura_spps   numeric(12,2) not null,
    valor_facilities_hora numeric(12,2) not null,
    valor_mao_obra_gm_hora numeric(12,2) not null,
    ativo                 boolean not null default true
);
