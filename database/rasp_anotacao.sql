-- ======================================================================
-- TABELA: rasp_anotacao
-- ======================================================================

CREATE TABLE rasp_anotacao (
    -- Identificador único da anotação
    id_rasp_anotacao  serial PRIMARY KEY,

    -- RASP relacionado
    id_rasp           integer NOT NULL,

    -- Usuário que fez a anotação
    id_usuario        integer NOT NULL,

    -- Perfil no momento da anotação (MT, FT, LG, DBA...)
    id_perfil_rasp    integer NOT NULL,

    -- Momento em que a anotação foi registrada
    data_hora         timestamp NOT NULL DEFAULT now(),

    -- Tipo de anotação (ex.: Complemento, Correção, Observação)
    tipo_anotacao     varchar(30) NOT NULL DEFAULT 'Complemento',

    -- Texto da anotação
    texto_anotacao    text NOT NULL,

    -- ==========================================================
    -- Foreign Keys
    -- ==========================================================
    CONSTRAINT fk_rasp_anotacao_rasp
        FOREIGN KEY (id_rasp)
        REFERENCES rasp (id_rasp),

    CONSTRAINT fk_rasp_anotacao_usuario
        FOREIGN KEY (id_usuario)
        REFERENCES usuarios (id_usuario),

    CONSTRAINT fk_rasp_anotacao_perfil
        FOREIGN KEY (id_perfil_rasp)
        REFERENCES perfil_rasp (id_perfil)
);

-- Índices para desempenho em consultas por RASP e por data
CREATE INDEX idx_rasp_anotacao_rasp
    ON rasp_anotacao (id_rasp);

CREATE INDEX idx_rasp_anotacao_rasp_data
    ON rasp_anotacao (id_rasp, data_hora);
