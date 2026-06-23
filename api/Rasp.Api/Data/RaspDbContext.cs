using Microsoft.EntityFrameworkCore;
using Rasp.Api.Models;

namespace Rasp.Api.Data
{
    public class RaspDbContext : DbContext
    {
        public RaspDbContext(DbContextOptions<RaspDbContext> options)
            : base(options)
        {
        }

        // ---------------------------------------------------------------------
        // DbSets
        // ---------------------------------------------------------------------
        // Cada DbSet representa uma tabela principal usada pela API.

        public DbSet<StatusRasp> StatusRasp => Set<StatusRasp>();
        public DbSet<PnRasp> PnRasp => Set<PnRasp>();
        public DbSet<FornecedorRasp> FornecedorRasp => Set<FornecedorRasp>();
        public DbSet<PerfilRasp> PerfilRasp => Set<PerfilRasp>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<UsuarioTerceiro> UsuariosTerceiros => Set<UsuarioTerceiro>();

        public DbSet<RaspArquivo> RaspArquivo => Set<RaspArquivo>();
        public DbSet<RaspEntity> Rasp => Set<RaspEntity>();

        public DbSet<OnePageRaspEntity> OnePageRasp => Set<OnePageRaspEntity>();


        public DbSet<RaspHistoricoFluxoEntity> RaspHistoricoFluxo => Set<RaspHistoricoFluxoEntity>();

        public DbSet<RaspPnEntity> RaspPn => Set<RaspPnEntity>();
        public DbSet<RaspAnotacao> RaspAnotacao => Set<RaspAnotacao>();
        public DbSet<ModeloVeiculoRasp> ModeloVeiculoRasp => Set<ModeloVeiculoRasp>();
        public DbSet<SetorRasp> SetorRasp => Set<SetorRasp>();
        public DbSet<TurnoRasp> TurnoRasp => Set<TurnoRasp>();
        public DbSet<OrigemFabricacaoRasp> OrigemFabricacaoRasp => Set<OrigemFabricacaoRasp>();
        public DbSet<PilotoRasp> PilotoRasp => Set<PilotoRasp>();
        public DbSet<ImpactoClienteRasp> ImpactoClienteRasp => Set<ImpactoClienteRasp>();
        public DbSet<ImpactoQualidadeRasp> ImpactoQualidadeRasp => Set<ImpactoQualidadeRasp>();
        public DbSet<MaiorImpactoRasp> MaiorImpactoRasp => Set<MaiorImpactoRasp>();
        public DbSet<MajorRasp> MajorRasp => Set<MajorRasp>();
        public DbSet<EmpresaSelecaoRasp> EmpresaSelecaoRasp => Set<EmpresaSelecaoRasp>();
        public DbSet<ContaCrRasp> ContaCrRasp => Set<ContaCrRasp>();
        public DbSet<ContaCrSubcontaRasp> ContaCrSubcontaRasp => Set<ContaCrSubcontaRasp>();
        public DbSet<GmAliadoRasp> GmAliadoRasp => Set<GmAliadoRasp>();
        public DbSet<SppsClassificacaoRasp> SppsClassificacaoRasp => Set<SppsClassificacaoRasp>();
        public DbSet<SppsStatusRasp> SppsStatusRasp => Set<SppsStatusRasp>();
        public DbSet<OperadorSelecaoTerceiro> OperadorSelecaoTerceiro => Set<OperadorSelecaoTerceiro>();
        public DbSet<IndiceOperacionalRasp> IndiceOperacionalRasp => Set<IndiceOperacionalRasp>();
        public DbSet<RaspBp> RaspBp => Set<RaspBp>();

        public DbSet<RaspLgHistoricoEntity> RaspLgHistorico => Set<RaspLgHistoricoEntity>();

        public DbSet<ScrapTable> ScrapTable => Set<ScrapTable>();


        public DbSet<RaspContencao> RaspContencao { get; set;}

