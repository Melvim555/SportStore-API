using SportStore.Domain;
using SportStore.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace SportStore.Application.DTOs;

/// <summary>
/// DTO para cria��o de usu�rio
/// </summary>
public class UsuarioCreateDTO
{
    /// <summary>
    /// Nome completo do usu�rio (m�nimo 2 partes, m�ximo 60 caracteres, sem n�meros ou caracteres especiais)
    /// </summary>
    /// <example>Jo�o Silva</example>
    [Required(ErrorMessage = "Nome � obrigat�rio")]
    [NomeCompleto]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email �nico do usu�rio (apenas caracteres ASCII - sem acentos, cedilhas, etc.)
    /// </summary>
    /// <example>joao.silva@email.com</example>
    [Required(ErrorMessage = "Email � obrigat�rio")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato v�lido")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Email deve ter entre 5 e 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usu�rio (m�nimo 8 caracteres, deve conter letra mai�scula, min�scula, n�mero e caractere especial)
    /// </summary>
    /// <example>Senha@123</example>
    [Required(ErrorMessage = "Senha � obrigat�ria")]
    [SenhaSegura]
    public string Senha { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usu�rio: 1 = Administrador, 2 = Vendedor (apenas estes valores s�o permitidos)
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Tipo de usu�rio � obrigat�rio")]
    [TipoUsuarioValido]
    public TipoUsuario TipoUsuario { get; set; }
}

/// <summary>
/// DTO de resposta com dados do usu�rio
/// </summary>
public class UsuarioResponseDTO
{
    /// <summary>
    /// Identificador �nico do usu�rio
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// UUID �nico do usu�rio
    /// </summary>
    public Guid Uuid { get; set; }

    /// <summary>
    /// Nome completo do usu�rio
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do usu�rio
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usu�rio (1 = Administrador, 2 = Vendedor)
    /// </summary>
    public TipoUsuario TipoUsuario { get; set; }

    /// <summary>
    /// Data de cria��o da conta
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Status da conta (ativo/inativo)
    /// </summary>
    public bool Ativo { get; set; }
}

/// <summary>
/// DTO para autentica��o de usu�rio
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// Email do usu�rio
    /// </summary>
    /// <example>admin@sportstore.com</example>
    [Required(ErrorMessage = "Email � obrigat�rio")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato v�lido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usu�rio
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "Senha � obrigat�ria")]
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public UsuarioResponseDTO Usuario { get; set; } = null!;
}


