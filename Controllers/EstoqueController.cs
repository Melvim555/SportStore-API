using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;

namespace SportStore.Controllers;

/// <summary>
/// Controller responsável pelo controle de estoque e movimentações
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EstoqueController : BaseController
{
    private readonly IEstoqueService _estoqueService;

    public EstoqueController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>
    /// Adiciona produtos ao estoque (Somente Administradores)
    /// </summary>
    /// <param name="estoqueDto">Informações da movimentação de entrada de estoque</param>
    /// <returns>Registro da movimentação de estoque criada</returns>
    /// <response code="201">Movimentação de estoque registrada com sucesso</response>
    /// <response code="400">Dados inválidos ou produto não encontrado</response>
    /// <response code="401">Token de acesso inválido ou expirado</response>
    /// <response code="403">Usuário não possui permissão de administrador</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("adicionar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(EstoqueResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EstoqueResponseDTO>> AdicionarEstoque([FromBody] EstoqueCreateDTO estoqueDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsAdmin())
                return Forbid();

            var usuario = ObterUsuarioLogado();
            var movimentacao = await _estoqueService.AdicionarEstoqueAsync(estoqueDto, usuario.Id);
            
            return CreatedAtAction(nameof(ObterHistoricoMovimentacao), movimentacao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtém histórico de movimentações de estoque (somente Admin)
    /// </summary>
    /// <returns>Histórico de movimentações</returns>
    [HttpGet("historico")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<List<EstoqueResponseDTO>>> ObterHistoricoMovimentacao()
    {
        if (!IsAdmin())
            return Forbid();

        var historico = await _estoqueService.ObterHistoricoMovimentacaoAsync();
        return Ok(historico);
    }

    /// <summary>
    /// Obtém quantidade disponível de um produto
    /// </summary>
    /// <param name="produtoId">ID do produto</param>
    /// <returns>Quantidade disponível</returns>
    [HttpGet("disponivel/{produtoId}")]
    public async Task<ActionResult<int>> ObterQuantidadeDisponivel(int produtoId)
    {
        var quantidade = await _estoqueService.ObterQuantidadeDisponivelAsync(produtoId);
        return Ok(new { produtoId, quantidadeDisponivel = quantidade });
    }
}


