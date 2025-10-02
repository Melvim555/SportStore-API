using Microsoft.EntityFrameworkCore;
using SportStore.Domain;
using SportStore.Domain.ValueObjects;

namespace SportStore.Infrastructure;

public class SportStoreContext : DbContext
{
    public SportStoreContext(DbContextOptions<SportStoreContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração do Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Uuid).IsUnique();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Senha).IsRequired();
            entity.Property(e => e.TipoUsuario).IsRequired();
            entity.Property(e => e.Uuid).IsRequired();
        });

        // Configuração do Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(1000);
            // Mapear Value Object Preco para coluna decimal
            entity.Property(e => e.Preco)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => new Preco(v)
                );
        });

        // Configuração do Estoque
        modelBuilder.Entity<Estoque>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NotaFiscal).HasMaxLength(200);
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.Estoques)
                .HasForeignKey(e => e.ProdutoId);
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId);
        });

        // Configuração do Pedido
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NomeCliente).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired();
            // Mapear Value Object Documento para coluna string
            entity.Property(e => e.DocumentoCliente)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(
                    v => v.Numero,
                    v => new Documento(v)
                );
            // Mapear Value Object Preco para coluna decimal
            entity.Property(e => e.ValorTotal)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => new Preco(v)
                );
            entity.HasOne(e => e.Vendedor)
                .WithMany()
                .HasForeignKey(e => e.VendedorId);
            entity.HasMany(e => e.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId);
        });

        // Configuração do ItemPedido
        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).IsRequired();
            // Mapear Value Object Preco para coluna decimal
            entity.Property(e => e.PrecoUnitario)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => new Preco(v)
                );
            entity.HasOne(e => e.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(e => e.PedidoId);
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(e => e.ProdutoId);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nome = "Administrador Sistema",
                Email = "admin@sportstore.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                TipoUsuario = TipoUsuario.Administrador,
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                Uuid = Guid.NewGuid()
            }
        );
    }
}