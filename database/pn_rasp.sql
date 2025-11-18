create table pn_rasp (
    id_pn_rasp serial primary key,
    pn varchar(8) not null unique,
    nome_peca varchar(100) not null,
    constraint chk_nome_peca_length check (char_length(nome_peca) >= 4)
);