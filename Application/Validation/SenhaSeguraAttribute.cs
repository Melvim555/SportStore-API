using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SportStore.Application.Validation;

/// <summary>
/// Valida se a senha atende aos requisitos de segurança
/// </summary>
public class SenhaSeguraAttribute : ValidationAttribute
{
    public SenhaSeguraAttribute()
    {
        ErrorMessage = "Senha deve conter no mínimo 8 caracteres, pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial (@$!%*?&)";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Senha é obrigatória");
        }

        var senha = value.ToString()!;

        // Validar tamanho mínimo
        if (senha.Length < 8)
        {
            return new ValidationResult("Senha deve ter no mínimo 8 caracteres");
        }

        // Validar se tem pelo menos uma letra minúscula
        if (!Regex.IsMatch(senha, @"[a-z]"))
        {
            return new ValidationResult("Senha deve conter pelo menos uma letra minúscula");
        }

        // Validar se tem pelo menos uma letra maiúscula
        if (!Regex.IsMatch(senha, @"[A-Z]"))
        {
            return new ValidationResult("Senha deve conter pelo menos uma letra maiúscula");
        }

        // Validar se tem pelo menos um número
        if (!Regex.IsMatch(senha, @"[0-9]"))
        {
            return new ValidationResult("Senha deve conter pelo menos um número");
        }

        // Validar se tem pelo menos um caractere especial
        if (!Regex.IsMatch(senha, @"[@$!%*?&#]"))
        {
            return new ValidationResult("Senha deve conter pelo menos um caractere especial (@$!%*?&#)");
        }

        return ValidationResult.Success;
    }
} 