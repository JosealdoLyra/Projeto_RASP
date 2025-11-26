create table perfil_rasp(
    id_perfil serial primary key,
    nome varchar(30) not null unique, -- ADMIN,Analista, FT, LG, Gerente, SA, EQF
    descricao varchar(150)
);