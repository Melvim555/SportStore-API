namespace SportStore.Application.Events;

/// <summary>
/// Evento: Produto Cadastrado
/// </summary>
public class ProdutoCadastradoEvent : BaseEvent
{
    public ProdutoCadastradoEvent(int produtoId, string nome, string descricao, decimal preco)
    {
        Evento = "produtos.cadastrado";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            produtoId = $"P{produtoId}",
            nome,
            descricao,
            preco
        };
    }
}

/// <summary>
/// Evento: Produto Atualizado
/// </summary>
public class ProdutoAtualizadoEvent : BaseEvent
{
    public ProdutoAtualizadoEvent(int produtoId, string nome, string descricao, decimal preco, bool ativo)
    {
        Evento = "produtos.atualizado";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            produtoId = $"P{produtoId}",
            nome,
            descricao,
            preco,
            ativo
        };
    }
}

/// <summary>
/// Evento: Produto Removido
/// </summary>
public class ProdutoRemovidoEvent : BaseEvent
{
    public ProdutoRemovidoEvent(int produtoId, string nome)
    {
        Evento = "produtos.removido";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            produtoId = $"P{produtoId}",
            nome
        };
    }
} 