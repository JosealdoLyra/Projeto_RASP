create table rasp_arquivo (
    id_arquivo_rasp   serial primary key,
    id_rasp           integer not null,
    tipo_arquivo      varchar(20) not null,
    descricao         varchar(150),
    caminho_arquivo   varchar(300) not null,
    data_upload       date not null default current_date,
    id_usuario_upload integer not null
);