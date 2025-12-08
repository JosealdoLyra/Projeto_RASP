CREATE TABLE status_rasp (
    id_status_rasp SERIAL PRIMARY KEY,
    descricao VARCHAR(50) NOT NULL UNIQUE,
    ordem_fluxo SMALLINT NOT NULL
);

INSERT INTO status_rasp (descricao, ordem_fluxo) VALUES
    ('Em análise (MT)', 1),
    ('Em avaliação FT', 2),
    ('Em avaliação LG', 3),
    ('Concluído', 4),
    ('Cancelado', 5);
