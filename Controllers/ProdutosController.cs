using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;

namespace SportStore.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de produtos do catálogo
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : BaseController
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Cadastra um novo produto no catálogo (Somente Administradores)
    /// </summary>
    /// <param name="produtoDto">Informações do produto a ser cadastrado</param>
    /// <returns>Dados do produto criado com sucesso</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Token de acesso inválido ou expirado</response>
    /// <response code="403">Usuário não possui permissão de administrador</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoResponseDTO>> CriarProduto([FromBody] ProdutoCreateDTO produtoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                
                return BadRequest(new { 
                    message = "Dados inválidos", 
                    errors = errors 
                });
            }

            if (!IsAdmin())
            {
                var usuario = base.ObterUsuarioLogado();
                return Forbid($"Acesso negado. Usuário atual: {usuario.Nome} (Tipo: {usuario.TipoUsuario}). Apenas administradores podem criar produtos");
            }

            var produto = await _produtoService.CriarProdutoAsync(produtoDto);
            
            // Debug: verificar se o produto foi criado corretamente
            if (produto == null)
                return StatusCode(500, "Erro: Produto não foi criado");
                
            // Retornar diretamente o produto criado
            return Ok(produto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (System.Text.Json.JsonException)
        {
            return BadRequest(new { 
                message = "Formato JSON inválido. Use ponto (.) para decimais, não vírgula (,). Exemplo: 29.90",
                error = "Formato de número inválido"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Erro interno do servidor", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Obtém informações de um produto específico
    /// </summary>
    /// <param name="id">Identificador único do produto</param>
    /// <returns>Dados completos do produto incluindo quantidade disponível</returns>
    /// <response code="200">Produto encontrado com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="401">Token de acesso inválido ou expirado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProdutoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoResponseDTO>> ObterProduto(int id)
    {
        try
        {
            var produto = await _produtoService.ObterProdutoPorIdAsync(id);
            if (produto == null)
                return NotFound($"Produto com ID {id} não encontrado");

            return Ok(produto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Lista todos os produtos ativos do catálogo
    /// </summary>
    /// <returns>Lista de produtos com informações de estoque</returns>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    /// <response code="401">Token de acesso inválido ou expirado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProdutoResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProdutoResponseDTO>>> ListarProdutos()
    {
        try
        {
            var produtos = await _produtoService.ListarProdutosAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao listar produtos: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza um produto (somente Admin)
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="produtoDto">Dados atualizados do produto</param>
    /// <returns>Produto atualizado</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoResponseDTO>> AtualizarProduto(int id, [FromBody] ProdutoUpdateDTO produtoDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsAdmin())
                return Forbid();

            var produto = await _produtoService.AtualizarProdutoAsync(id, produtoDto);
            return Ok(produto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Exclui um produto (somente Admin)
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> ExcluirProduto(int id)
    {
        if (!IsAdmin())
            return Forbid();

        var sucesso = await _produtoService.ExcluirProdutoAsync(id);
        if (!sucesso)
            return NotFound();

        return NoContent();
    }
}


