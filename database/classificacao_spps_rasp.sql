-- ======================================================================
-- TABELA: classificacao_spps_rasp
--
-- Finalidade:
--     Armazena as classificações de documentos SPPS relacionadas ao RASP.
--     Esta tabela serve como domínio para definir os tipos de status/
--     classificações que um SPPS pode assumir durante sua análise interna.
--
-- Exemplos de possíveis classificações (dependendo da definição da GM):
--     - Em Verificação do Fornecedor
--     - Procedente
--     - Não Procedente
--     - Rejeitado
--     - Aguardando Evidências
--     - Revisado com Gerente
--     - Etc.
--
-- Observação:
--     Esta tabela NÃO define status (que ficam em `spps_status_rasp`),
--     mas sim a classificação/conclusão do documento SPPS.
-- ======================================================================

create table classificacao_spps_rasp (

    -- Identificador único da classificação SPPS
    id_classificacao_spps_rasp serial primary key,

    -- Nome (curto) da classificação — valor único
    nome varchar(30) not null unique,

    -- Descrição detalhada da classificação (opcional)
    descricao varchar(100),

    -- Ordem de exibição no front-end
    ordem_exibicao smallint not null
);

