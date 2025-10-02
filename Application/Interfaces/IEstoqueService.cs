using SportStore.Application.DTOs;

namespace SportStore.Application.Interfaces;

public interface IEstoqueService
{
    Task<EstoqueResponseDTO> AdicionarEstoqueAsync(EstoqueCreateDTO estoqueDto, int usuarioId);
    Task<List<EstoqueResponseDTO>> ObterHistoricoMovimentacaoAsync();
    Task<int> ObterQuantidadeDisponivelAsync(int produtoId);
    Task<bool> VerificarDisponibilidadeAsync(int produtoId, int quantidade);
}



