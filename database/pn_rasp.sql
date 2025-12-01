-- ======================================================================
-- TABELA: pn_rasp
--
-- Finalidade:
--     Armazena a lista de PNs (Part Numbers) utilizados no processo RASP.
--     Essa tabela serve como cadastro mestre de peças, permitindo que
--     campos como nome da peça, código e validações sejam padronizados.
--
-- Contexto:
--     - Usado como domínio para seleção de peças no RASP.
--     - Pode ser usado para consulta rápida em telas, integração e relatórios.
--     - Relaciona-se indiretamente com rasp_pn (peças envolvidas no caso).
--
-- Regras aplicadas:
--     * PN deve ser único.
--     * Nome da peça deve possuir ao menos 4 caracteres
--       (garante qualidade mínima da descrição).
--
-- Exemplos:
--     pn: '94763542'
--     nome_peca: 'SUPORTE DO PARACHOQUE'
--
-- ======================================================================

create table pn_rasp (

    -- Identificador único da peça
    id_pn_rasp serial primary key,

    -- Código da peça (PN) — deve ser único
    pn varchar(8) not null unique,

    -- Nome da peça (descrição legível)
    nome_peca varchar(100) not null,

    -- Regra: nome da peça deve ter pelo menos 4 caracteres
    constraint chk_nome_peca_length
        check (char_length(nome_peca) >= 4)
);
