using System.ComponentModel.DataAnnotations;
using SportStore.Domain.ValueObjects;

namespace SportStore.Domain;

public enum StatusPedido
{
    Pendente = 1,
    Processando = 2,
    Finalizado = 3,
    Cancelado = 4
}

public class Pedido
{
    public int Id { get; set; }
    
    // Usando Value Object Documento para validação de CPF/CNPJ
    public Documento? DocumentoCliente { get; private set; }
    
    [Required]
    [MaxLength(200)]
    public string NomeCliente { get; set; } = string.Empty;
    
    public int VendedorId { get; set; }
    public virtual Usuario Vendedor { get; set; } = null!;
    
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    
    // Usando Value Object Preco para valor total
    public Preco ValorTotal { get; private set; } = Preco.Zero;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataFinalizacao { get; set; }
    
    public virtual ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

    /// <summary>
    /// Define o documento do cliente
    /// </summary>
    public void DefinirDocumentoCliente(Documento documento)
    {
        DocumentoCliente = documento;
    }

    /// <summary>
    /// Define o documento do cliente a partir de string
    /// </summary>
    public void DefinirDocumentoCliente(string documento)
    {
        DocumentoCliente = new Documento(documento);
    }

    /// <summary>
    /// Define o valor total do pedido
    /// </summary>
    public void DefinirValorTotal(Preco valor)
    {
        ValorTotal = valor;
    }

    /// <summary>
    /// Define o valor total do pedido a partir de decimal
    /// </summary>
    public void DefinirValorTotal(decimal valor)
    {
        ValorTotal = new Preco(valor);
    }

    /// <summary>
    /// Obtém o valor total como decimal
    /// </summary>
    public decimal ObterValorTotalDecimal()
    {
        return ValorTotal;
    }

    /// <summary>
    /// Obtém o valor total formatado
    /// </summary>
    public string ObterValorTotalFormatado()
    {
        return ValorTotal.FormatoBrasileiro;
    }

    /// <summary>
    /// Obtém o documento do cliente como string
    /// </summary>
    public string ObterDocumentoClienteString()
    {
        return DocumentoCliente?.Numero ?? string.Empty;
    }

    /// <summary>
    /// Obtém o documento formatado
    /// </summary>
    public string ObterDocumentoFormatado()
    {
        return DocumentoCliente?.NumeroFormatado ?? string.Empty;
    }
}