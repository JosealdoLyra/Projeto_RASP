create table usuarios (
    id_usuario serial primary key,
    nome varchar(60) not null,
    gmin varchar(9) not null unique,
    email varchar(150) not null,
    cargo varchar(80) not null,
    perfil varchar(30) not null,
    ativo boolean not null default true,
    senha_hash varchar(200)
)