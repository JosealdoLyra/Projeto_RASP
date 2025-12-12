/*
================================================================================
ARQUIVO: rasp_trava_status_rascunho.sql
PROJETO: RASP
BANCO: PostgreSQL

OBJETIVO:
Impedir que um RASP avance de status enquanto estiver em rascunho
(status inicial / preenchimento mínimo).

REGRA DE NEGÓCIO:
- MT pode criar RASP com payload mínimo
- FT/LG só recebem RASP após validação de completude
- Banco garante a regra (não depende só da tela)

AUTOR: Josealdo
DATA: 2025-12
================================================================================
*/
-- Função de validação de avanço de status
CREATE OR REPLACE FUNCTION fn_rasp_bloqueia_avanco_se_rascunho()
RETURNS trigger AS $$
BEGIN
    -- lógica aqui
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger associado à tabela rasp
CREATE TRIGGER trg_rasp_bloqueia_avanco_se_rascunho
BEFORE UPDATE ON rasp
FOR EACH ROW
EXECUTE FUNCTION fn_rasp_bloqueia_avanco_se_rascunho();
