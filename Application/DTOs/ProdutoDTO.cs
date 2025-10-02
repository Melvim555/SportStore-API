using System.ComponentModel.DataAnnotations;
using SportStore.Application.Validation;

namespace SportStore.Application.DTOs;

public class ProdutoCreateDTO
{
    /// <summary>
    /// Nome do produto
    /// </summary>
    /// <example>Bola de Futebol Oficial</example>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>
    /// Descrição detalhada do produto
    /// </summary>
    /// <example>Bola oficial de futebol FIFA, tamanho 5, material premium</example>
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    /// <summary>
    /// Preço do produto (máximo 2 casas decimais)
    /// </summary>
    /// <example>89.90</example>
    [Required(ErrorMessage = "Preço é obrigatório")]
    [PrecoValidation]
    public decimal Preco { get; set; }
}

public class ProdutoUpdateDTO
{
    /// <summary>
    /// Nome do produto
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>
    /// Descrição detalhada do produto
    /// </summary>
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    /// <summary>
    /// Preço do produto (máximo 2 casas decimais)
    /// </summary>
    [Required(ErrorMessage = "Preço é obrigatório")]
    [PrecoValidation]
    public decimal Preco { get; set; }
    
    /// <summary>
    /// Status do produto (ativo/inativo)
    /// </summary>
    [Required(ErrorMessage = "Status é obrigatório")]
    public bool Ativo { get; set; }
}

public class ProdutoResponseDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
    public int QuantidadeDisponivel { get; set; }
}


