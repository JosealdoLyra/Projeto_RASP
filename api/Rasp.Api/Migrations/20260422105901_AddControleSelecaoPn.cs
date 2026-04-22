using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rasp.Api.Migrations
{
    /// <summary>
    /// Migration de alinhamento do histórico do Entity Framework para o controle
    /// de seleção, trava e QHD no nível de RASP_PN.
    ///
    /// Importante:
    /// - As colunas desta funcionalidade já existem no banco de dados.
    /// - Portanto, esta migration NÃO deve tentar criar novamente essas colunas.
    /// - Também NÃO deve removê-las no Down, para evitar perda de estrutura
    ///   e inconsistência com o banco já consolidado.
    ///
    /// Finalidade:
    /// - Registrar no histórico de migrations que essa etapa já foi considerada.
    /// - Permitir que o EF siga o fluxo sem tentar duplicar colunas existentes.
    /// </summary>
    public partial class AddControleSelecaoPn : Migration
    {
        /// <summary>
        /// Não executa alterações físicas no banco.
        /// Esta migration é apenas de alinhamento do histórico.
        /// </summary>
        /// <param name="migrationBuilder">Builder de operações da migration.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =================================================================
            // MIGRATION DE ALINHAMENTO
            // =================================================================
            // As colunas abaixo já existem na tabela rasp_pn no banco atual:
            // - entrou_selecao
            // - status_selecao
            // - datahora_entrada_selecao
            // - datahora_saida_selecao
            // - trava_ativa
            // - datahora_solicitacao_trava
            // - datahora_remocao_trava
            // - qhd_ativo
            // - datahora_qhd
            //
            // Portanto, não fazemos AddColumn aqui.
            // =================================================================
        }

        /// <summary>
        /// Não remove nada do banco.
        /// Como a estrutura já existe e pode estar em uso, o Down permanece vazio.
        /// </summary>
        /// <param name="migrationBuilder">Builder de operações da migration.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =================================================================
            // MIGRATION DE ALINHAMENTO
            // =================================================================
            // Não remover colunas no Down.
            // Esta migration existe apenas para sincronizar o histórico do EF
            // com a realidade atual do banco.
            // =================================================================
        }
    }
}