        public DbSet<RaspAnexo> RaspAnexos => Set<RaspAnexo>();

        
            protected override void OnModelCreating(ModelBuilder modelBuilder)
{
            // -----------------------------------------------------------------
            // RASP LG HISTÓRICO
            // -----------------------------------------------------------------
            modelBuilder.Entity<RaspLgHistoricoEntity>(entity =>
            {
                entity.ToTable("rasp_lg_historico");

                entity.HasKey(e => e.IdRaspLgHistorico);

                entity.Property(e => e.IdRaspLgHistorico)
                    .HasColumnName("id_rasp_lg_historico");

                entity.Property(e => e.IdRasp)
                    .HasColumnName("id_rasp");

                entity.Property(e => e.IdUsuarioLg)
                    .HasColumnName("id_usuario_lg");

                entity.Property(e => e.Decisao)
                    .HasColumnName("decisao");

                entity.Property(e => e.Justificativa)
                    .HasColumnName("justificativa");

                entity.Property(e => e.DataHora)
                    .HasColumnName("data_hora");
            });

            // -----------------------------------------------------------------
            // STATUS RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<StatusRasp>(entity =>
            {
                entity.ToTable("status_rasp");

                entity.HasKey(e => e.IdStatusRasp);

                entity.Property(e => e.IdStatusRasp)
                    .HasColumnName("id_status_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemFluxo)
                    .HasColumnName("ordem_fluxo");
            });

            // -----------------------------------------------------------------
            // RASP ARQUIVO
            // -----------------------------------------------------------------
            modelBuilder.Entity<RaspArquivo>(entity =>
            {
                entity.ToTable("rasp_arquivo");

                entity.HasKey(e => e.IdArquivoRasp);

                entity.Property(e => e.IdArquivoRasp)
                    .HasColumnName("id_arquivo_rasp");

                entity.Property(e => e.IdRasp)
                    .HasColumnName("id_rasp");

                entity.Property(e => e.TipoArquivo)
                    .HasColumnName("tipo_arquivo");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.CaminhoArquivo)
                    .HasColumnName("caminho_arquivo");

                entity.Property(e => e.DataUpload)
                    .HasColumnName("data_upload");

                entity.Property(e => e.IdUsuarioUpload)
                    .HasColumnName("id_usuario_upload");
            });


            // -----------------------------------------------------------------
            // PN RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<PnRasp>(entity =>
            {
                entity.ToTable("pn_rasp");

                entity.HasKey(e => e.IdPn);

                entity.Property(e => e.IdPn)
                    .HasColumnName("id_pn");

                entity.Property(e => e.CodigoPn)
                    .HasColumnName("codigo_pn");

                entity.Property(e => e.NomePeca)
                    .HasColumnName("nome_peca");
            });

