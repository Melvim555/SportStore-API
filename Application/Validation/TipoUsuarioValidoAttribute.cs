using System.ComponentModel.DataAnnotations;
using SportStore.Domain;

namespace SportStore.Application.Validation;

/// <summary>
/// Valida se o tipo de usuário é válido (1 = Administrador, 2 = Vendedor)
/// </summary>
public class TipoUsuarioValidoAttribute : ValidationAttribute
{
    public TipoUsuarioValidoAttribute()
    {
        ErrorMessage = "Tipo de usuário deve ser 1 (Administrador) ou 2 (Vendedor)";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Tipo de usuário é obrigatório");
        }

        // Verificar se é um valor inteiro válido
        if (value is TipoUsuario tipoUsuario)
        {
            var valorInt = (int)tipoUsuario;
            
            // Verificar se é exatamente 1 ou 2 (sem decimais ou valores inválidos)
            if (valorInt != 1 && valorInt != 2)
            {
                return new ValidationResult("Tipo de usuário inválido. Use apenas: 1 (Administrador) ou 2 (Vendedor)");
            }
            
            // Verifica se é Administrador (1) ou Vendedor (2)
            if (tipoUsuario == TipoUsuario.Administrador || tipoUsuario == TipoUsuario.Vendedor)
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult("Tipo de usuário inválido. Deve ser um número inteiro: 1 (Administrador) ou 2 (Vendedor)");
    }
}

