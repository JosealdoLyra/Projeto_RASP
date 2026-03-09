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

                entity.Property(e => e.Senha)
                    .HasColumnName("senha");
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

                entity.Property(e => e.DescricaoProblema)
                    .HasColumnName("descricao_problema");

                entity.Property(e => e.IdStatusRasp)
                    .HasColumnName("id_status_rasp");
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
        }
    }
}