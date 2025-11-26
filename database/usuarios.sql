create table usuarios (
    id_usuario  serial       primary key,
    nome        varchar(60)  not null,
    gmin        varchar(9)   not null unique,
    email       varchar(150) not null,
    cargo       varchar(80)  not null,            -- descrição livre: Analista, FT 2º turno, etc.
    ativo       boolean      not null default true,
    senha_hash  varchar(200)                      -- hash da senha (quando tiver login)
);