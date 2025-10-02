using SportStore.Application.DTOs;

namespace SportStore.Application.Interfaces;

public interface IProdutoService
{
    Task<ProdutoResponseDTO> CriarProdutoAsync(ProdutoCreateDTO produtoDto);
    Task<ProdutoResponseDTO?> ObterProdutoPorIdAsync(int id);
    Task<List<ProdutoResponseDTO>> ListarProdutosAsync();
    Task<ProdutoResponseDTO?> AtualizarProdutoAsync(int id, ProdutoUpdateDTO produtoDto);
    Task<bool> ExcluirProdutoAsync(int id);
}



