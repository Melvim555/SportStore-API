using System.ComponentModel.DataAnnotations;
using SportStore.Domain.ValueObjects;

namespace SportStore.Domain;

public class ItemPedido
{
    public int Id { get; set; }
    
    public int PedidoId { get; set; }
    public virtual Pedido Pedido { get; set; } = null!;
    
    public int ProdutoId { get; set; }
    public virtual Produto Produto { get; set; } = null!;
    
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }
    
    // Usando Value Object Preco para preço unitário
    public Preco PrecoUnitario { get; private set; } = Preco.Zero;
    
    // Calcula subtotal usando Value Objects
    public Preco Subtotal => PrecoUnitario * Quantidade;

    /// <summary>
    /// Define o preço unitário
    /// </summary>
    public void DefinirPrecoUnitario(Preco preco)
    {
        PrecoUnitario = preco;
    }

    /// <summary>
    /// Define o preço unitário a partir de decimal
    /// </summary>
    public void DefinirPrecoUnitario(decimal valor)
    {
        PrecoUnitario = new Preco(valor);
    }

    /// <summary>
    /// Obtém o preço unitário como decimal
    /// </summary>
    public decimal ObterPrecoUnitarioDecimal()
    {
        return PrecoUnitario;
    }

    /// <summary>
    /// Obtém o subtotal como decimal
    /// </summary>
    public decimal ObterSubtotalDecimal()
    {
        return Subtotal;
    }
}