            // -----------------------------------------------------------------
            // FORNECEDOR RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<FornecedorRasp>(entity =>
            {
                entity.ToTable("fornecedor_rasp");

                entity.HasKey(e => e.IdFornecedor);

                entity.Property(e => e.IdFornecedor)
                    .HasColumnName("id_fornecedor");

                entity.Property(e => e.Duns)
                    .HasColumnName("duns");

                entity.Property(e => e.Nome)
                    .HasColumnName("nome");

                entity.Property(e => e.TipoFornecedor)
                    .HasColumnName("tipo_fornecedor");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo");

                entity.Property(e => e.IdPais)
                    .HasColumnName("id_pais");
            });

            // -----------------------------------------------------------------
            // PERFIL RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<PerfilRasp>(entity =>
            {
                entity.ToTable("perfil_rasp");

                entity.HasKey(e => e.IdPerfil);

                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil");

                entity.Property(e => e.Nome)
                    .HasColumnName("nome");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");
            });

            // -----------------------------------------------------------------
            // MODELO VEÍCULO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<ModeloVeiculoRasp>(entity =>
            {
                entity.ToTable("modelo_veiculo_rasp");

                entity.HasKey(e => e.IdModeloVeiculoRasp);

                entity.Property(e => e.IdModeloVeiculoRasp)
                    .HasColumnName("id_modelo_veiculo_rasp");

                entity.Property(e => e.NomeModelo)
                    .HasColumnName("nome_modelo");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // SETOR RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<SetorRasp>(entity =>
            {
                entity.ToTable("setor_rasp");

                entity.HasKey(e => e.IdSetorRasp);

                entity.Property(e => e.IdSetorRasp)
                    .HasColumnName("id_setor_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // TURNO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<TurnoRasp>(entity =>
            {
                entity.ToTable("turno_rasp");

                entity.HasKey(e => e.IdTurnoRasp);

                entity.Property(e => e.IdTurnoRasp)
                    .HasColumnName("id_turno_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // ORIGEM FABRICAÇÃO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<OrigemFabricacaoRasp>(entity =>
            {
                entity.ToTable("origem_fabricacao_rasp");

                entity.HasKey(e => e.IdOrigemFabricacaoRasp);

                entity.Property(e => e.IdOrigemFabricacaoRasp)
                    .HasColumnName("id_origem_fabricacao_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // PILOTO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<PilotoRasp>(entity =>
            {
                entity.ToTable("piloto_rasp");

                entity.HasKey(e => e.IdPilotoRasp);

                entity.Property(e => e.IdPilotoRasp)
                    .HasColumnName("id_piloto_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

// -----------------------------------------------------------------
// USUÁRIOS GM
// -----------------------------------------------------------------
modelBuilder.Entity<Usuario>(entity =>
{
    entity.ToTable("usuarios");


    entity.HasKey(e => e.IdUsuario);


// -----------------------------------------------------------------
// ANEXOS RASP
// -----------------------------------------------------------------
    modelBuilder.Entity<RaspAnexo>(entity =>
{
    entity.ToTable("rasp_anexos");

    entity.HasKey(e => e.IdAnexo);

    entity.Property(e => e.IdAnexo).HasColumnName("id_anexo");
    entity.Property(e => e.IdRasp).HasColumnName("id_rasp");
    entity.Property(e => e.NumeroRasp).HasColumnName("numero_rasp");
    entity.Property(e => e.TipoAnexo).HasColumnName("tipo_anexo");
    entity.Property(e => e.Posicao).HasColumnName("posicao");
    entity.Property(e => e.NomeOriginal).HasColumnName("nome_original");
    entity.Property(e => e.NomeArquivo).HasColumnName("nome_arquivo");
    entity.Property(e => e.CaminhoArquivo).HasColumnName("caminho_arquivo");
    entity.Property(e => e.Extensao).HasColumnName("extensao");
    entity.Property(e => e.DataUpload).HasColumnName("data_upload");
    entity.Property(e => e.Ativo).HasColumnName("ativo");
});



    


    // =============================================================
    // IDENTIFICAÇÃO
    // =============================================================
    entity.Property(e => e.IdUsuario)
        .HasColumnName("id_usuario");


    entity.Property(e => e.Nome)
        .HasColumnName("nome");


    entity.Property(e => e.Sobrenome)
        .HasColumnName("sobrenome");


    entity.Property(e => e.Gmin)
        .HasColumnName("gmin");


    // =============================================================
    // DADOS CORPORATIVOS
    // =============================================================
    entity.Property(e => e.Email)
        .HasColumnName("email");


    entity.Property(e => e.Cargo)
        .HasColumnName("cargo");


    // =============================================================
    // CONTROLE DE ACESSO
    // =============================================================
    entity.Property(e => e.Ativo)
        .HasColumnName("ativo");


    entity.Property(e => e.Administrador)
        .HasColumnName("administrador");


    entity.Property(e => e.PrimeiroAcesso)
        .HasColumnName("primeiro_acesso");


    entity.Property(e => e.IdPerfil)
        .HasColumnName("id_perfil");


    entity.Property(e => e.IdTurnoRasp)
        .HasColumnName("id_turno_rasp");


    // =============================================================
    // SENHA
    // =============================================================
    entity.Property(e => e.SenhaHash)
        .HasColumnName("senha_hash");


    // =============================================================
    // AUDITORIA
    // =============================================================
    entity.Property(e => e.DataCriacao)
        .HasColumnName("data_criacao");


    entity.Property(e => e.UltimoLogin)
        .HasColumnName("ultimo_login");
});


    // -----------------------------------------------------------------
    // ONE PAGE
    // -----------------------------------------------------------------//
    //
    //
        modelBuilder.Entity<OnePageRaspEntity>(entity =>
    {
        entity.ToTable("onepage_rasp");

        entity.HasKey(e => e.IdOnepageRasp);

        entity.Property(e => e.IdOnepageRasp)
            .HasColumnName("id_onepage_rasp");

        entity.Property(e => e.IdRasp)
            .HasColumnName("id_rasp");

        entity.Property(e => e.IssueDescription)
            .HasColumnName("issue_description");

        entity.Property(e => e.PreliminaryRootCause)
            .HasColumnName("preliminary_root_cause");

        entity.Property(e => e.ContainmentAction)
            .HasColumnName("containment_action");

        entity.Property(e => e.RootCauseAnalysis)
            .HasColumnName("root_cause_analysis");

        entity.Property(e => e.BreakingPoint)
            .HasColumnName("breaking_point");

        entity.Property(e => e.ImagemPath)
            .HasColumnName("imagem_path");

        entity.Property(e => e.DataCriacao)
            .HasColumnName("data_criacao");

        entity.Property(e => e.IdUsuarioCriacao)
            .HasColumnName("id_usuario_criacao");
    });

// -----------------------------------------------------------------
// USUÁRIOS TERCEIROS
// -----------------------------------------------------------------
modelBuilder.Entity<UsuarioTerceiro>(entity =>
{
    entity.ToTable("usuarios_terceiros");


    entity.HasKey(e => e.IdUsuarioTerceiro);


    // =============================================================
    // IDENTIFICAÇÃO
    // =============================================================
    entity.Property(e => e.IdUsuarioTerceiro)
        .HasColumnName("id_usuario_terceiro");


    entity.Property(e => e.Nome)
        .HasColumnName("nome");


    entity.Property(e => e.Sobrenome)
        .HasColumnName("sobrenome");


    entity.Property(e => e.Gmid)
        .HasColumnName("gmid");


    // =============================================================
    // EMPRESA
    // =============================================================
    entity.Property(e => e.IdEmpresaSelecao)
        .HasColumnName("id_empresa_selecao");


    // =============================================================
    // CONTROLE DE ACESSO
    // =============================================================
    entity.Property(e => e.Ativo)
        .HasColumnName("ativo");


    entity.Property(e => e.PrimeiroAcesso)
        .HasColumnName("primeiro_acesso");


    // =============================================================
    // SENHA
    // =============================================================
    entity.Property(e => e.SenhaHash)
        .HasColumnName("senha_hash");


    // =============================================================
    // AUDITORIA
    // =============================================================
    entity.Property(e => e.DataCriacao)
        .HasColumnName("data_criacao");
});



            // -----------------------------------------------------------------
            // RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<RaspEntity>(entity =>
            {
                entity.ToTable("rasp");

                entity.HasKey(e => e.IdRasp);

                entity.Property(e => e.IdRasp)
                    .HasColumnName("id_rasp");

                entity.Property(e => e.NumeroRasp)
                    .HasColumnName("numero_rasp");

                entity.Property(e => e.DataCriacao)
                    .HasColumnName("data_criacao");

                entity.Property(e => e.HoraCriacao)
                    .HasColumnName("hora_criacao");

                entity.Property(e => e.IdFornecedorRasp)
                    .HasColumnName("id_fornecedor_rasp");

                entity.Property(e => e.IdModeloVeiculoRasp)
                    .HasColumnName("id_modelo_veiculo_rasp");

                entity.Property(e => e.IdSetorRasp)
                    .HasColumnName("id_setor_rasp");

                entity.Property(e => e.IdTurnoRasp)
                    .HasColumnName("id_turno_rasp");

                entity.Property(e => e.IdOrigemFabricacaoRasp)
                    .HasColumnName("id_origem_fabricacao_rasp");

                entity.Property(e => e.IdPilotoRasp)
                    .HasColumnName("id_piloto_rasp");

                entity.Property(e => e.IdImpactoClienteRasp)
                    .HasColumnName("id_impacto_cliente_rasp");

                entity.Property(e => e.IdImpactoQualidadeRasp)
                    .HasColumnName("id_impacto_qualidade_rasp");

                entity.Property(e => e.IdMaiorImpactoRasp)
                    .HasColumnName("id_maior_impacto_rasp");

                entity.Property(e => e.IdMajorRasp)
                    .HasColumnName("id_major_rasp");

                entity.Property(e => e.IdSppsClassificacaoRasp)
                    .HasColumnName("id_spps_classificacao_rasp");

                entity.Property(e => e.IdSppsStatusRasp)
                    .HasColumnName("id_spps_status_rasp");

                entity.Property(e => e.IdEmpresaSelecaoRasp)
                    .HasColumnName("id_empresa_selecao_rasp");

                entity.Property(e => e.IdContaCrRasp)
                    .HasColumnName("id_conta_cr_rasp");

                entity.Property(e => e.IdContaCrSubcontaRasp)
                    .HasColumnName("id_conta_cr_subconta_rasp");

                entity.Property(e => e.IdGmAliadoRasp)
                    .HasColumnName("id_gm_aliado_rasp");

                entity.Property(e => e.IdPerfilRasp)
                    .HasColumnName("id_perfil_rasp");

                entity.Property(e => e.SppsNumero)
                    .HasColumnName("spps_numero");

                entity.Property(e => e.Procedencia)
                    .HasColumnName("procedencia");

                entity.Property(e => e.VinVeiculoProblema)
                    .HasColumnName("vin_veiculo_problema");


                entity.Property(e => e.DescricaoProblema)
                    .HasColumnName("descricao_problema");

                entity.Property(e => e.IniciativaFornecedor)
                    .HasColumnName("iniciativa_fornecedor");

                entity.Property(e => e.SupplierAlert)
                    .HasColumnName("supplier_alert");

                entity.Property(e => e.Reversao)
                    .HasColumnName("reversao");

                entity.Property(e => e.Safety)
                    .HasColumnName("safety");

                entity.Property(e => e.EmitiuPrr)
                    .HasColumnName("emitiu_prr");

                entity.Property(e => e.AprovadoLg)
                    .HasColumnName("aprovado_lg");

                entity.Property(e => e.BpTexto)
                    .HasColumnName("bp_texto");

                entity.Property(e => e.BpSerie)
                    .HasColumnName("bp_serie");

                entity.Property(e => e.BpDatahora)
                    .HasColumnName("bp_datahora");

                entity.Property(e => e.IdAnalista)
                    .HasColumnName("id_analista");

                entity.Property(e => e.IdAprovadorFt)
                    .HasColumnName("id_aprovador_ft");


                modelBuilder.Entity<RaspHistoricoFluxoEntity>(entity =>
                {
                    entity.ToTable("rasp_historico_fluxo");

                    entity.HasKey(e => e.IdHistoricoFluxo);

                    entity.Property(e => e.IdHistoricoFluxo).HasColumnName("id_historico_fluxo");
                    entity.Property(e => e.IdRasp).HasColumnName("id_rasp");
                    entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                    entity.Property(e => e.Acao).HasColumnName("acao");
                    entity.Property(e => e.StatusAnterior).HasColumnName("status_anterior");
                    entity.Property(e => e.StatusNovo).HasColumnName("status_novo");
                    entity.Property(e => e.Observacao).HasColumnName("observacao");
                    entity.Property(e => e.DataHora).HasColumnName("data_hora");
                    entity.Property(e => e.DataHoraAnterior).HasColumnName("data_hora_anterior");
                    entity.Property(e => e.TempoFaseMinutos).HasColumnName("tempo_fase_minutos");
                    entity.Property(e => e.OrigemEvento).HasColumnName("origem_evento");
                    entity.Property(e => e.TipoComplemento).HasColumnName("tipo_complemento");
                });


                    // ==========================================================
                    // DATAS E JUSTIFICATIVAS DO FLUXO FT / LG
                    // ==========================================================

                    entity.Property(e => e.DataEnvioFt)
                        .HasColumnName("data_envio_ft");

                    entity.Property(e => e.DataAprovacaoFt)
                        .HasColumnName("data_aprovacao_ft");

                    entity.Property(e => e.DataEnvioLg)
                        .HasColumnName("data_envio_lg");

                    entity.Property(e => e.DataAprovacaoLg)
                        .HasColumnName("data_aprovacao_lg");

                    entity.Property(e => e.JustificativaFt)
                        .HasColumnName("justificativa_ft");

                    entity.Property(e => e.JustificativaLg)
                        .HasColumnName("justificativa_lg");


                entity.Property(e => e.IdAprovadorLg)
                    .HasColumnName("id_aprovador_lg");

                entity.Property(e => e.AnoRasp)
                    .HasColumnName("ano_rasp");

                entity.Property(e => e.DataFechamento)
                    .HasColumnName("data_fechamento");

                entity.Property(e => e.IdAnalistaMt)
                    .HasColumnName("id_analista_mt");

                entity.Property(e => e.BreakpointTexto)
                    .HasColumnName("breakpoint_texto");

                entity.Property(e => e.BreakpointCodigo)
                    .HasColumnName("breakpoint_codigo");

                entity.Property(e => e.BreakpointDatahora)
                    .HasColumnName("breakpoint_datahora");

                entity.Property(e => e.IsSupplierAlert)
                    .HasColumnName("is_supplier_alert");

                entity.Property(e => e.IsSafety)
                    .HasColumnName("is_safety");

                entity.Property(e => e.IsReversao)
                    .HasColumnName("is_reversao");

                entity.Property(e => e.GeraPrr)
                    .HasColumnName("gera_prr");

                entity.Property(e => e.ObservacaoGeral)
                    .HasColumnName("observacao_geral");

                entity.Property(e => e.IsRascunho)
                    .HasColumnName("is_rascunho");

                entity.Property(e => e.PercentualCompletude)
                    .HasColumnName("percentual_completude");

                entity.Property(e => e.IdStatusRasp)
                    .HasColumnName("id_status_rasp");

                entity.Property(e => e.RdNumero).HasColumnName("rd_numero");
                entity.Property(e => e.CampanhaNumero).HasColumnName("campanha_numero");
                entity.Property(e => e.NomeContato).HasColumnName("nome_contato");
                entity.Property(e => e.DataContato).HasColumnName("data_contato");


                entity.Property(e => e.IdIndiceOperacionalRasp)
                    .HasColumnName("id_indice_operacional_rasp");
            });

// -----------------------------------------------------------------
// RASP_PN
// -----------------------------------------------------------------
modelBuilder.Entity<RaspPnEntity>(entity =>
{
    entity.ToTable("rasp_pn");

    entity.HasKey(e => e.IdRaspPn);

    entity.Property(e => e.IdRaspPn)
        .HasColumnName("id_rasp_pn");

    entity.Property(e => e.IdRasp)
        .HasColumnName("id_rasp");

    entity.Property(e => e.Pn)
        .HasColumnName("pn");

    entity.Property(e => e.DataLoteInicial)
        .HasColumnName("data_lote_inicial");

    entity.Property(e => e.QuantidadeSuspeita)
        .HasColumnName("quantidade_suspeita");

    entity.Property(e => e.QuantidadeChecada)
        .HasColumnName("quantidade_checada");

    entity.Property(e => e.QuantidadeRejeitada)
        .HasColumnName("quantidade_rejeitada");

    entity.Property(e => e.EmContencao)
        .HasColumnName("em_contencao");

    entity.Property(e => e.Duns)
        .HasColumnName("duns");

    entity.Property(e => e.OrdemExibicao)
        .HasColumnName("ordem_exibicao");
});

// -----------------------------------------------------------------
// SCRAP TABLE
// -----------------------------------------------------------------
modelBuilder.Entity<ScrapTable>(entity =>
{
    entity.ToTable("scrap_table");

    entity.HasKey(e => e.IdScrap);

    entity.Property(e => e.IdScrap)
        .HasColumnName("id_scrap");

    entity.Property(e => e.NumeroScrap)
        .HasColumnName("numero_scrap");

    entity.Property(e => e.IdRasp)
        .HasColumnName("id_rasp");

    entity.Property(e => e.IdRaspPn)
        .HasColumnName("id_rasp_pn");

    entity.Property(e => e.NumeroRasp)
        .HasColumnName("numero_rasp");

    entity.Property(e => e.NumeroSpps)
        .HasColumnName("numero_spps");

    entity.Property(e => e.IdFornecedor)
        .HasColumnName("id_fornecedor");

    entity.Property(e => e.FornecedorNome)
        .HasColumnName("fornecedor_nome");

    entity.Property(e => e.FornecedorDuns)
        .HasColumnName("fornecedor_duns");

    entity.Property(e => e.OrigemPeca)
        .HasColumnName("origem_peca");

    entity.Property(e => e.Pn)
        .HasColumnName("pn");

    entity.Property(e => e.NomePeca)
        .HasColumnName("nome_peca");

    entity.Property(e => e.Quantidade)
        .HasColumnName("quantidade");

    entity.Property(e => e.TipoDestinacao)
        .HasColumnName("tipo_destinacao");

    entity.Property(e => e.StatusScrap)
        .HasColumnName("status_scrap");

    entity.Property(e => e.BrunetaNumero)
        .HasColumnName("bruneta_numero");

    entity.Property(e => e.NotaFiscalNumero)
        .HasColumnName("nota_fiscal_numero");

    entity.Property(e => e.BaixaEstoqueRealizada)
        .HasColumnName("baixa_estoque_realizada");

    entity.Property(e => e.BaixaEstoqueAutomatica)
        .HasColumnName("baixa_estoque_automatica");

    entity.Property(e => e.DataBaixaEstoque)
        .HasColumnName("data_baixa_estoque");

    entity.Property(e => e.IdUsuarioBaixa)
        .HasColumnName("id_usuario_baixa");

    entity.Property(e => e.ObservacaoBaixa)
        .HasColumnName("observacao_baixa");

    entity.Property(e => e.DataCriacao)
        .HasColumnName("data_criacao");

    entity.Property(e => e.IdUsuarioCriacao)
        .HasColumnName("id_usuario_criacao");

    entity.Property(e => e.Observacao)
        .HasColumnName("observacao");

    entity.Property(e => e.IdUsuarioMt)
        .HasColumnName("id_usuario_mt");

    entity.Property(e => e.DataEnvioFt)
        .HasColumnName("data_envio_ft");

    entity.Property(e => e.IdUsuarioFt)
        .HasColumnName("id_usuario_ft");

    entity.Property(e => e.DataAprovacaoFt)
        .HasColumnName("data_aprovacao_ft");

    entity.Property(e => e.ObservacaoFt)
        .HasColumnName("observacao_ft");

    entity.Property(e => e.DataEnvioLg)
        .HasColumnName("data_envio_lg");

    entity.Property(e => e.IdUsuarioLg)
        .HasColumnName("id_usuario_lg");

    entity.Property(e => e.DataAprovacaoLg)
        .HasColumnName("data_aprovacao_lg");

    entity.Property(e => e.ObservacaoLg)
        .HasColumnName("observacao_lg");

    entity.Property(e => e.DataBruneta)
        .HasColumnName("data_bruneta");

    entity.Property(e => e.IdUsuarioBruneta)
        .HasColumnName("id_usuario_bruneta");

    entity.Property(e => e.DataNotaFiscal)
        .HasColumnName("data_nota_fiscal");

    entity.Property(e => e.IdUsuarioNotaFiscal)
        .HasColumnName("id_usuario_nota_fiscal");

    entity.Property(e => e.BloqueadoEdicao)
        .HasColumnName("bloqueado_edicao");
});


            // -----------------------------------------------------------------
            // RASP ANOTAÇÃO
            // -----------------------------------------------------------------
            modelBuilder.Entity<RaspAnotacao>(entity =>
            {
                entity.ToTable("rasp_anotacao");

                entity.HasKey(e => e.IdRaspAnotacao);

                entity.Property(e => e.IdRaspAnotacao)
                    .HasColumnName("id_rasp_anotacao");

                entity.Property(e => e.IdRasp)
                    .HasColumnName("id_rasp");

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario");

                entity.Property(e => e.IdPerfilRasp)
                    .HasColumnName("id_perfil_rasp");

                entity.Property(e => e.DataHora)
                    .HasColumnName("data_hora");

                entity.Property(e => e.TipoAnotacao)
                    .HasColumnName("tipo_anotacao");

                entity.Property(e => e.TextoAnotacao)
                    .HasColumnName("texto_anotacao");
            });

            // -----------------------------------------------------------------
            // IMPACTO CLIENTE RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<ImpactoClienteRasp>(entity =>
            {
                entity.ToTable("impacto_cliente_rasp");

                entity.HasKey(e => e.IdImpactoCliente);

                entity.Property(e => e.IdImpactoCliente)
                    .HasColumnName("id_impacto_cliente");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // IMPACTO QUALIDADE RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<ImpactoQualidadeRasp>(entity =>
            {
                entity.ToTable("impacto_qualidade_rasp");

                entity.HasKey(e => e.IdImpactoQualidadeRasp);

                entity.Property(e => e.IdImpactoQualidadeRasp)
                    .HasColumnName("id_impacto_qualidade_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");

                entity.Property(e => e.CodigoOpcao)
                    .HasColumnName("codigo_opcao");
            });

            // -----------------------------------------------------------------
            // MAIOR IMPACTO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<MaiorImpactoRasp>(entity =>
            {
                entity.ToTable("maior_impacto_rasp");

                entity.HasKey(e => e.IdMaiorImpactoRasp);

                entity.Property(e => e.IdMaiorImpactoRasp)
                    .HasColumnName("id_maior_impacto_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // MAJOR RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<MajorRasp>(entity =>
            {
                entity.ToTable("major_rasp");

                entity.HasKey(e => e.IdMajorRasp);

                entity.Property(e => e.IdMajorRasp)
                    .HasColumnName("id_major_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // EMPRESA SELEÇÃO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<EmpresaSelecaoRasp>(entity =>
            {
                entity.ToTable("empresa_selecao_rasp");

                entity.HasKey(e => e.IdEmpresaSelecao);

                entity.Property(e => e.IdEmpresaSelecao)
                    .HasColumnName("id_empresa_selecao");

                entity.Property(e => e.NomeEmpresa)
                    .HasColumnName("nome_empresa");

                entity.Property(e => e.TipoEmpresa)
                    .HasColumnName("tipo_empresa");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo");
            });

            // -----------------------------------------------------------------
            // CONTA CR RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<ContaCrRasp>(entity =>
            {
                entity.ToTable("conta_cr_rasp");

                entity.HasKey(e => e.IdContaCr);

                entity.Property(e => e.IdContaCr)
                    .HasColumnName("id_conta_cr");

                entity.Property(e => e.Codigo)
                    .HasColumnName("codigo");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.Observacao)
                    .HasColumnName("observacao");
            });

            // -----------------------------------------------------------------
            // CONTA CR SUBCONTA RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<ContaCrSubcontaRasp>(entity =>
            {
                entity.ToTable("conta_cr_subconta_rasp");

                entity.HasKey(e => e.IdSubcontaCr);

                entity.Property(e => e.IdSubcontaCr)
                    .HasColumnName("id_subconta_cr");

                entity.Property(e => e.IdContaCr)
                    .HasColumnName("id_conta_cr");

                entity.Property(e => e.CodigoSubconta)
                    .HasColumnName("codigo_subconta");

                entity.Property(e => e.BurdenCenter)
                    .HasColumnName("burden_center");

                entity.Property(e => e.DescricaoSubconta)
                    .HasColumnName("descricao_subconta");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo");
            });

            // -----------------------------------------------------------------
            // GM ALIADO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<GmAliadoRasp>(entity =>
            {
                entity.ToTable("gm_aliado_rasp");

                entity.HasKey(e => e.IdGmAliadoRasp);

                entity.Property(e => e.IdGmAliadoRasp)
                    .HasColumnName("id_gm_aliado_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // SPPS CLASSIFICAÇÃO RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<SppsClassificacaoRasp>(entity =>
            {
                entity.ToTable("spps_classificacao_rasp");

                entity.HasKey(e => e.IdSppsClassificacaoRasp);

                entity.Property(e => e.IdSppsClassificacaoRasp)
                    .HasColumnName("id_spps_classificacao_rasp");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");
            });

            // -----------------------------------------------------------------
            // SPPS STATUS RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<SppsStatusRasp>(entity =>
            {
                entity.ToTable("spps_status_rasp");

                entity.HasKey(e => e.IdSppsStatusRasp);

                entity.Property(e => e.IdSppsStatusRasp)
                    .HasColumnName("id_spps_status_rasp");

                entity.Property(e => e.NomeStatus)
                    .HasColumnName("nome_status");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo");

                entity.Property(e => e.OrdemExibicao)
                    .HasColumnName("ordem_exibicao");

                entity.Property(e => e.EhFinal)
                    .HasColumnName("eh_final");

                entity.Property(e => e.ContaParaPrazo)
                    .HasColumnName("conta_para_prazo");
            });

            // -----------------------------------------------------------------
            // ÍNDICE OPERACIONAL RASP
            // -----------------------------------------------------------------
            modelBuilder.Entity<IndiceOperacionalRasp>(entity =>
            {
                entity.ToTable("indice_operacional_rasp");

                entity.HasKey(e => e.IdIndiceOperacionalRasp);

                entity.Property(e => e.IdIndiceOperacionalRasp)
                    .HasColumnName("id_indice_operacional_rasp");

                entity.Property(e => e.CodigoOpcao)
                    .HasColumnName("codigo_opcao");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao");
            });
// -----------------------------------------------------------------
// OPERADOR SELEÇÃO TERCEIRO
// -----------------------------------------------------------------
modelBuilder.Entity<OperadorSelecaoTerceiro>(entity =>
{
    entity.ToTable("operador_selecao_terceiro");

    entity.HasKey(e => e.IdOperadorSelecaoTerceiro);

    entity.Property(e => e.IdOperadorSelecaoTerceiro)
        .HasColumnName("id_operador_selecao_terceiro");

    entity.Property(e => e.Nome)
        .HasColumnName("nome");

    entity.Property(e => e.Empresa)
        .HasColumnName("empresa");

    entity.Property(e => e.Ativo)
        .HasColumnName("ativo");

    entity.Property(e => e.Observacao)
        .HasColumnName("observacao");

    entity.Property(e => e.DataCriacao)
        .HasColumnName("data_criacao");

    entity.Property(e => e.DataInativacao)
        .HasColumnName("data_inativacao");
});

            base.OnModelCreating(modelBuilder);
        }
    }
}