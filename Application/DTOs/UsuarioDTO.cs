using SportStore.Domain;
using SportStore.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace SportStore.Application.DTOs;

/// <summary>
/// DTO para criação de usuário
/// </summary>
public class UsuarioCreateDTO
{
    /// <summary>
    /// Nome completo do usuário (mínimo 2 partes, máximo 60 caracteres, sem números ou caracteres especiais)
    /// </summary>
    /// <example>João Silva</example>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [NomeCompleto]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email único do usuário (apenas caracteres ASCII - sem acentos, cedilhas, etc.)
    /// </summary>
    /// <example>joao.silva@email.com</example>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Email deve ter entre 5 e 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário (mínimo 8 caracteres, deve conter letra maiúscula, minúscula, número e caractere especial)
    /// </summary>
    /// <example>Senha@123</example>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [SenhaSegura]
    public string Senha { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usuário: 1 = Administrador, 2 = Vendedor (apenas estes valores são permitidos)
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
    [TipoUsuarioValido]
    public TipoUsuario TipoUsuario { get; set; }
}

/// <summary>
/// DTO de resposta com dados do usuário
/// </summary>
public class UsuarioResponseDTO
{
    /// <summary>
    /// Identificador único do usuário
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// UUID único do usuário
    /// </summary>
    public Guid Uuid { get; set; }

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usuário (1 = Administrador, 2 = Vendedor)
    /// </summary>
    public TipoUsuario TipoUsuario { get; set; }

    /// <summary>
    /// Data de criação da conta
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Status da conta (ativo/inativo)
    /// </summary>
    public bool Ativo { get; set; }
}

/// <summary>
/// DTO para autenticação de usuário
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    /// <example>admin@sportstore.com</example>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public UsuarioResponseDTO Usuario { get; set; } = null!;
}


