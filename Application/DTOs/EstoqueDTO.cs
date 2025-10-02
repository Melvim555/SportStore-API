using SportStore.Domain;

namespace SportStore.Application.DTOs;

public class EstoqueCreateDTO
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public string? NotaFiscal { get; set; }
    public string? Observacoes { get; set; }
}

public class EstoqueResponseDTO
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public TipoMovimentacao TipoMovimentacao { get; set; }
    public string? NotaFiscal { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
}



