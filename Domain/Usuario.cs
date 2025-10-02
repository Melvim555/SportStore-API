using System.ComponentModel.DataAnnotations;

namespace SportStore.Domain;

public enum TipoUsuario
{
    Administrador = 1,
    Vendedor = 2
}

public class Usuario
{
    public int Id { get; set; }

    public Guid Uuid { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = string.Empty;

    public TipoUsuario TipoUsuario { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public bool Ativo { get; set; } = true;
}


