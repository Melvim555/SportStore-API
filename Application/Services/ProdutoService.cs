using Microsoft.EntityFrameworkCore;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;
using SportStore.Domain;
using SportStore.Domain.ValueObjects;
using SportStore.Infrastructure;

namespace SportStore.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly SportStoreContext _context;

    public ProdutoService(SportStoreContext context)
    {
        _context = context;
    }

    public async Task<ProdutoResponseDTO> CriarProdutoAsync(ProdutoCreateDTO produtoDto)
    {
        // Verificar se já existe um produto ativo com o mesmo nome
        var produtoExistente = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Nome.ToLower() == produtoDto.Nome.ToLower() && p.Ativo);

        if (produtoExistente != null)
        {
            throw new InvalidOperationException($"Já existe um produto ativo com o nome '{produtoDto.Nome}'.");
        }

        var produto = new Produto
        {
            Nome = produtoDto.Nome,
            Descricao = produtoDto.Descricao,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        // Usar Value Object Preco
        produto.DefinirPreco(produtoDto.Preco);

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return new ProdutoResponseDTO
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.ObterValorDecimal(),
            DataCriacao = produto.DataCriacao,
            Ativo = produto.Ativo,
            QuantidadeDisponivel = 0
        };
    }

    public async Task<ProdutoResponseDTO?> ObterProdutoPorIdAsync(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Estoques)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

        if (produto == null)
            return null;

        var quantidadeDisponivel = CalcularQuantidadeDisponivel(produto.Estoques);

        return new ProdutoResponseDTO
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.ObterValorDecimal(),
            DataCriacao = produto.DataCriacao,
            Ativo = produto.Ativo,
            QuantidadeDisponivel = quantidadeDisponivel
        };
    }

    public async Task<List<ProdutoResponseDTO>> ListarProdutosAsync()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Estoques)
            .Where(p => p.Ativo)
            .ToListAsync();

        return produtos.Select(p => new ProdutoResponseDTO
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Preco = p.ObterValorDecimal(),
            DataCriacao = p.DataCriacao,
            Ativo = p.Ativo,
            QuantidadeDisponivel = CalcularQuantidadeDisponivel(p.Estoques)
        }).ToList();
    }

    public async Task<ProdutoResponseDTO?> AtualizarProdutoAsync(int id, ProdutoUpdateDTO produtoDto)
    {
        // Verificar se já existe outro produto ativo com o mesmo nome (excluindo o atual)
        var produtoComMesmoNome = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Nome.ToLower() == produtoDto.Nome.ToLower() 
                                   && p.Ativo 
                                   && p.Id != id);

        if (produtoComMesmoNome != null)
        {
            throw new InvalidOperationException($"Já existe outro produto ativo com o nome '{produtoDto.Nome}'.");
        }

        var produto = await _context.Produtos
            .Include(p => p.Estoques)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return null;

        produto.Nome = produtoDto.Nome;
        produto.Descricao = produtoDto.Descricao;
        produto.DefinirPreco(produtoDto.Preco);
        produto.Ativo = produtoDto.Ativo;

        await _context.SaveChangesAsync();

        var quantidadeDisponivel = CalcularQuantidadeDisponivel(produto.Estoques);

        return new ProdutoResponseDTO
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.ObterValorDecimal(),
            DataCriacao = produto.DataCriacao,
            Ativo = produto.Ativo,
            QuantidadeDisponivel = quantidadeDisponivel
        };
    }

    public async Task<bool> ExcluirProdutoAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
            return false;

        produto.Ativo = false; // Soft delete
        await _context.SaveChangesAsync();

        return true;
    }

    private static int CalcularQuantidadeDisponivel(ICollection<Estoque> estoques)
    {
        return estoques.Sum(e => e.TipoMovimentacao == TipoMovimentacao.Entrada ? e.Quantidade : -e.Quantidade);
    }
}