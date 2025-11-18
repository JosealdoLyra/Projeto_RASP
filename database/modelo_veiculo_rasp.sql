create table modelo_veiculo_rasp(
id_modelo_veiculo_rasp serial primary key,
    descricao varchar(20) not null unique,
    ordem_exibicao smallint not null
);