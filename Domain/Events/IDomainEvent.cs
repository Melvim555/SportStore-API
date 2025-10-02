namespace SportStore.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public class PedidoCriadoEvent : IDomainEvent
{
    public int PedidoId { get; }
    public int VendedorId { get; }
    public decimal ValorTotal { get; }
    public DateTime OccurredOn { get; }

    public PedidoCriadoEvent(int pedidoId, int vendedorId, decimal valorTotal)
    {
        PedidoId = pedidoId;
        VendedorId = vendedorId;
        ValorTotal = valorTotal;
        OccurredOn = DateTime.UtcNow;
    }
}

public class EstoqueBaixoEvent : IDomainEvent
{
    public int ProdutoId { get; }
    public string ProdutoNome { get; }
    public int QuantidadeAtual { get; }
    public DateTime OccurredOn { get; }

    public EstoqueBaixoEvent(int produtoId, string produtoNome, int quantidadeAtual)
    {
        ProdutoId = produtoId;
        ProdutoNome = produtoNome;
        QuantidadeAtual = quantidadeAtual;
        OccurredOn = DateTime.UtcNow;
    }
}
