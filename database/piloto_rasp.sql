-- ======================================================================
-- TABELA: piloto_rasp
--
-- Finalidade:
--     Armazena os códigos de pilotos, lotes pilotos ou fases específicas
--     de pré-produção relacionadas ao RASP.
--
-- Contexto:
--     - Usada para identificar se o problema ocorreu em lote piloto,
--       VP, PP, PV, ou qualquer outra fase definida pela engenharia.
--     - Campo auxiliar utilizado diretamente na tabela principal RASP.
--
-- Exemplos possíveis de valores:
--     - 'VP'  (Veículo Pré-Série)
--     - 'PP'  (Pré-Produção)
--     - 'PV'  (Ponto de Verificação)
--     - 'LT1' (Lote piloto 1)
--
-- Observação:
--     "descricao" é curta porque normalmente são siglas (2 a 5 caracteres).
--     ordem_exibicao controla a sequência das opções no front-end.
--
-- ======================================================================

create table piloto_rasp (

    -- Identificador único da fase/piloto
    id_piloto_rasp serial primary key,

    -- Código da fase/piloto (sigla única)
    descricao varchar(5) not null unique,

    -- Ordem de apresentação em menus e relatórios
    ordem_exibicao smallint not null
);
