-- Database: rasp_db

-- DROP DATABASE IF EXISTS rasp_db;

CREATE DATABASE rasp_db
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'Portuguese_Brazil.1252'
    LC_CTYPE = 'Portuguese_Brazil.1252'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

COMMENT ON DATABASE rasp_db
    IS 'Base principal do Sistema RASP, utilizada para controle de inspe��es, fornecedores, pe�as, registros di�rios de sele��o e rastreabilidade de PNs, no contexto da Qualidade de Pe�as Compradas (SAC-SJC).
';