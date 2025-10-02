using SportStore.Application.DTOs;

namespace SportStore.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoResponseDTO> CriarPedidoAsync(PedidoCreateDTO pedidoDto, int vendedorId);
    Task<PedidoResponseDTO?> ObterPedidoPorIdAsync(int id);
    Task<List<PedidoResponseDTO>> ListarPedidosAsync();
}



