using System.ComponentModel.DataAnnotations;

namespace SportStore.Domain;

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2
}

public class Estoque
{
    public int Id { get; set; }
    
    public int ProdutoId { get; set; }
    public virtual Produto Produto { get; set; } = null!;
    
    public int Quantidade { get; set; }
    
    public TipoMovimentacao TipoMovimentacao { get; set; }
    
    [MaxLength(200)]
    public string? NotaFiscal { get; set; }
    
    [MaxLength(500)]
    public string? Observacoes { get; set; }
    
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
    
    public int UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
}



