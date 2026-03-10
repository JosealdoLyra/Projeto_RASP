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

        public DbSet<StatusRasp> StatusRasp => Set<StatusRasp>();
        public DbSet<PnRasp> PnRasp => Set<PnRasp>();
        public DbSet<FornecedorRasp> FornecedorRasp => Set<FornecedorRasp>();
        public DbSet<PerfilRasp> PerfilRasp => Set<PerfilRasp>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<RaspEntity> Rasp => Set<RaspEntity>();
        public DbSet<RaspPnEntity> RaspPn => Set<RaspPnEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<PerfilRasp>(entity =>
            {
                entity.ToTable("perfil_rasp");

                entity.HasKey(e => e.IdPerfil);

                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil");

                entity.Property(e => e.Nome)
                    .HasColumnName("nome");
            });

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
            });

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

            base.OnModelCreating(modelBuilder);
        }
    }
}