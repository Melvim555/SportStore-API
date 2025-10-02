using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;

namespace SportStore.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de pedidos e vendas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PedidosController : BaseController
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    /// <summary>
    /// Cria um novo pedido de venda (Somente Vendedores)
    /// </summary>
    /// <param name="pedidoDto">Informações do pedido incluindo cliente e itens</param>
    /// <returns>Pedido criado com sucesso e baixa automática no estoque</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Dados inválidos, estoque insuficiente ou produto não encontrado</response>
    /// <response code="401">Token de acesso inválido ou expirado</response>
    /// <response code="403">Usuário não possui permissão de vendedor</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [Authorize(Roles = "Vendedor")]
    [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PedidoResponseDTO>> CriarPedido([FromBody] PedidoCreateDTO pedidoDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsVendedor())
                return Forbid();

            var usuario = ObterUsuarioLogado();
            var pedido = await _pedidoService.CriarPedidoAsync(pedidoDto, usuario.Id);
            
            return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtém pedido por ID
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Dados do pedido</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PedidoResponseDTO>> ObterPedido(int id)
    {
        var pedido = await _pedidoService.ObterPedidoPorIdAsync(id);
        if (pedido == null)
            return NotFound();

        return Ok(pedido);
    }

    /// <summary>
    /// Lista todos os pedidos
    /// </summary>
    /// <returns>Lista de pedidos</returns>
    [HttpGet]
    public async Task<ActionResult<List<PedidoResponseDTO>>> ListarPedidos()
    {
        var pedidos = await _pedidoService.ListarPedidosAsync();
        return Ok(pedidos);
    }
}


