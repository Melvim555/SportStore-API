using System.ComponentModel.DataAnnotations;
using SportStore.Domain;

namespace SportStore.Application.DTOs;

public class ItemPedidoCreateDTO
{
    [Required(ErrorMessage = "ID do produto é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do produto deve ser maior que zero")]
    public int ProdutoId { get; set; }
    
    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }
}

public class PedidoCreateDTO
{
    /// <summary>
    /// CPF (11 dígitos) ou CNPJ (14 dígitos) do cliente
    /// </summary>
    /// <example>12345678901</example>
    [Required(ErrorMessage = "Documento do cliente é obrigatório")]
    [StringLength(14, MinimumLength = 11, ErrorMessage = "Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ)")]
    public string DocumentoCliente { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome completo do cliente
    /// </summary>
    /// <example>João Silva Santos</example>
    [Required(ErrorMessage = "Nome do cliente é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 200 caracteres")]
    public string NomeCliente { get; set; } = string.Empty;
    
    /// <summary>
    /// Lista de itens do pedido
    /// </summary>
    [Required(ErrorMessage = "Lista de itens é obrigatória")]
    [MinLength(1, ErrorMessage = "Pedido deve ter pelo menos um item")]
    public List<ItemPedidoCreateDTO> Itens { get; set; } = new();
}

public class ItemPedidoResponseDTO
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class PedidoResponseDTO
{
    public int Id { get; set; }
    public string DocumentoCliente { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string VendedorNome { get; set; } = string.Empty;
    public StatusPedido Status { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public List<ItemPedidoResponseDTO> Itens { get; set; } = new();
}