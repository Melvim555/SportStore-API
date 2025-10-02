using Microsoft.EntityFrameworkCore;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;
using SportStore.Domain;
using SportStore.Domain.ValueObjects;
using SportStore.Infrastructure;

namespace SportStore.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly SportStoreContext _context;
    private readonly IEstoqueService _estoqueService;

    public PedidoService(SportStoreContext context, IEstoqueService estoqueService)
    {
        _context = context;
        _estoqueService = estoqueService;
    }

    public async Task<PedidoResponseDTO> CriarPedidoAsync(PedidoCreateDTO pedidoDto, int vendedorId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Verificar se todos os produtos existem e têm estoque
            foreach (var item in pedidoDto.Itens)
            {
                var produto = await _context.Produtos
                    .Include(p => p.Estoques)
                    .FirstOrDefaultAsync(p => p.Id == item.ProdutoId && p.Ativo);

                if (produto == null)
                    throw new InvalidOperationException($"Produto com ID {item.ProdutoId} não encontrado ou inativo.");

                var quantidadeDisponivel = produto.Estoques
                    .Sum(e => e.TipoMovimentacao == TipoMovimentacao.Entrada ? e.Quantidade : -e.Quantidade);

                if (quantidadeDisponivel < item.Quantidade)
                {
                    throw new InvalidOperationException($"Produto '{produto?.Nome}' não possui quantidade suficiente em estoque. Disponível: {quantidadeDisponivel}, Solicitada: {item.Quantidade}");
                }
            }

            // Criar o pedido
            var pedido = new Pedido
            {
                NomeCliente = pedidoDto.NomeCliente,
                VendedorId = vendedorId,
                Status = StatusPedido.Pendente,
                DataCriacao = DateTime.UtcNow
            };

            // Usar Value Object Documento
            pedido.DefinirDocumentoCliente(pedidoDto.DocumentoCliente);
            pedido.DefinirValorTotal(0m);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Criar itens do pedido e calcular valor total
            decimal valorTotal = 0;

            foreach (var itemDto in pedidoDto.Itens)
            {
                var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);

                var itemPedido = new ItemPedido
                {
                    PedidoId = pedido.Id,
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade
                };

                // Usar Value Object Preco
                itemPedido.DefinirPrecoUnitario(produto!.ObterValorDecimal());

                _context.ItensPedido.Add(itemPedido);

                // Calcular subtotal usando Value Objects
                var subtotal = itemPedido.ObterSubtotalDecimal();
                valorTotal += subtotal;

                // Dar baixa no estoque
                var estoqueDto = new EstoqueCreateDTO
                {
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = -itemDto.Quantidade, // Negativo para saída
                    NotaFiscal = $"PEDIDO-{pedido.Id}",
                    Observacoes = $"Baixa automática do pedido #{pedido.Id}"
                };
                await _estoqueService.AdicionarEstoqueAsync(estoqueDto, vendedorId);
            }

            // Atualizar valor total do pedido
            pedido.DefinirValorTotal(valorTotal);
            pedido.Status = StatusPedido.Finalizado;
            pedido.DataFinalizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await ObterPedidoPorIdAsync(pedido.Id) ?? throw new InvalidOperationException("Erro ao recuperar pedido criado.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PedidoResponseDTO?> ObterPedidoPorIdAsync(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Vendedor)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return null;

        return new PedidoResponseDTO
        {
            Id = pedido.Id,
            DocumentoCliente = pedido.ObterDocumentoClienteString(),
            NomeCliente = pedido.NomeCliente,
            VendedorNome = pedido.Vendedor.Nome,
            Status = pedido.Status,
            ValorTotal = pedido.ObterValorTotalDecimal(),
            DataCriacao = pedido.DataCriacao,
            DataFinalizacao = pedido.DataFinalizacao,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponseDTO
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto.Nome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.ObterPrecoUnitarioDecimal(),
                Subtotal = i.ObterSubtotalDecimal()
            }).ToList()
        };
    }

    public async Task<List<PedidoResponseDTO>> ListarPedidosAsync()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Vendedor)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();

        return pedidos.Select(p => new PedidoResponseDTO
        {
            Id = p.Id,
            DocumentoCliente = p.ObterDocumentoClienteString(),
            NomeCliente = p.NomeCliente,
            VendedorNome = p.Vendedor.Nome,
            Status = p.Status,
            ValorTotal = p.ObterValorTotalDecimal(),
            DataCriacao = p.DataCriacao,
            DataFinalizacao = p.DataFinalizacao,
            Itens = p.Itens.Select(i => new ItemPedidoResponseDTO
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto.Nome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.ObterPrecoUnitarioDecimal(),
                Subtotal = i.ObterSubtotalDecimal()
            }).ToList()
        }).ToList();
    }

}