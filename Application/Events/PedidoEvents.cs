namespace SportStore.Application.Events;

/// <summary>
/// Evento: Pedido Criado
/// </summary>
public class PedidoCriadoEvent : BaseEvent
{
    public PedidoCriadoEvent(int pedidoId, string clienteDocumento, string nomeCliente, string vendedor, List<ItemPedidoDto> itens, decimal valorTotal)
    {
        Evento = "pedidos.criado";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            pedidoId = $"PED{pedidoId}",
            clienteDocumento,
            nomeCliente,
            vendedor,
            itens,
            valorTotal
        };
    }
}

public class ItemPedidoDto
{
    public string ProdutoId { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
} 