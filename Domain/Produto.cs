using System.ComponentModel.DataAnnotations;
using SportStore.Domain.ValueObjects;

namespace SportStore.Domain;

public class Produto
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;
    
    // Usando Value Object Preco para validação robusta
    public Preco Preco { get; private set; } = Preco.Zero;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    public bool Ativo { get; set; } = true;
    
    // Navegação para estoque
    public virtual ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    
    // Navegação para itens do pedido
    public virtual ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();

    /// <summary>
    /// Define o preço do produto usando Value Object
    /// </summary>
    public void DefinirPreco(Preco preco)
    {
        Preco = preco;
    }

    /// <summary>
    /// Define o preço do produto a partir de decimal
    /// </summary>
    public void DefinirPreco(decimal valor)
    {
        Preco = new Preco(valor);
    }

    /// <summary>
    /// Obtém o valor decimal do preço
    /// </summary>
    public decimal ObterValorDecimal()
    {
        return Preco;
    }

    /// <summary>
    /// Obtém o preço formatado em reais
    /// </summary>
    public string ObterPrecoFormatado()
    {
        return Preco.FormatoBrasileiro;
    }
}