using Microsoft.EntityFrameworkCore;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;
using SportStore.Domain;
using SportStore.Infrastructure;

namespace SportStore.Application.Services;

public class EstoqueService : IEstoqueService
{
    private readonly SportStoreContext _context;

    public EstoqueService(SportStoreContext context)
    {
        _context = context;
    }

    public async Task<EstoqueResponseDTO> AdicionarEstoqueAsync(EstoqueCreateDTO estoqueDto, int usuarioId)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == estoqueDto.ProdutoId && p.Ativo);

        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        var estoque = new Estoque
        {
            ProdutoId = estoqueDto.ProdutoId,
            Quantidade = estoqueDto.Quantidade,
            TipoMovimentacao = TipoMovimentacao.Entrada,
            NotaFiscal = estoqueDto.NotaFiscal,
            Observacoes = estoqueDto.Observacoes,
            DataMovimentacao = DateTime.UtcNow,
            UsuarioId = usuarioId
        };

        _context.Estoques.Add(estoque);
        await _context.SaveChangesAsync();

        var usuario = await _context.Usuarios.FindAsync(usuarioId);

        return new EstoqueResponseDTO
        {
            Id = estoque.Id,
            ProdutoId = estoque.ProdutoId,
            ProdutoNome = produto.Nome,
            Quantidade = estoque.Quantidade,
            TipoMovimentacao = estoque.TipoMovimentacao,
            NotaFiscal = estoque.NotaFiscal,
            Observacoes = estoque.Observacoes,
            DataMovimentacao = estoque.DataMovimentacao,
            UsuarioNome = usuario?.Nome ?? "Desconhecido"
        };
    }

    public async Task<List<EstoqueResponseDTO>> ObterHistoricoMovimentacaoAsync()
    {
        var movimentacoes = await _context.Estoques
            .Include(e => e.Produto)
            .Include(e => e.Usuario)
            .OrderByDescending(e => e.DataMovimentacao)
            .ToListAsync();

        return movimentacoes.Select(e => new EstoqueResponseDTO
        {
            Id = e.Id,
            ProdutoId = e.ProdutoId,
            ProdutoNome = e.Produto.Nome,
            Quantidade = e.Quantidade,
            TipoMovimentacao = e.TipoMovimentacao,
            NotaFiscal = e.NotaFiscal,
            Observacoes = e.Observacoes,
            DataMovimentacao = e.DataMovimentacao,
            UsuarioNome = e.Usuario.Nome
        }).ToList();
    }

    public async Task<int> ObterQuantidadeDisponivelAsync(int produtoId)
    {
        var movimentacoes = await _context.Estoques
            .Where(e => e.ProdutoId == produtoId)
            .ToListAsync();

        return movimentacoes.Sum(e => e.TipoMovimentacao == TipoMovimentacao.Entrada ? e.Quantidade : -e.Quantidade);
    }

    public async Task<bool> VerificarDisponibilidadeAsync(int produtoId, int quantidade)
    {
        var quantidadeDisponivel = await ObterQuantidadeDisponivelAsync(produtoId);
        return quantidadeDisponivel >= quantidade;
    }

    public async Task DarBaixaEstoqueAsync(int produtoId, int quantidade, int usuarioId, string observacoes = "")
    {
        var quantidadeDisponivel = await ObterQuantidadeDisponivelAsync(produtoId);
        
        if (quantidadeDisponivel < quantidade)
            throw new InvalidOperationException($"Quantidade insuficiente em estoque. Disponível: {quantidadeDisponivel}, Solicitada: {quantidade}");

        var estoque = new Estoque
        {
            ProdutoId = produtoId,
            Quantidade = quantidade,
            TipoMovimentacao = TipoMovimentacao.Saida,
            Observacoes = observacoes,
            DataMovimentacao = DateTime.UtcNow,
            UsuarioId = usuarioId
        };

        _context.Estoques.Add(estoque);
        await _context.SaveChangesAsync();
    }
}



