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
        public DbSet<RaspEntity> Rasp => Set<RaspEntity>();
        public DbSet<RaspPnEntity> RaspPn => Set<RaspPnEntity>();
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
        public DbSet<IndiceOperacionalRasp> IndiceOperacionalRasp => Set<IndiceOperacionalRasp>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // -----------------------------------------------------------------
            // STATUS RASP
            // -----------------------------------------------------------------
            // Tabela de domínio do fluxo do RASP.
            // Ex.: Em análise, Em avaliação FT, Em avaliação LG, etc.
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
            // PN RASP
            // -----------------------------------------------------------------
            // Cadastro mestre de PNs.
            // Regras de negócio do código PN são tratadas na API:
            // - 8 dígitos
            // - somente números
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
            // Cadastro de fornecedores usados no processo RASP.
            // Regras de DUNS são tratadas na API:
            // - 9 dígitos
            // - somente números
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
            // Perfis do sistema.
            // Ex.: ADMIN, ANALISTA, FT, LG.
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
            // USUÁRIOS
            // -----------------------------------------------------------------
            // Usuários do sistema com vínculo ao perfil.
            // O campo id_perfil é importante para as regras de permissão da API.
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");

                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario");

                entity.Property(e => e.Nome)
                    .HasColumnName("nome");

                entity.Property(e => e.Gmin)
                    .HasColumnName("gmin");

                entity.Property(e => e.Email)
                    .HasColumnName("email");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo");

                entity.Property(e => e.Ativo)
                    .HasColumnName("ativo");

                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil");
            });

            // -----------------------------------------------------------------
            // RASP
            // -----------------------------------------------------------------
            // Tabela principal do processo.
            //
            // Observações importantes:
            // - id_analista_mt guarda a autoria do RASP
            // - id_status_rasp controla a fase do fluxo
            // - is_rascunho indica se o RASP ainda está em construção
            // - percentual_completude será importante para validar envio de fase
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

                entity.Property(e => e.IdIndiceOperacionalRasp)
                    .HasColumnName("id_indice_operacional_rasp");
            });

            // -----------------------------------------------------------------
            // RASP_PN
            // -----------------------------------------------------------------
            // Tabela de vínculo entre o RASP e os PNs envolvidos.
            //
            // Regras principais tratadas pela API:
            // - RASP deve existir
            // - PN deve existir
            // - DUNS deve existir
            // - quantidades não negativas
            // - não repetir o mesmo PN no mesmo RASP
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

            base.OnModelCreating(modelBuilder);
        }
    }
}