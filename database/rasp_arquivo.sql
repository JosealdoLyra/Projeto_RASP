-- ============================================================
-- TABELA: rasp_arquivo
-- Finalidade:
--   Armazena os arquivos anexados ao RASP, como fotos, vídeos,
--   PDFs, RADL, prints, evidências de inspeção, etc.
--
-- Observações:
--   - Armazena somente o caminho/URL do arquivo, nunca o binário.
--   - Cada registro pertence a um RASP específico.
--   - Guarda também o usuário que fez o upload e a data.
-- ============================================================

create table rasp_arquivo (

    -- Identificador único do arquivo
    id_arquivo_rasp   serial primary key,

    -- RASP ao qual o arquivo pertence
    id_rasp           integer not null,

    -- Tipo do arquivo (foto, vídeo, pdf, radl, planilha, etc.)
    tipo_arquivo      varchar(20) not null,

    -- Descrição opcional do arquivo
    descricao         varchar(150),

    -- Caminho ou URL onde o arquivo está armazenado
    caminho_arquivo   varchar(300) not null,

    -- Data em que o arquivo foi anexado
    data_upload       date not null default current_date,

    -- Usuário que realizou o upload
    id_usuario_upload integer not null
);

-- Observação:
-- As chaves estrangeiras (id_rasp, id_usuario_upload) podem ser
-- adicionadas agora ou posteriormente, conforme escolha do projeto:
--   references rasp(id_rasp)
--   references usuarios(id_usuario)
