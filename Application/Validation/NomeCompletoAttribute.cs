using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SportStore.Application.Validation;

/// <summary>
/// Valida se o nome atende aos requisitos de nome completo
/// </summary>
public class NomeCompletoAttribute : ValidationAttribute
{
    public NomeCompletoAttribute()
    {
        ErrorMessage = "Nome inválido";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Nome é obrigatório");
        }

        var nome = value.ToString()!.Trim();

        // 1. Validar máximo 60 caracteres
        if (nome.Length > 60)
        {
            return new ValidationResult("Nome deve ter no máximo 60 caracteres");
        }

        // 2. Validar se não tem números
        if (Regex.IsMatch(nome, @"\d"))
        {
            return new ValidationResult("Nome não pode conter números");
        }

        // 3. Validar se não tem caracteres especiais (exceto espaço e hífen)
        if (Regex.IsMatch(nome, @"[^a-zA-ZÀ-ÿ\s\-]"))
        {
            return new ValidationResult("Nome não pode conter caracteres especiais (apenas letras, espaços e hífen)");
        }

        // 4. Dividir em partes (separado por espaços)
        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // 5. Validar mínimo 2 partes (nome + sobrenome)
        if (partes.Length < 2)
        {
            return new ValidationResult("Nome deve ter pelo menos 2 partes (nome e sobrenome)");
        }

        // 6. Validar máximo 5 partes
        if (partes.Length > 5)
        {
            return new ValidationResult("Nome deve ter no máximo 5 partes");
        }

        // 7. Validar que cada parte tem pelo menos 2 letras
        foreach (var parte in partes)
        {
            // Remover hífen da parte para contar apenas letras
            var letrasApenas = Regex.Replace(parte, @"[^a-zA-ZÀ-ÿ]", "");
            
            if (letrasApenas.Length < 2)
            {
                return new ValidationResult($"Cada parte do nome deve ter pelo menos 2 letras ('{parte}' é inválido)");
            }
        }

        return ValidationResult.Success;
    }
} 