create table pn(
    id_pn serial primary key,
    --Código do PN com 8 digitos
    codigo_pn char(8) not null unique,
    -- nome da peça (pode repetir)
    nome_peca varchar(255) not null
);

--Regra para permitir apenas 8 dígitos numéricos
alter table pn
add constraint chk_pn_formato
check (codigo_pn ~ '^[0-9]{8}$');

--Regra para nome da peça ter ao menos 4 caracteres (ignorando espaços nas bordas)
alter table pn
add constraint chk_nome_peca_min_len
check (length(trim(nome_peca))>= 4);