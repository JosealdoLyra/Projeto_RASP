create table conta_cr_subconta_rasp (
    id_subconta_cr   serial primary key,
    id_conta_cr      integer not null references conta_cr_rasp(id_conta_cr),
    codigo_subconta  varchar(20) not null,
    burden_center    char(2),
    descricao_subconta varchar(150),
    ativo            boolean not null default true
);
