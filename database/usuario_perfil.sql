create table usuario_perfil(
    id_usuario int not null references usuarios(id_usuario) on delete cascade,
    id_perfil int not null references perfil_rasp(id_perfil),
    primary key (id_usuario, id_perfil)
)