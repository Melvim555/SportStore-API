using System.Linq.Expressions;

namespace SportStore.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}

public interface IUsuarioRepository : IRepository<Domain.Usuario>
{
    Task<Domain.Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}

public interface IProdutoRepository : IRepository<Domain.Produto>
{
    Task<IEnumerable<Domain.Produto>> GetAtivosAsync();
    Task<Domain.Produto?> GetWithEstoqueAsync(int id);
}

public interface IEstoqueRepository : IRepository<Domain.Estoque>
{
    Task<IEnumerable<Domain.Estoque>> GetByProdutoAsync(int produtoId);
    Task<int> GetQuantidadeDisponivelAsync(int produtoId);
}

public interface IPedidoRepository : IRepository<Domain.Pedido>
{
    Task<Domain.Pedido?> GetWithItensAsync(int id);
    Task<IEnumerable<Domain.Pedido>> GetByVendedorAsync(int vendedorId);
}
