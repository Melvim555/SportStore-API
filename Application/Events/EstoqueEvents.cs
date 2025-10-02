namespace SportStore.Application.Events;

/// <summary>
/// Evento: Estoque Adicionado
/// </summary>
public class EstoqueAdicionadoEvent : BaseEvent
{
    public EstoqueAdicionadoEvent(int produtoId, int quantidade, string notaFiscal, string? observacoes)
    {
        Evento = "estoque.adicionado";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            produtoId = $"P{produtoId}",
            quantidade,
            tipoMovimentacao = "ENTRADA",
            notaFiscal,
            observacoes
        };
    }
}

/// <summary>
/// Evento: Estoque Atualizado (Baixa)
/// </summary>
public class EstoqueAtualizadoEvent : BaseEvent
{
    public EstoqueAtualizadoEvent(int produtoId, int quantidade, string motivo)
    {
        Evento = "estoque.atualizado";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            produtoId = $"P{produtoId}",
            quantidade,
            tipoMovimentacao = "SAIDA",
            motivo
        };
    }
}

/// <summary>
/// Evento: Movimentação de Estoque (Histórico)
/// </summary>
public class EstoqueMovimentacaoEvent : BaseEvent
{
    public EstoqueMovimentacaoEvent(int produtoId, int quantidade, string tipoMovimentacao, string? notaFiscal, string? observacoes)
    {
        Evento = "estoque.movimentacao";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            produtoId = $"P{produtoId}",
            quantidade,
            tipoMovimentacao,
            notaFiscal,
            observacoes
        };
    }
} 